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

            Conversation currentConversation = DialogueSystem.Instance.conversationManager.conversation;
            int currentProgress = DialogueSystem.Instance.conversationManager.conversationProgress;

            EncapsulatedData ifData = RipEncapsulationData(currentConversation, currentProgress, false, parentStartingIndex: currentConversation.fileStartIndex);
            EncapsulatedData elseData = new EncapsulatedData();

            if(ifData.endingIndex + 1 < currentConversation.countLines)
            {
                string nextLine = currentConversation.GetLines()[ifData.endingIndex + 1].Trim();

                if(nextLine == elseKeyword)
                {
                    elseData = RipEncapsulationData(currentConversation, ifData.endingIndex + 1, false, parentStartingIndex: currentConversation.fileStartIndex);
                    ifData.endingIndex = elseData.endingIndex;
                }
            }

            currentConversation.SetProgress(ifData.endingIndex);
            EncapsulatedData selectedData = conditionResult ? ifData : elseData;

            if(!selectedData.isNull && selectedData.lines.Count > 0)
            {
                Conversation newConversation = new Conversation(selectedData.lines, file: currentConversation.file, fileStartIndex: currentConversation.fileStartIndex, fileEndIndex: currentConversation.fileEndIndex);
                DialogueSystem.Instance.conversationManager.conversation.SetProgress(selectedData.endingIndex);
                DialogueSystem.Instance.conversationManager.EnqueuePriority(newConversation);
            }

            yield return null;
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

