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
            database.AddCommand("setEventParameter", new Action<string[]>(SetEventParameter));
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
            string fullPath = GetFullPath(filename);

            if (activeEvents.ContainsKey(fullPath))
            {
                EventInstance eventToStop = activeEvents[fullPath];
                eventToStop.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                eventToStop.release();
                activeEvents.Remove(fullPath);
                //Debug.Log("Stopped Event: " + fullPath);
            }
            else
            {
                Debug.LogWarning("Event not found: " + filename);
            }
        }

        private static void pauseEvent(string filename)
        {
            string fullPath = GetFullPath(filename);

            if (activeEvents.ContainsKey(fullPath))
            {
                EventInstance eventToPause = activeEvents[fullPath];
                CoroutineRunner.Instance.StartCoroutine(FadeOutAndPause(eventToPause, 2.5f)); // Fade out over 2.5 seconds
                Debug.Log("Pausing Event with Fade Out: " + fullPath);
            }
            else
            {
                Debug.LogWarning("Event not found: " + filename);
            }
        }

        private static void resumeEvent(string filename)
        {
            string fullPath = GetFullPath(filename);

            if (activeEvents.ContainsKey(fullPath))
            {
                EventInstance eventToResume = activeEvents[fullPath];
                CoroutineRunner.Instance.StartCoroutine(FadeInAndResume(eventToResume, 2.5f)); // Fade in over 2.5 seconds
                Debug.Log("Resuming Event with Fade In: " + fullPath);
            }
            else
            {
                Debug.LogWarning("Event not found: " + filename);
            }
        }

        // Fade out the event and then pause it
        private static IEnumerator FadeOutAndPause(EventInstance eventInstance, float fadeDuration)
        {
            float currentVolume = 1.0f;
            float fadeSpeed = 1.0f / fadeDuration;

            while (currentVolume > 0)
            {
                currentVolume = Mathf.Max(0, currentVolume - fadeSpeed * Time.deltaTime);
                eventInstance.setVolume(currentVolume);
                yield return null;
            }

            // Pause the event after fading out
            eventInstance.setPaused(true);
            Debug.Log("Event paused after fade out.");
        }

        // Fade in the event and then resume it
        private static IEnumerator FadeInAndResume(EventInstance eventInstance, float fadeDuration)
        {
            // Unpause the event before fading in
            eventInstance.setPaused(false);

            float currentVolume = 0.0f;
            float fadeSpeed = 1.0f / fadeDuration;

            while (currentVolume < 1.0f)
            {
                currentVolume = Mathf.Min(1.0f, currentVolume + fadeSpeed * Time.deltaTime);
                eventInstance.setVolume(currentVolume);
                yield return null;
            }

            Debug.Log("Event resumed with fade in.");
        }

        private static string GetFullPath(string filename)
        {
            if (activeEvents.ContainsKey(MUSIC_FILEPATH + filename)) return MUSIC_FILEPATH + filename;
            if (activeEvents.ContainsKey(AMB_FILEPATH + filename)) return AMB_FILEPATH + filename;
            return MUSIC_FILEPATH + filename; // Default to Music if unknown
        }

        private static void SetEventParameter(string[] data)
        {
            string filename = data[0];
            string parameterName = data[1];

            if (int.TryParse(data[2], out int value))
            {
                string fullPath = GetFullPath(filename);

                if (activeEvents.TryGetValue(fullPath, out EventInstance eventInstance) && eventInstance.isValid())
                {
                    // Validate discrete values 
                    int[] validValues = { 0, 1, 2, 3, 4, 5 };
                    if (!Array.Exists(validValues, v => v == value))
                    {
                        Debug.LogWarning($"Invalid value {value} for discrete parameter '{parameterName}'. Allowed: {string.Join(", ", validValues)}");
                        return;
                    }

                    eventInstance.setParameterByName(parameterName, value);
                    Debug.Log($"Set discrete parameter '{parameterName}' to {value} for {fullPath}");
                }
                else
                {
                    Debug.LogWarning($"Event not found or invalid: {filename}");
                }
            }
        }

        // Singleton coroutine runner to handle coroutines in a non-MonoBehaviour class
        public class CoroutineRunner : MonoBehaviour
        {
            private static CoroutineRunner _instance;

            public static CoroutineRunner Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("CoroutineRunner");
                        _instance = obj.AddComponent<CoroutineRunner>();
                        DontDestroyOnLoad(obj);
                    }
                    return _instance;
                }
            }
        }
    }
}

