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
            InputPanel inputPanel = UIManager.Instance.CreateUI<InputPanel>();

            while (inputPanel.isWaitingForUserInput)
            {
                yield return null;
            }

            UIManager.Instance.DestroyUI<InputPanel>(inputPanel);
        }

        public bool Matches(DialogueLine line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
        }
    }

}
