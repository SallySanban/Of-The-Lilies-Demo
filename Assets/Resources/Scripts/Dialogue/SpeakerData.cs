using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Dialogue
{
    public class SpeakerData
    {
        public string rawData { get; private set; } = string.Empty;

        public string name, castName;

        public string displayName => (isCastingName ? castName : name);

        public DialogueContainer.ContainerType textboxPosition;
        public Vector2 castPosition;
        public List<(int layer, string expression)> castExpressions { get; set; }

        public bool isCastingName => castName != string.Empty;
        public bool isCastingPosition = false;
        public bool isCastingExpressions => castExpressions.Count > 0;

        public bool makeCharacterEnter = false;
        public bool talkInSpeechBubble = false;

        private const string nameCastId = " as ";
        private const string positionCastId = " at ";
        private const string expressionCastId = " [";
        private const char axisDelimiter = ':';
        private const char expressionDelimiter = ',';
        private const char expressionLayerDelimiter = ':';
        private const string enterKeyword = "enter ";
        private const string pixelKeyword = "pixel ";

        private string ProcessKeywords(string rawSpeaker)
        {
            if (rawSpeaker.StartsWith(enterKeyword))
            {
                rawSpeaker = rawSpeaker.Substring(enterKeyword.Length);

                makeCharacterEnter = true;
            }
            else if (rawSpeaker.StartsWith(pixelKeyword))
            {
                rawSpeaker = rawSpeaker.Substring(pixelKeyword.Length);

                talkInSpeechBubble = true;
            }

            return rawSpeaker;
        }

        public SpeakerData(string rawSpeaker)
        {
            rawData = rawSpeaker;
            rawSpeaker = ProcessKeywords(rawSpeaker);

            string pattern = @$"{nameCastId}|{positionCastId}|{expressionCastId.Insert(expressionCastId.Length - 1, @"\")}";

            MatchCollection matches = Regex.Matches(rawSpeaker, pattern);

            castName = "";
            castPosition = Vector2.zero;
            textboxPosition = DialogueContainer.ContainerType.Textbox;
            castExpressions = new List<(int layer, string expression)>();

            if (matches.Count == 0)
            {
                name = rawSpeaker;

                return;
            }

            int index = matches[0].Index;

            name = rawSpeaker.Substring(0, index);

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                int startIndex = 0;
                int endIndex = 0;

                if (match.Value == nameCastId)
                {
                    startIndex = match.Index + nameCastId.Length;
                    endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    castName = rawSpeaker.Substring(startIndex, endIndex - startIndex);
                }
                else if (match.Value == positionCastId)
                {
                    isCastingPosition = true;

                    startIndex = match.Index + positionCastId.Length;
                    endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    string castPos = rawSpeaker.Substring(startIndex, endIndex - startIndex);

                    string[] axis = castPos.Split(axisDelimiter, System.StringSplitOptions.RemoveEmptyEntries);

                    float.TryParse(axis[0], out castPosition.x);

                    if (axis.Length > 1)
                    {
                        float.TryParse(axis[1], out castPosition.y);
                    }

                    textboxPosition = DialogueManager.Instance.GetTextboxTypeFromPosition(castPosition.x);
                }
                else if (match.Value == expressionCastId)
                {
                    startIndex = match.Index + expressionCastId.Length;
                    endIndex = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    string castExp = rawSpeaker.Substring(startIndex, endIndex - (startIndex + 1));

                    castExpressions = castExp.Split(expressionDelimiter).Select(x =>
                    {
                        var parts = x.Trim().Split(expressionLayerDelimiter);

                        if (parts.Length == 2)
                        {
                            return (int.Parse(parts[0]), parts[1]);
                        }
                        else
                        {
                            return (1, parts[0]);
                        }

                    }).ToList();
                }
            }
        }
    }
}