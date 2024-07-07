using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class DialogueLine
    {
        public string speaker;
        public DialogueData dialogue;
        public string commands;

        public bool hasSpeaker => speaker != string.Empty;
        public bool hasDialogue => dialogue.hasDialogue;
        public bool hasCommands => commands != string.Empty;

        public DialogueLine(string speaker, string dialogue, string commands)
        {
            this.speaker = speaker;
            this.dialogue = new DialogueData(dialogue);
            this.commands = commands;
        }
    }
}