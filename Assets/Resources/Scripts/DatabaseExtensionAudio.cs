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
            database.AddCommand("PlayMusic", new Action<string>(PlayMusic));
        }

        private static void PlaySoundEffect(string audioFilename)
        {
            AudioManager.Instance.PlaySoundEffect(audioFilename);
        }

        private static void PlayMusic(string audioFilename)
        {
            AudioManager.Instance.PlayTrack(audioFilename, startingVolume: 1);  //temporary starting volume while music doesnt fade in and out
        }
    }
}

