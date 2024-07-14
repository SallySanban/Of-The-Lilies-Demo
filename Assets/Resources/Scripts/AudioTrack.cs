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

        private AudioChannel channel;
        private AudioSource source;

        public bool loop => source.loop;

        public float volumeCap { get; private set; }

        public bool isPlaying => source.isPlaying;

        public AudioTrack(AudioClip clip, bool loop, float startingVolume, float volumeCap, AudioChannel channel, AudioMixerGroup mixer)
        {
            trackName = clip.name;
            this.channel = channel;
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
            gameObj.transform.SetParent(channel.trackContainer);

            AudioSource source = gameObj.AddComponent<AudioSource>();

            return source;
        }

        public void Play()
        {
            source.Play();
        }

        public void Stop()
        {
            source.Stop();
        }
    }
}

