using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Dialogue.LogicalLines.LogicalLineUtils.Encapsulation;
using static Dialogue.LogicalLines.LogicalLineUtils.Conditions;
using Characters;
using UnityEngine;

namespace Dialogue.LogicalLines
{
    public class LogicalLineChoice : ILogicalLine
    {
        private const string choiceIdentifier = "- ";
        private const string choicePositionIdentifier = " in ";
        private const string conditionStart = "if (";
        private const string conditionEnd = ")";

        private DialogueContainer.ContainerType choiceContainer;

        public string keyword => "choice";

        public IEnumerator Execute(DialogueLine line)
        {
            var currentConversation = DialogueManager.Instance.conversationManager.conversation;
            var progress = DialogueManager.Instance.conversationManager.conversationProgress;
            EncapsulatedData data = RipEncapsulationData(currentConversation, progress, true, parentStartingIndex: currentConversation.fileStartIndex);

            List<Choice> choices = GetChoicesFromData(data);

            string[] choiceTexts = choices.Select(c => c.choiceText).ToArray();

            if(choiceContainer == DialogueContainer.ContainerType.SpeechBubble)
            {
                yield return DialogueManager.Instance.ShowTextbox(choiceContainer, speakerName: TagManager.Inject("<playerName>"), speakerSprite: SceneManager.Instance.player?.root, listOfChoices: choiceTexts);
            }
            else
            {
                Character character = CharacterManager.Instance.GetCharacter(SceneManager.Instance.MC_NAME, false);

                //temp until the new tayabac ahlai portrait is here
                if(character == null)
                {
                    character = CharacterManager.Instance.GetCharacter("TayabacAhlai");
                }

                yield return DialogueManager.Instance.ShowTextbox(choiceContainer, speakerName: TagManager.Inject("<playerName>"), speakerSprite: character?.root.gameObject, listOfChoices: choiceTexts);
            }

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

                    keywordToFind = line.speakerData.name.Substring(0, match.Index).Trim();
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
                    string trimmedLine = line.Trim().Substring(choiceIdentifier.Length);
                    
                    if (trimmedLine.StartsWith(conditionStart))
                    {
                        int conditionEndIndex = trimmedLine.IndexOf(conditionEnd);
                        if (conditionEndIndex != -1)
                        {
                            string condition = trimmedLine.Substring(conditionStart.Length, 
                                conditionEndIndex - conditionStart.Length);
                            
                            choice.condition = condition;
                            choice.choiceText = TagManager.Inject(
                                trimmedLine.Substring(conditionEndIndex + 1).Trim());
                        }
                    }
                    else
                    {
                        choice.choiceText = TagManager.Inject(trimmedLine);
                    }
                    
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

            return choices.Where(c => string.IsNullOrEmpty(c.condition) || EvaluateCondition(c.condition)).ToList();
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
            public string condition;
            public List<string> executableLines;
            public int startIndex;
            public int endIndex;
        }
    }
}

