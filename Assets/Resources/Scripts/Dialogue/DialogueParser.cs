using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


namespace Dialogue
{
    public class DialogueParser
    {
        private const string commandRegexPattern = @"\b\w+\(";

        public static DialogueLine Parse(string rawLine)
        {
            (string speaker, string dialogue, string commands) = GetContent(rawLine);

            return new DialogueLine(rawLine, speaker, dialogue, commands);
        }

        private static (string, string, string) GetContent(string rawLine)
        {
            string speaker = "", dialogue = "", commands = "";

            int dialogueStart = -1;
            int dialogueEnd = -1;
            bool isEscaped = false;

            for(int i = 0; i < rawLine.Length; i++)
            {
                char current = rawLine[i];

                if(current == '\\')
                {
                    isEscaped = !isEscaped;
                }
                else if(current == '"' && !isEscaped)
                {
                    if(dialogueStart == -1)
                    {
                        dialogueStart = i;
                    }
                    else if(dialogueEnd == -1)
                    {
                        dialogueEnd = i;
                    }
                }
                else
                {
                    isEscaped = false;
                }
            }

            Regex commandRegex = new Regex(commandRegexPattern);
            Match match = commandRegex.Match(rawLine);

            int commandStart = -1;

            if(match.Success)
            {
                commandStart = match.Index;

                if(dialogueStart == -1 && dialogueEnd == -1)
                    return ("", "", rawLine.Trim());
            }

            if(dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd))
            {
                speaker = rawLine.Substring(0, dialogueStart).Trim();
                dialogue = rawLine.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1).Replace("\\\"", "\"");
                
                if(commandStart != -1)
                {
                    commands = rawLine.Substring(commandStart).Trim();
                }
            }
            else if(commandStart != -1 && dialogueStart > commandStart)
            {
                commands = rawLine;
            }
            else
            {
                speaker = rawLine;
            }

            return (speaker, dialogue, commands);
        }
    }
}
