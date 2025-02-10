using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;

namespace Commands
{
    public class DatabaseExtensionAudio : CommandDatabaseExtension
    {
        private static Dictionary<string, EventInstance> activeEvents = new Dictionary<string, EventInstance>();
        private const string MUSIC_FILEPATH = "event:/Music/";
        private const string SFX_FILEPATH = "event:/SFX/";
        private const string AMB_FILEPATH = "event:/Ambience/";


        new public static void Extend(CommandDatabase database)
        {

            database.AddCommand("playSFX", new Action<string>(playSFX));
            database.AddCommand("playMusic", new Action<string>(playMusic));
            database.AddCommand("playAmbience", new Action<string>(playAmbience));
            database.AddCommand("stopEvent", new Action<string>(stopEvent));
            database.AddCommand("pauseEvent", new Action<string>(pauseEvent));
            database.AddCommand("resumeEvent", new Action<string>(resumeEvent));
        }

        private static void playSFX(string filename)
        {
            FMODUnity.RuntimeManager.PlayOneShot(SFX_FILEPATH + filename);
        }

        private static void playMusic(string filename)
        {
            string fullPath = MUSIC_FILEPATH + filename;
            if (!activeEvents.ContainsKey(fullPath))
            {
                EventInstance music = RuntimeManager.CreateInstance(fullPath);
                music.start();
                activeEvents.Add(fullPath, music);
                Debug.Log("Playing Music: " + fullPath);
            }
            else
            {
                Debug.LogWarning("Music is already playing: " + fullPath);
            }
        }

        private static void playAmbience(string filename)
        {
            string fullPath = AMB_FILEPATH + filename;
            if (!activeEvents.ContainsKey(fullPath))
            {
                EventInstance ambience = RuntimeManager.CreateInstance(fullPath);
                ambience.start();
                activeEvents.Add(fullPath, ambience);
                Debug.Log("Playing Ambience: " + fullPath);
            }
            else
            {
                Debug.LogWarning("Ambience is already playing: " + fullPath);
            }
        }

        private static void stopEvent(string filename)
        {
            string fullPath = MUSIC_FILEPATH + filename;
            if (!activeEvents.ContainsKey(fullPath))
            {
                fullPath = AMB_FILEPATH + filename; // Check if it's an ambience event
            }

            if (activeEvents.ContainsKey(fullPath))
            {
                EventInstance eventToStop = activeEvents[fullPath];
                eventToStop.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                eventToStop.release();
                activeEvents.Remove(fullPath);
                Debug.Log("Stopped Event: " + fullPath);
            }
            else
            {
                Debug.LogWarning("Event not found: " + filename);
            }
        }

        private static void pauseEvent(string filename)
        {
            string fullPath = MUSIC_FILEPATH + filename;
            if (!activeEvents.ContainsKey(fullPath))
            {
                fullPath = AMB_FILEPATH + filename; // Check if it's an ambience event
            }

            if (activeEvents.ContainsKey(fullPath))
            {
                EventInstance eventToPause = activeEvents[fullPath];
                eventToPause.setPaused(true);
                Debug.Log("Paused Event: " + fullPath);
            }
            else
            {
                Debug.LogWarning("Event not found: " + filename);
            }
        }

        private static void resumeEvent(string filename)
        {
            string fullPath = MUSIC_FILEPATH + filename;
            if (!activeEvents.ContainsKey(fullPath))
            {
                fullPath = AMB_FILEPATH + filename; // Check if it's an ambience event
            }

            if (activeEvents.ContainsKey(fullPath))
            {
                EventInstance eventToResume = activeEvents[fullPath];
                eventToResume.setPaused(false);
                Debug.Log("Resumed Event: " + fullPath);
            }
            else
            {
                Debug.LogWarning("Event not found: " + filename);
            }
        }
    }
}

