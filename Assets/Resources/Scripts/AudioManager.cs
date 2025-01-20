using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using FMODUnity;
using FMOD.Studio;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {

        public string VCAName;

        [Header("Volume")]
        [Range(0, 1)] public float masterVolume = 1.0f;


        private FMOD.Studio.VCA vca;

        //    private const string sfxParentName = "SFX";
        //    private const string musicParentName = "Music";
        //    private const string sfxNameFormat = "SFX - [{0}]";
        //    private const string audioFilenameId = "<audioName>";

        //    public const float trackTransitionSpeed = 1;

        //    private string sfxPath => $"Audio/Sound Effects/{audioFilenameId}";
        //    private string musicPath => $"Audio/Music/{audioFilenameId}";

        //    public static AudioManager Instance { get; private set; }

        //    private List<AudioTrack> tracks = new List<AudioTrack>();

        //    public AudioMixerGroup musicMixer;
        //    public AudioMixerGroup sfxMixer;
        //    public AudioMixerGroup voicesMixer;

        //    private Transform sfxRoot;
        //    public Transform musicRoot;

        //    public string musicIsPlaying = "";

        private void Start()
        {
            vca = FMODUnity.RuntimeManager.GetVCA("vca:/" + VCAName);


            //        sfxRoot = new GameObject(sfxParentName).transform;
            //        musicRoot = new GameObject(musicParentName).transform;

            //        sfxRoot.SetParent(transform);
            //        musicRoot.SetParent(transform);
            //    }

            //    public AudioSource PlaySoundEffect(string audioFilename, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false)
            //    {
            //        string audioPath = FormatAudioPath(sfxPath, audioFilename);

            //        AudioClip clip = Resources.Load<AudioClip>(audioPath);

            //        if (clip == null)
            //        {
            //            Debug.LogError($"Could not load audio file {audioPath}");
            //            return null;
            //        }

            //        return PlaySoundEffect(clip, mixer, volume, pitch, loop);
            //    }

            //    public AudioSource PlaySoundEffect(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false)
            //    {
            //        AudioSource effectSource = new GameObject(string.Format(sfxNameFormat, clip.name)).AddComponent<AudioSource>();
            //        effectSource.transform.SetParent(sfxRoot);
            //        effectSource.transform.position = sfxRoot.position;

            //        effectSource.clip = clip;

            //        if(mixer == null)
            //        {
            //            mixer = sfxMixer;
            //        }

            //        effectSource.outputAudioMixerGroup = mixer;
            //        effectSource.volume = volume;
            //        effectSource.spatialBlend = 0;
            //        effectSource.pitch = pitch;
            //        effectSource.loop = loop;

            //        effectSource.Play();

            //        if (!loop)
            //        {
            //            Destroy(effectSource.gameObject, (clip.length / pitch) + 1);
            //        }

            //        return effectSource;
            //    }

            //    public void StopSoundEffect(AudioClip clip) => StopSoundEffect(clip.name);

            //    public void StopSoundEffect(string audioFilename)
            //    {
            //        audioFilename = audioFilename.ToLower();

            //        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();

            //        foreach(var source in sources)
            //        {
            //            if (source.clip.name.ToLower() == audioFilename)
            //            {
            //                if (source != null) Destroy(source.gameObject);
            //                return;
            //            }
            //        }
            //    }

            //    public AudioTrack PlayTrack(string audioFilename, bool loop = true, float startingVolume = 0f, float volumeCap = 1)
            //    {
            //        //Debug.Log(musicIsPlaying);

            //        string audioPath = FormatAudioPath(musicPath, audioFilename);

            //        AudioClip clip = Resources.Load<AudioClip>(audioPath);

            //        if (clip == null)
            //        {
            //            Debug.LogError($"Could not load audio file {audioPath}");
            //            return null;
            //        }

            //        if (audioFilename != "Fire")
            //        {
            //            musicIsPlaying = audioFilename;
            //        }

            //        return PlayTrack(clip, loop, startingVolume, volumeCap);
            //    }

            //    public AudioTrack PlayTrack(AudioClip clip, bool loop = true, float startingVolume = 0f, float volumeCap = 1)
            //    {
            //        if (TryGetTrack(clip.name, out AudioTrack existingTrack))
            //        {
            //            if (!existingTrack.isPlaying)
            //            {
            //                existingTrack.Play();
            //            }

            //            return existingTrack;
            //        }

            //        AudioTrack track = new AudioTrack(clip, loop, startingVolume, volumeCap, musicMixer);
            //        tracks.Add(track);

            //        track.Play();

            //        return track;
            //    }

            //    public void StopTrack(string audioFilename)
            //    {
            //        if (TryGetTrack(audioFilename, out AudioTrack currentTrack))
            //        {
            //            if (currentTrack.isPlaying)
            //            {
            //                currentTrack.Stop();
            //                tracks.Remove(currentTrack);
            //            }
            //        }
            //    }

            //    public bool TryGetTrack(string trackName, out AudioTrack value)
            //    {
            //        trackName = trackName.ToLower();

            //        foreach (var track in tracks)
            //        {
            //            if (track.trackName.ToLower() == trackName)
            //            {
            //                value = track;
            //                return true;
            //            }
            //        }

            //        value = null;
            //        return false;
            //    }

            //    private string FormatAudioPath(string path, string audioFilename) => path.Replace(audioFilenameId, audioFilename);
        }
        private void OnValidate()
        {
                vca.setVolume(masterVolume);
        }
    }
}

