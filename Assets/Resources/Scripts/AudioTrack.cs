using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioTrack
    {
        private const string trackNameFormat = "Track - [{0}]";
        public string trackName { get; private set; }

        public AudioSource source;

        public bool loop => source.loop;

        public float volumeCap { get; private set; }

        public bool isPlaying => source.isPlaying;

        protected Coroutine fadingMusicCoroutine;
        public bool isFadingMusic => fadingMusicCoroutine != null;

        public AudioTrack(AudioClip clip, bool loop, float startingVolume, float volumeCap, AudioMixerGroup mixer)
        {
            trackName = clip.name;
            this.volumeCap = volumeCap;

            source = CreateSource();
            source.clip = clip;
            source.loop = loop;
            source.volume = startingVolume;

            source.outputAudioMixerGroup = mixer;
        }

        private AudioSource CreateSource()
        {
            GameObject gameObj = new GameObject(string.Format(trackNameFormat, trackName));
            gameObj.transform.SetParent(AudioManager.Instance.musicRoot);

            AudioSource source = gameObj.AddComponent<AudioSource>();

            return source;
        }

        public Coroutine Play()
        {
            if (isFadingMusic) return fadingMusicCoroutine;

            source.volume = 0f;

            source.Play();

            fadingMusicCoroutine = AudioManager.Instance.StartCoroutine(FadeMusic(true));

            return fadingMusicCoroutine;
        }

        public Coroutine Stop()
        {
            if (isFadingMusic) return fadingMusicCoroutine;

            fadingMusicCoroutine = AudioManager.Instance.StartCoroutine(FadeMusic(false));

            return fadingMusicCoroutine;
        }

        private IEnumerator FadeMusic(bool fadeIn)
        {
            float finalVolume = fadeIn ? volumeCap : 0;
            float time = 0;
            float duration = fadeIn ? 3f : 20f;

            while (time < duration)
            {
                source.volume = Mathf.Lerp(source.volume, finalVolume, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            source.volume = finalVolume;

            if (!fadeIn)
            {
                Object.Destroy(source.gameObject);
            }

            fadingMusicCoroutine = null;
        }
    }
}

