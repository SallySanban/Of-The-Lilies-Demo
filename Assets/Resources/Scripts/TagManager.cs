using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

public class TagManager
{
    public static Dictionary<string, Func<string>> tags = new Dictionary<string, Func<string>>();
    private readonly Regex tagRegex = new Regex("<\\w+>");

    public TagManager()
    {
        InitializeTags();
    }

    private void InitializeTags()
    {
        tags["<playerName>"] = () => InputPanel.Instance.lastInput; //Ahlai
        tags["<subjectPronoun>"] = () => InputPanel.Instance.subjectPronoun; //they
        tags["<objectPronoun>"] = () => InputPanel.Instance.objectPronoun; //them
        tags["<possessivePronoun>"] = () => InputPanel.Instance.possessivePronoun; //their
    }

    public string PutTagsIn(string text)
    {
        if (tagRegex.IsMatch(text))
        {
            foreach(Match match in tagRegex.Matches(text))
            {
                if (tags.TryGetValue(match.Value, out var tagValueRequest))
                {
                    text = text.Replace(match.Value, tagValueRequest());
                }
            }
        }

        return text;
    }
}
