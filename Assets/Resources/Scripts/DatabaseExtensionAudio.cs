using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;
using System;

namespace Commands
{
    public class DatabaseExtensionAudio : CommandDatabaseExtension
    {
        private static string loopParameter => "-l";
        private static string volumeParameter => "-vol";

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("PlaySoundEffect", new Action<string[]>(PlaySoundEffect));
            database.AddCommand("StopSoundEffect", new Action<string>(StopSoundEffect));
            database.AddCommand("PlayMusic", new Action<string[]>(PlayMusic));
            database.AddCommand("StopMusic", new Action<string>(StopMusic));
        }

        private static void PlaySoundEffect(string[] data)
        {
            string audioFilename = data[0];

            var parameters = ConvertDataToParameters(data);

            float volume = 1;
            bool loop = false;

            parameters.TryGetValue(volumeParameter, out volume, defaultValue: 1f);
            parameters.TryGetValue(loopParameter, out loop, defaultValue: false);


            AudioManager.Instance.PlaySoundEffect(audioFilename, volume: volume, loop: loop);
        }

        private static void StopSoundEffect(string audioFilename)
        {
            AudioManager.Instance.StopSoundEffect(audioFilename);
        }

        private static void PlayMusic(string[] data)
        {
            string audioFilename = data[0];

            var parameters = ConvertDataToParameters(data);

            float volume = 1;

            parameters.TryGetValue(volumeParameter, out volume, defaultValue: 1f);

            AudioManager.Instance.PlayTrack(audioFilename, volumeCap: volume);

            if(audioFilename != "Fire")
            {
                AudioManager.Instance.musicIsPlaying = audioFilename;
            }
        }

        private static void StopMusic(string audioFilename)
        {
            AudioManager.Instance.StopTrack(audioFilename);

            if (audioFilename != "Fire")
            {
                AudioManager.Instance.musicIsPlaying = "";
            }
        }
    }
}

