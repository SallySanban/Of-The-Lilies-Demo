using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private float parallaxEffectMultiplier;
    
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    private void Start()
    {
        GameObject cameraObject = GameObject.Find("Pixel Camera");

        if (cameraObject != null)
        {
            cameraTransform = cameraObject.transform;
        }

        lastCameraPosition = cameraTransform.position;
    }

    private void LateUpdate()

    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += deltaMovement * parallaxEffectMultiplier;
        lastCameraPosition = cameraTransform.position;
    }
}