using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commands;
using Characters;
using Dialogue.LogicalLines;

namespace Dialogue
{
    public class ConversationManager
    {
        private DialogueSystem dialogueSystem => DialogueSystem.Instance;
        private Coroutine process = null;

        public bool isRunning => process != null;

        private TextArchitect architect = null;
        private bool userNext = false;
        private float completionTime = 0.3f;

        private LogicalLineManager logicalLineManager;

        private ConversationQueue conversationQueue;
        public Conversation conversation => (conversationQueue.IsEmpty() ? null : conversationQueue.top);
        public int conversationProgress => (conversationQueue.IsEmpty() ? -1 : conversationQueue.top.GetProgress());

        public ConversationManager(TextArchitect textArchitect)
        {
            this.architect = textArchitect;
            dialogueSystem.onUserNext += OnUserNext;

            logicalLineManager = new LogicalLineManager();

            conversationQueue = new ConversationQueue();
        }

        public void Enqueue(Conversation conversation) => conversationQueue.Enqueue(conversation);

        public void EnqueuePriority(Conversation conversation) => conversationQueue.EnqueuePriority(conversation);

        private void OnUserNext()
        {
            if (SceneManager.Instance.inVNMode && !InputPanel.Instance.isWaitingForUserInput && !ChoicePanel.Instance.isWaitingForUserChoice)
            {
                userNext = true;
            }
        }

        public Coroutine StartConversation(Conversation conversation)
        {
            StopConversation();
            conversationQueue.Clear();

            Enqueue(conversation);

            process = dialogueSystem.StartCoroutine(RunningConversation());

            return process;
        }

        public void StopConversation()
        {
            if (!isRunning)
            {
                return;
            }

            dialogueSystem.StopCoroutine(process);
            process = null;
        }

        IEnumerator RunningConversation()
        {
            while(!conversationQueue.IsEmpty())
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

                if(logicalLineManager.TryGetLogicalLine(line, out Coroutine logic))
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

                    if (line.hasDialogue)
                    {
                        yield return WaitForUserInput();
                    }
                }

                TryAdvanceConversation(currentConversation);
            }

            process = null;
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
            if (line.hasSpeaker)
            {
                HandleSpeakerLogic(line.speakerData);
            }

            dialogueSystem.Show();

            yield return BuildLineSegments(line.dialogueData);
        }

        private void HandleSpeakerLogic(SpeakerData speakerData)
        {
            if (speakerData.mainCharacterThought)
            {
                dialogueSystem.HideSpeakerName();

                return;
            }

            Character character = CharacterManager.Instance.GetCharacter(speakerData.name); //creates the character as soon as character speaks

            if (speakerData.isCastingPosition && !character.isCharacterVisible)
            {
                character.SetPosition(speakerData.castPosition);
            }

            if (speakerData.makeCharacterEnter && !character.isCharacterVisible && !character.isCharacterShowing)
            {
                character.Show();
            }

            dialogueSystem.ShowSpeakerName(TagManager.Inject(speakerData.displayName));

            DialogueSystem.Instance.ApplySpeakerDataToDialogueContainer(speakerData.name);

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

            foreach(CommandData.Command command in commands)
            {
                if (command.waitForCompletion || command.name == "Wait")
                {
                    yield return CommandManager.Instance.Execute(command.name, command.arguments);
                    yield return new WaitForSeconds(completionTime);
                }
                else if (command.waitForUserInput)
                {
                    yield return CommandManager.Instance.Execute(command.name, command.arguments);
                    yield return WaitForUserInput();
                }
                else
                {
                    CommandManager.Instance.Execute(command.name, command.arguments);
                }
            }

            yield return null;
        }

        IEnumerator BuildLineSegments(DialogueData line)
        {
            for(int i = 0; i < line.lineSegments.Count; i++)
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
                architect.Build(dialogue);
            }
            else
            {
                architect.Append(dialogue);
            }

            while (architect.isBuilding)
            {
                if (userNext)
                {
                    if (architect.isBuilding)
                    {
                        architect.ForceComplete();
                    }

                    userNext = false;
                }

                yield return null;
            }
        }

        IEnumerator WaitForUserInput()
        {
            while(!userNext)
            {
                yield return null;
            }

            userNext = false;
        }
    }
}

