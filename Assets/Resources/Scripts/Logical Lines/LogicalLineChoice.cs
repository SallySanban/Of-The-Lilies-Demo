using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Dialogue.LogicalLines.LogicalLineUtils.Encapsulation;

namespace Dialogue.LogicalLines
{
    public class LogicalLineChoice : ILogicalLine
    {
        private const string choiceIdentifier = "- ";
        private const string choicePositionIdentifier = " in ";

        private DialogueContainer.ContainerType choiceContainer;

        public string keyword => "choice";

        public IEnumerator Execute(DialogueLine line)
        {
            var currentConversation = DialogueManager.Instance.conversationManager.conversation;
            var progress = DialogueManager.Instance.conversationManager.conversationProgress;
            EncapsulatedData data = RipEncapsulationData(currentConversation, progress, true, parentStartingIndex: currentConversation.fileStartIndex);

            List<Choice> choices = GetChoicesFromData(data);

            string[] choiceTexts = choices.Select(c => c.choiceText).ToArray();

            yield return DialogueManager.Instance.ShowTextbox(choiceContainer, speakerName: TagManager.Inject("<playerName>"), listOfChoices: choiceTexts);

            while (DialogueManager.Instance.currentTextbox.currentChoiceContainer.isWaitingForUserChoice)
            {
                yield return null;
            }

            Choice selectedChoice = choices[DialogueManager.Instance.currentTextbox.currentChoiceContainer.lastChoicePicked.answerIndex];

            Conversation newConversation = new Conversation(selectedChoice.executableLines, file: currentConversation.file, fileStartIndex: selectedChoice.startIndex, fileEndIndex: selectedChoice.endIndex);
            DialogueManager.Instance.conversationManager.conversation.SetProgress(data.endingIndex);

            DialogueManager.Instance.conversationManager.EnqueuePriority(newConversation);

            yield return null;
        }

        public bool Matches(DialogueLine line)
        {
            string keywordToFind;

            if (line.hasSpeaker)
            {
                Match match = Regex.Match(line.speakerData.name, choicePositionIdentifier);

                if (match.Success)
                {
                    int position = int.Parse(line.speakerData.name.Substring(match.Index + choicePositionIdentifier.Length));

                    choiceContainer = DialogueManager.Instance.GetTextboxTypeFromPosition(position);

                    keywordToFind = line.speakerData.name.Substring(0, match.Index);
                }
                else
                {
                    keywordToFind = line.speakerData.name;
                }

                return (keywordToFind.ToLower() == keyword);
            }
            else
            {
                return false;
            }
        }

        private List<Choice> GetChoicesFromData(EncapsulatedData data)
        {
            List<Choice> choices = new List<Choice>();

            int encapsulationDepth = 0;
            bool isFirstChoice = true;

            Choice choice = new Choice
            {
                choiceText = string.Empty,
                executableLines = new List<string>()
            };

            int choiceIndex = 0, i = 0;

            for(i = 1; i < data.lines.Count; i++)
            {
                var line = data.lines[i];

                if(IsChoiceStart(line) && encapsulationDepth == 1)
                {
                    if(!isFirstChoice)
                    {
                        choice.startIndex = data.startingIndex + (choiceIndex + 1);
                        choice.endIndex = data.startingIndex + (i - 1);
                        choices.Add(choice);
                        choice = new Choice
                        {
                            choiceText = string.Empty,
                            executableLines = new List<string>()
                        };
                    }

                    choiceIndex = i;
                    choice.choiceText = TagManager.Inject(line.Trim().Substring(choiceIdentifier.Length));
                    isFirstChoice = false;
                    continue;
                }

                AddLineToResults(line, ref choice, ref encapsulationDepth);
            }

            if (!choices.Contains(choice))
            {
                choice.startIndex = data.startingIndex + (choiceIndex + 1);
                choice.endIndex = data.startingIndex + (i - 2);
                choices.Add(choice);
            }

            return choices;
        }

        private void AddLineToResults(string line, ref Choice choice, ref int encapsulationDepth)
        {
            line.Trim();

            if (IsEncapsulationStart(line))
            {
                if(encapsulationDepth > 0)
                {
                    choice.executableLines.Add(line);
                }

                encapsulationDepth++;
                return;
            }

            if (IsEncapsulationEnd(line))
            {
                encapsulationDepth--;

                if(encapsulationDepth > 0)
                {
                    choice.executableLines.Add(line);
                }

                return;
            }

            choice.executableLines.Add(line);
        }

        private bool IsChoiceStart(string line) => line.Trim().StartsWith(choiceIdentifier);

        private struct Choice
        {
            public string choiceText;
            public List<string> executableLines;
            public int startIndex;
            public int endIndex;
        }
    }
}

