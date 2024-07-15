using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Dialogue.LogicalLines
{
    public class LogicalLineChoice : ILogicalLine
    {
        private const char encapsulationStart = '{';
        private const char encapsulationEnd = '}';
        private const string choiceIdentifier = "- ";
        private const string choicePositionIdentifier = " in ";

        private ChoicePanel.ChoicePosition choicePosition;

        public string keyword => "choice";
        

        public IEnumerator Execute(DialogueLine line)
        {
            RawChoiceData data = RipChoiceData();

            List<Choice> choices = GetChoicesFromData(data);

            string title = line.hasDialogue ? line.dialogueData.rawData : "";
            ChoicePanel choicePanel = ChoicePanel.Instance;

            string[] choiceTexts = choices.Select(c => c.choiceText).ToArray();

            choicePanel.Show(choiceTexts, choicePosition, title: title);

            while (choicePanel.isWaitingForUserChoice)
            {
                yield return null;
            }

            Choice selectedChoice = choices[choicePanel.lastChoicePicked.answerIndex];

            Conversation newConversation = new Conversation(selectedChoice.executableLines);
            DialogueSystem.Instance.conversationManager.conversation.SetProgress(data.endingIndex);

            DialogueSystem.Instance.conversationManager.EnqueuePriority(newConversation);
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

                    if (position == -1)
                    {
                        choicePosition = ChoicePanel.ChoicePosition.Left;
                    }
                    else if (position == 0)
                    {
                        choicePosition = ChoicePanel.ChoicePosition.Center;
                    }
                    else if (position == 1)
                    {
                        choicePosition = ChoicePanel.ChoicePosition.Right;
                    }

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

        private RawChoiceData RipChoiceData()
        {
            Conversation currentConversation = DialogueSystem.Instance.conversationManager.conversation;
            int currentProgress = DialogueSystem.Instance.conversationManager.conversationProgress;

            int encapsulationDepth = 0;

            RawChoiceData data = new RawChoiceData {  lines = new List<string>(), endingIndex = 0 };

            for(int i = currentProgress; i < currentConversation.countLines; i++)
            {
                string line = currentConversation.GetLines()[i];
                data.lines.Add(line);

                if (IsEncapsulationStart(line))
                {
                    encapsulationDepth++;
                    continue;
                }

                if (IsEncapsulationEnd(line))
                {
                    encapsulationDepth--;

                    if(encapsulationDepth == 0)
                    {
                        data.endingIndex = i;
                        break;
                    }
                }
            }

            return data;
        }

        private List<Choice> GetChoicesFromData(RawChoiceData data)
        {
            List<Choice> choices = new List<Choice>();

            int encapsulationDepth = 0;
            bool isFirstChoice = true;

            Choice choice = new Choice
            {
                choiceText = string.Empty,
                executableLines = new List<string>()
            };

            foreach(var line in data.lines.Skip(1))
            {
                if(IsChoiceStart(line) && encapsulationDepth == 1)
                {
                    if(!isFirstChoice)
                    {
                        choices.Add(choice);
                        choice = new Choice
                        {
                            choiceText = string.Empty,
                            executableLines = new List<string>()
                        };
                    }

                    choice.choiceText = line.Trim().Substring(choiceIdentifier.Length);
                    isFirstChoice = false;
                    continue;
                }

                AddLineToResults(line, ref choice, ref encapsulationDepth);
            }

            if (!choices.Contains(choice))
            {
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

        private bool IsEncapsulationStart(string line) => line.Trim().StartsWith(encapsulationStart);

        private bool IsEncapsulationEnd(string line) => line.Trim().StartsWith(encapsulationEnd);

        private bool IsChoiceStart(string line) => line.Trim().StartsWith(choiceIdentifier);

        private struct RawChoiceData
        {
            public List<string> lines;
            public int endingIndex;
        }

        private struct Choice
        {
            public string choiceText;
            public List<string> executableLines;
        }
    }
}

