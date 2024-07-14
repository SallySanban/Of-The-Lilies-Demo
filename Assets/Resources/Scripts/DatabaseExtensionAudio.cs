using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;
using System;

namespace Commands
{
    public class DatabaseExtensionAudio : CommandDatabaseExtension
    {
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("PlaySoundEffect", new Action<string>(PlaySoundEffect));
        }

        private static void PlaySoundEffect(string audioFilename)
        {
            AudioManager.Instance.PlaySoundEffect(audioFilename);
        }
    }
}

