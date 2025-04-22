using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commands;
using Characters;
using Dialogue.LogicalLines;
using System;

namespace Dialogue
{
    public class ConversationManager
    {
        private DialogueManager dialogueManager => DialogueManager.Instance;
        private SceneManager sceneManager => SceneManager.Instance;

        private Coroutine process = null;

        public bool isRunning => process != null;

        public TextArchitect textArchitect = null;

        private bool userNext = false;
        private float completionTime = 0.3f;

        private LogicalLineManager logicalLineManager;

        private ConversationQueue conversationQueue;
        public Conversation conversation => (conversationQueue.IsEmpty() ? null : conversationQueue.top);
        public int conversationProgress => (conversationQueue.IsEmpty() ? -1 : conversationQueue.top.GetProgress());

        public bool proceed = false;

        public event Action OnConversationEnd;

        public ConversationManager()
        {
            dialogueManager.onUserNext += OnUserNext;

            logicalLineManager = new LogicalLineManager();

            conversationQueue = new ConversationQueue();
        }

        public void Enqueue(Conversation conversation) => conversationQueue.Enqueue(conversation);

        public void EnqueuePriority(Conversation conversation) => conversationQueue.EnqueuePriority(conversation);

        private void OnUserNext()
        {
            userNext = true;
        }

        public Coroutine StartConversation(Conversation conversation)
        {
            StopConversation();
            conversationQueue.Clear();

            Enqueue(conversation);

            process = dialogueManager.StartCoroutine(RunningConversation());

            return process;
        }

        public void StopConversation()
        {
            if (!isRunning)
            {
                return;
            }

            dialogueManager.StopCoroutine(process);
            process = null;
        }

        IEnumerator RunningConversation()
        {
            while (!conversationQueue.IsEmpty())
            {
                Conversation currentConversation = conversation;

                if (currentConversation.HasReachedEnd())
                {
                    conversationQueue.Dequeue();
                    continue;
                }

                string rawLine = conversation.CurrentLine();

                if (string.IsNullOrWhiteSpace(rawLine))
                {
                    TryAdvanceConversation(currentConversation);
                    continue;
                }

                DialogueLine line = DialogueParser.Parse(rawLine);

                if (logicalLineManager.TryGetLogicalLine(line, out Coroutine logic))
                {
                    yield return logic;
                }
                else
                {
                    if (line.hasDialogue)
                    {
                        yield return RunDialogue(line);
                    }

                    if (line.hasCommands)
                    {
                        yield return RunCommands(line);
                    }

                    if (line.hasDialogue && !proceed)
                    {
                        yield return WaitForUserInput();
                    }
                }

                TryAdvanceConversation(currentConversation);
            }

            process = null;

            OnConversationEnd?.Invoke();
        }

        private void TryAdvanceConversation(Conversation conversation)
        {
            conversation.IncrementProgress();

            if (conversation != conversationQueue.top) return;

            if (conversation.HasReachedEnd())
            {
                conversationQueue.Dequeue();
            }
        }

        IEnumerator RunDialogue(DialogueLine line)
        {
            if (line.speakerData.talkInSpeechBubble)
            {
                GameObject speakerSprite;
                if (line.speakerData.name.Equals(sceneManager.MC_NAME))
                {
                    speakerSprite = sceneManager.player.root;
                }
                else
                {
                    speakerSprite = sceneManager.npcManager.GetNPC(line.speakerData.name)?.root 
                                     ?? sceneManager.interactableManager.GetInteractable(line.speakerData.name).gameObject;
                }
                

                yield return dialogueManager.ShowTextbox(DialogueContainer.ContainerType.SpeechBubble, speakerName: TagManager.Inject(line.speakerData.displayName), speakerSprite: speakerSprite);
            }
            else
            {
                if (line.hasSpeaker)
                {
                    HandleSpeakerLogic(line.speakerData);
                }

                yield return dialogueManager.ShowTextbox(line.speakerData.textboxPosition, speakerName: TagManager.Inject(line.speakerData.displayName));
            }

            yield return BuildLineSegments(line.dialogueData);
        }

        private void HandleSpeakerLogic(SpeakerData speakerData)
        {
            Character character = CharacterManager.Instance.GetCharacter(speakerData.name);

            if (speakerData.isCastingExpressions && character.isCharacterVisible)
            {
                foreach (var exp in speakerData.castExpressions)
                {
                    character.OnReceiveCastingExpression(exp.layer, exp.expression);
                }
            }
        }

        IEnumerator RunCommands(DialogueLine line)
        {
            List<CommandData.Command> commands = line.commandData.commands;

            foreach (CommandData.Command command in commands)
            {
                if (command.waitForCompletion || command.name == "Wait")
                {
                    yield return dialogueManager.commandManager.Execute(command.name, command.arguments);
                    yield return new WaitForSeconds(completionTime);
                }
                else if (command.waitForUserInput)
                {
                    yield return dialogueManager.commandManager.Execute(command.name, command.arguments);
                    yield return WaitForUserInput();
                }
                else
                {
                    dialogueManager.commandManager.Execute(command.name, command.arguments);
                }
            }

            yield return null;
        }

        IEnumerator BuildLineSegments(DialogueData line)
        {
            for (int i = 0; i < line.lineSegments.Count; i++)
            {
                DialogueData.DialogueSegment segment = line.lineSegments[i];

                yield return WaitForDialogueSegmentSignal(segment);

                yield return BuildDialogue(segment.dialogue, segment.appendText);
            }
        }

        IEnumerator WaitForDialogueSegmentSignal(DialogueData.DialogueSegment segment)
        {
            switch (segment.startSignal)
            {
                case DialogueData.DialogueSegment.StartSignal.C:
                case DialogueData.DialogueSegment.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DialogueData.DialogueSegment.StartSignal.WC:
                case DialogueData.DialogueSegment.StartSignal.WA:
                    yield return new WaitForSeconds(segment.signalDelay);
                    break;
                default:
                    break;
            }
        }

        IEnumerator BuildDialogue(string dialogue, bool append = false)
        {
            dialogue = TagManager.Inject(dialogue);

            if (!append)
            {
                textArchitect.Build(dialogue);
            }
            else
            {
                textArchitect.Append(dialogue);
            }

            while (textArchitect.isBuilding)
            {
                if (userNext)
                {
                    if (textArchitect.isBuilding)
                    {
                        textArchitect.ForceComplete();
                    }

                    userNext = false;
                }

                yield return null;
            }
        }

        public IEnumerator WaitForUserInput()
        {
            while (!userNext)
            {
                yield return null;
            }

            userNext = false;
        }
    }
}