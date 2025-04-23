using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AnimationAudio : MonoBehaviour
{
    public FMODUnity.EventReference sound;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void AxeSwingAudio()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(sound, gameObject);
    }

    void CombatMovementAudio()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_CombatMovement");
    }

    void CombatHitAudio()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_CombatHit");
    }

    void CombatThudAudio()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_CombatThud");
    }

    void CombatGrassStepAudio()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_CombatGrassStep");
    }

}
