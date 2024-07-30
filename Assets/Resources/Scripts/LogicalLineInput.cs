using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue.LogicalLines
{
    public class LogicalLineInput : ILogicalLine
    {
        public string keyword => "input";

        public IEnumerator Execute(DialogueLine line)
        {
            InputPanel inputPanel = InputPanel.Instance;

            inputPanel.Show();

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
