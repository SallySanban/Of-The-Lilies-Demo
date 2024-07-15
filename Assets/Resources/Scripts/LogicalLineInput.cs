using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class LogicalLineInput : ILogicalLine
    {
        public string keyword => "input";

        public IEnumerator Execute(DialogueLine line)
        {
            InputPanel inputPanel = InputPanel.Instance;

            string title = line.dialogueData.rawData;

            inputPanel.Show(title);

            while (inputPanel.isWaitingForUserInput)
            {
                yield return null;
            }
        }

        public bool Matches(DialogueLine line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
        }
    }

}
