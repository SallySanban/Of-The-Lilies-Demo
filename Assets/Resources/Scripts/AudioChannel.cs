using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AudioChannel
    {
        private const string trackContainerNameFormat = "Channel - [{0}]";
        public int channelIndex { get; private set; }

        public Transform trackContainer { get; private set; } = null;

        private List<AudioTrack> tracks = new List<AudioTrack>();

        public AudioChannel(int channel)
        {
            channelIndex = channel;

            trackContainer = new GameObject(string.Format(trackContainerNameFormat, channel)).transform;
            trackContainer.SetParent(AudioManager.Instance.transform);
        }

        public AudioTrack PlayTrack(AudioClip clip, bool loop, float startingVolume, float volumeCap)
        {
            if(TryGetTrack(clip.name, out AudioTrack existingTrack))
            {
                if(!existingTrack.isPlaying)
                {
                    existingTrack.Play();
                }

                return existingTrack;
            }

            AudioTrack track = new AudioTrack(clip, loop, startingVolume, volumeCap, this, AudioManager.Instance.musicMixer);
            track.Play();

            return track;
        }

        public bool TryGetTrack(string trackName, out AudioTrack value)
        {
            trackName = trackName.ToLower();

            foreach(var track in tracks)
            {
                if(track.trackName.ToLower() == trackName)
                {
                    value = track;
                    return true;
                }
            }

            value = null;
            return false;
        }
    }
}

