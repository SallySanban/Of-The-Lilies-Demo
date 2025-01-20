using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Dialogue.LogicalLines.LogicalLineUtils.Encapsulation;
using static Dialogue.LogicalLines.LogicalLineUtils.Conditions;

namespace Dialogue.LogicalLines
{
    public class LogicalLineCondition : ILogicalLine
    {
        public string keyword => "if";
        private const string elseKeyword = "else";
        private readonly string[] containers = new string[] { "(", ")" };

        public IEnumerator Execute(DialogueLine line)
        {
            string rawCondition = ExtractCondition(line.rawData.Trim());
            bool conditionResult = EvaluateCondition(rawCondition);

            // Debugging: Log the condition and its result
            Debug.Log($"Condition: {rawCondition}, Result: {conditionResult}");

            Conversation currentConversation = DialogueManager.Instance.conversationManager.conversation;
            int currentProgress = DialogueManager.Instance.conversationManager.conversationProgress;

            EncapsulatedData ifData = RipEncapsulationData(currentConversation, currentProgress, true, parentStartingIndex: currentConversation.fileStartIndex);
            EncapsulatedData elseData = new EncapsulatedData();

            if(ifData.endingIndex + 1 < currentConversation.countLines)
            {
                string nextLine = currentConversation.GetLines()[ifData.endingIndex + 1].Trim();

                if(nextLine == elseKeyword)
                {
                    elseData = RipEncapsulationData(currentConversation, ifData.endingIndex + 1, true, parentStartingIndex: currentConversation.fileStartIndex);
                    ifData.endingIndex = elseData.endingIndex;
                }
            }

            currentConversation.SetProgress(ifData.endingIndex + 1);
            EncapsulatedData selectedData = conditionResult ? ifData : elseData;

            if(!selectedData.isNull && selectedData.lines.Count > 0)
            {
                Conversation newConversation = new Conversation(selectedData.lines, file: currentConversation.file, fileStartIndex: currentConversation.fileStartIndex, fileEndIndex: currentConversation.fileEndIndex);
                DialogueManager.Instance.conversationManager.EnqueuePriority(newConversation);
            }

            yield return null;
        }

        private EncapsulatedData RipEncapsulationData(Conversation conversation, int startIndex, bool includeNested = true, int parentStartingIndex = 0)
        {
            List<string> lines = conversation.GetLines();
            List<string> encapsulatedLines = new List<string>();
            int bracketCount = 0;
            int currentIndex = startIndex;

            currentIndex++;

            while (currentIndex < lines.Count)
            {
                string line = lines[currentIndex].Trim();

                if (line.EndsWith("{"))
                {
                    bracketCount++;
                }
                else if (line.EndsWith("}"))
                {
                    bracketCount--;
                    
                    if (bracketCount == 0)
                    {
                        break;
                    }
                }
                
                if (includeNested || bracketCount <= 1)
                {
                    encapsulatedLines.Add(lines[currentIndex]);
                }

                currentIndex++;
            }

            return new EncapsulatedData()
            {
                lines = encapsulatedLines,
                startingIndex = startIndex,
                endingIndex = currentIndex,
                fileStartIndex = parentStartingIndex
            };
        }

        public bool Matches(DialogueLine line)
        {
            return line.rawData.Trim().StartsWith(keyword);
        }

        private string ExtractCondition(string line)
        {
            int startIndex = line.IndexOf(containers[0]) + 1;
            int endIndex = line.IndexOf(containers[1]);

            return line.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}

