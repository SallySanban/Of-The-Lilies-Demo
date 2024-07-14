using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        private const string sfxParentName = "SFX";
        private const string sfxNameFormat = "SFX - [{0}]";
        private const string audioFilenameId = "<audioName>";

        public const float trackTransitionSpeed = 1;

        private string sfxPath => $"Audio/Sound Effects/{audioFilenameId}";
        private string musicPath => $"Audio/Music/{audioFilenameId}";

        public static AudioManager Instance { get; private set; }

        public Dictionary<int, AudioChannel> channels = new Dictionary <int, AudioChannel>();

        public AudioMixerGroup musicMixer;
        public AudioMixerGroup sfxMixer;
        public AudioMixerGroup voicesMixer;

        private Transform sfxRoot;

        private void Awake()
        {
            if (Instance == null)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);

                Instance = this;
            }
            else
            {
                DestroyImmediate(gameObject);
                return;
            }

            sfxRoot = new GameObject(sfxParentName).transform;
            sfxRoot.SetParent(transform);
        }

        public AudioSource PlaySoundEffect(string audioFilename, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false)
        {
            string audioPath = FormatAudioPath(sfxPath, audioFilename);

            AudioClip clip = Resources.Load<AudioClip>(audioPath);

            if (clip == null)
            {
                Debug.LogError($"Could not load audio file {audioPath}");
                return null;
            }

            return PlaySoundEffect(clip, mixer, volume, pitch, loop);
        }

        public AudioSource PlaySoundEffect(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false)
        {
            AudioSource effectSource = new GameObject(string.Format(sfxNameFormat, clip.name)).AddComponent<AudioSource>();
            effectSource.transform.SetParent(sfxRoot);
            effectSource.transform.position = sfxRoot.position;

            effectSource.clip = clip;

            if(mixer == null)
            {
                mixer = sfxMixer;
            }

            effectSource.outputAudioMixerGroup = mixer;
            effectSource.volume = volume;
            effectSource.spatialBlend = 0;
            effectSource.pitch = pitch;
            effectSource.loop = loop;

            effectSource.Play();

            if (!loop)
            {
                Destroy(effectSource.gameObject, (clip.length / pitch) + 1);
            }

            return effectSource;
        }

        public void StopSoundEffect(AudioClip clip) => StopSoundEffect(clip.name);

        public void StopSoundEffect(string audioFilename)
        {
            audioFilename = audioFilename.ToLower();

            AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();

            foreach(var source in sources)
            {
                if(source.clip.name.ToLower() == audioFilename)
                {
                    Destroy(source.gameObject);
                    return;
                }
            }
        }

        public AudioTrack PlayTrack(string audioFilename, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1)
        {
            string audioPath = FormatAudioPath(musicPath, audioFilename);

            AudioClip clip = Resources.Load<AudioClip>(audioPath);

            if (clip == null)
            {
                Debug.LogError($"Could not load audio file {audioPath}");
                return null;
            }

            return PlayTrack(clip, channel, loop, startingVolume, volumeCap);
        }

        public AudioTrack PlayTrack(AudioClip clip, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1)
        {
            AudioChannel audioChannel = TryGetChannel(channel);

            AudioTrack track = audioChannel.PlayTrack(clip, loop, startingVolume, volumeCap);

            return track;
        }

        public AudioChannel TryGetChannel(int channelNumber)
        {
            AudioChannel channel = null;

            if(channels.TryGetValue(channelNumber, out channel))
            {
                return channel;
            }
            else
            {
                return new AudioChannel(channelNumber);
            }
        }

        private string FormatAudioPath(string path, string audioFilename) => path.Replace(audioFilenameId, audioFilename);
    }
}

