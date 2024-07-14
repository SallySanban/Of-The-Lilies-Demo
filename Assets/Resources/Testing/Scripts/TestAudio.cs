using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

namespace Testing
{
    public class TestAudio : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(Running());
        }

        IEnumerator Running()
        {
            AudioManager.Instance.PlayTrack("Epilogue", startingVolume: 0.7f);
            AudioManager.Instance.PlaySoundEffect("Coins Jiggling");

            yield return new WaitForSeconds(3f);

            AudioManager.Instance.StopTrack("Epilogue");

            yield return null;
        }
    }
}

