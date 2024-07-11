using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commands;
using Characters;

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

        public ConversationManager(TextArchitect textArchitect)
        {
            this.architect = textArchitect;
            dialogueSystem.onUserNext += OnUserNext;
        }

        private void OnUserNext()
        {
            userNext = true;
        }

        public Coroutine StartConversation(List<string> conversation)
        {
            StopConversation();

            process = dialogueSystem.StartCoroutine(RunningConversation(conversation));

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

        IEnumerator RunningConversation(List<string> conversation)
        {
            for (int i = 0; i < conversation.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(conversation[i]))
                {
                    continue;
                }

                DialogueLine line = DialogueParser.Parse(conversation[i]);

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
        }

        IEnumerator RunDialogue(DialogueLine line)
        {
            if (line.hasSpeaker)
            {
                HandleSpeakerLogic(line.speakerData);
            }

            yield return dialogueSystem.Show();

            yield return BuildLineSegments(line.dialogueData);
        }

        private void HandleSpeakerLogic(SpeakerData speakerData)
        {
            Character character = CharacterManager.Instance.GetCharacter(speakerData.name); //creates the character as soon as character speaks

            if (speakerData.isCastingPosition && !character.isCharacterVisible)
            {
                character.SetPosition(speakerData.castPosition);
            }

            if (speakerData.makeCharacterEnter && !character.isCharacterVisible && !character.isCharacterShowing)
            {
                character.Show();
            }

            dialogueSystem.ShowSpeakerName(speakerData.displayName);

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
                if (command.waitForCompletion)
                {
                    yield return CommandManager.Instance.Execute(command.name, command.arguments);
                    yield return new WaitForSeconds(completionTime);
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

