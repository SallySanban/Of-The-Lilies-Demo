using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class DialogueLine
    {
        public string rawData { get; private set; } = string.Empty;

        public SpeakerData speakerData;
        public DialogueData dialogueData;
        public CommandData commandData;

        public bool hasSpeaker => speakerData != null;
        public bool hasDialogue => dialogueData != null;
        public bool hasCommands => commandData != null;

        public DialogueLine(string rawLine, string speaker, string dialogue, string commands)
        {
            rawData = rawLine;

            this.speakerData = (string.IsNullOrWhiteSpace(speaker)) ? null : new SpeakerData(speaker);
            this.dialogueData = (string.IsNullOrWhiteSpace(dialogue)) ? null : new DialogueData(dialogue);
            this.commandData = (string.IsNullOrWhiteSpace(commands)) ? null : new CommandData(commands);
        }
    }
}