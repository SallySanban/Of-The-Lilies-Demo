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
        private static List<EventInstance> activeEvents = new List<EventInstance>();
        private static int eventCounter = 0;
        private const string MUSIC_FILEPATH = "event:/Music/";
        private const string SFX_FILEPATH = "event:/SFX/";
        private const string AMB_FILEPATH = "event:/Ambience/";
        

        new public static void Extend(CommandDatabase database)
        {
            
            database.AddCommand("playMusic", new Action<string>(playMusic));
            database.AddCommand("playSFX", new Action<string>(playSFX));
            database.AddCommand("playAmbience", new Action<string>(playAmbience));
            database.AddCommand("stopEvent", new Action<string>(stopEvent));
        }

        private static void playMusic(string filename)
        {
            eventCounter++;
            FMOD.Studio.EventInstance music;
            music = FMODUnity.RuntimeManager.CreateInstance(MUSIC_FILEPATH + filename);
            music.start();
            Debug.Log("Playing Event " + eventCounter + ": " + MUSIC_FILEPATH + filename);
            activeEvents.Add(music);
        }
        private static void playSFX(string filename)
        {
            FMODUnity.RuntimeManager.PlayOneShot(SFX_FILEPATH + filename);
        }

        private static void playAmbience(string filename)
        {
            eventCounter++;
            FMOD.Studio.EventInstance amb;
            amb = FMODUnity.RuntimeManager.CreateInstance(AMB_FILEPATH + filename);
            amb.start();
            Debug.Log("Playing Event " + eventCounter + ": " + AMB_FILEPATH + filename);
            activeEvents.Add(amb);
        }

        private static void stopEvent(string data)
        {
            if (int.TryParse(data, out int eventID))
            {
                if (eventID <= 0 || eventID > activeEvents.Count)
                {
                    Debug.Log($"Stopping Event with ID: {eventID}. Total active events: {activeEvents.Count}");
                    return;
                }

                else
                {
                    FMOD.Studio.EventInstance eventToStop = activeEvents[eventID - 1];
                    Debug.Log("Stopping Event ID " + eventID + " (Index: " + (eventID - 1) + ")");
                    eventToStop.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    eventToStop.release();
                    Debug.Log("Event " + eventID + " stopped successfully.");
                }
            }
            //else
            //{
            //    FMOD.Studio.EventInstance eventToStop = activeEvents[eventID - 1];
            //    Debug.Log("Stopping Event ID " + eventID + " (Index: " + (eventID - 1) + ")");
            //    eventToStop.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            //    eventToStop.release();
            //    activeEvents.RemoveAt(eventID - 1);
            //    Debug.Log("Event " + eventID + " stopped successfully.");
            //}
        }
    }
}

