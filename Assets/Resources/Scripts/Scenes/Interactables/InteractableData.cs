using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InteractableData
{
    public string interactableName;
    public bool locked;
    public bool isInteractable;
    public string sceneToPlay;

    public InteractableData Copy()
    {
        InteractableData result = new InteractableData();

        result.interactableName = interactableName;
        result.locked = locked;
        result.isInteractable = isInteractable;
        result.sceneToPlay = sceneToPlay;

        return result;
    }
}
