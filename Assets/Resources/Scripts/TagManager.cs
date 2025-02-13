using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Linq;

public class TagManager
{
    public static Dictionary<string, Func<string>> tags = new Dictionary<string, Func<string>>()
    {
        { "<playerName>", () => 
            {
                var name = UIManager.Instance.inputPanel.name;
                return string.IsNullOrEmpty(name) ? "Ahlai" : char.ToUpper(name[0]) + name.Substring(1).ToLower();
            }
        },
        { "<subjectPronoun>", () => string.IsNullOrEmpty(UIManager.Instance.inputPanel.subjectPronoun) ? "they" : UIManager.Instance.inputPanel.subjectPronoun },
        { "<objectPronoun>", () => string.IsNullOrEmpty(UIManager.Instance.inputPanel.objectPronoun) ? "them" : UIManager.Instance.inputPanel.objectPronoun },
        { "<possessivePronoun>", () => string.IsNullOrEmpty(UIManager.Instance.inputPanel.possessivePronoun) ? "their" : UIManager.Instance.inputPanel.possessivePronoun },
    };

    private static readonly Regex tagRegex = new Regex("<\\w+>");

    public static string Inject(string text, bool putTags = true, bool putVariables = true)
    {
        if (putTags)
        {
            text = PutTagsIn(text);
        }

        if (putVariables)
        {
            text = PutVariablesIn(text);
        }

        return text;
    }

    private static string PutTagsIn(string value)
    {
        if (tagRegex.IsMatch(value))
        {
            var sentenceEndings = new[] { '.', '!', '?' };
            foreach (Match match in tagRegex.Matches(value))
            {
                if (tags.TryGetValue(match.Value, out var tagValueRequest))
                {
                    var tagValue = tagValueRequest();
                    int tagIndex = match.Index;

                    bool capitalize = tagIndex == 0 || 
                                      (tagIndex > 0 && sentenceEndings.Contains(value[tagIndex - 2]));

                    if (capitalize)
                    {
                        tagValue = char.ToUpper(tagValue[0]) + tagValue.Substring(1);
                    }

                    value = value.Replace(match.Value, tagValue);
                }
            }
        }

        return value;
    }

    private static string PutVariablesIn(string value)
    {
        var matches = Regex.Matches(value, VariableStore.regexVariableIds);
        var matchesList = matches.Cast<Match>().ToList();

        for (int i = matchesList.Count - 1; i >= 0; i--)
        {
            var match = matchesList[i];

            string variableName = match.Value.TrimStart(VariableStore.variableId, '!');

            bool negate = match.Value.StartsWith('!');

            bool endsInIllegalCharacter = variableName.EndsWith(VariableStore.databaseVariableId);

            if (endsInIllegalCharacter)
            {
                variableName = variableName.Substring(0, variableName.Length - 1);
            }

            if (!VariableStore.TryGetValue(variableName, out object variableValue))
            {
                Debug.Log($"Variable {variableName} not found in string assignment");
                continue;
            }

            if (negate && variableValue is bool)
            {
                variableValue = !(bool)variableValue;
            }

            int lengthToBeRemoved = match.Index + match.Length > value.Length ? value.Length - match.Index : match.Length;

            if (endsInIllegalCharacter)
            {
                lengthToBeRemoved -= 1;
            }

            value = value.Remove(match.Index, lengthToBeRemoved);
            value = value.Insert(match.Index, variableValue.ToString());
        }

        return value;
    }
}
