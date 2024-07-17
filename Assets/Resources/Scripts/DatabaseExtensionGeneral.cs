using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Commands
{
    public class DatabaseExtensionGeneral : CommandDatabaseExtension
    {
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("SavePronoun", new Action<string>(SavePronoun));
            database.AddCommand("SwitchPixel", new Action(() => { SceneManager.Instance.SwitchToPixel(); }));
        }

        private static void SavePronoun(string data)
        {
            if (data == "he")
            {
                TagManager.tags["<subjectPronoun>"] = () => "he";
                TagManager.tags["<objectPronoun>"] = () => "him";
                TagManager.tags["<possessivePronoun>"] = () => "his";
            }
            else if (data == "she")
            {
                TagManager.tags["<subjectPronoun>"] = () => "she";
                TagManager.tags["<objectPronoun>"] = () => "her";
                TagManager.tags["<possessivePronoun>"] = () => "her";
            }
            else if (data == "they")
            {
                TagManager.tags["<subjectPronoun>"] = () => "they";
                TagManager.tags["<objectPronoun>"] = () => "them";
                TagManager.tags["<possessivePronoun>"] = () => "their";
            }
        }
    }
}

