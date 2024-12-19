using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InteractableData
{
    public string interactableName;
    public Interactable.InteractableType interactableType;
    public bool isInteractable;
    public string storyToPlay;
    public Vector2 moveToInteractPosition;

    public InteractableData Copy()
    {
        InteractableData result = new InteractableData();

        result.interactableName = interactableName;
        result.interactableType = interactableType;
        result.isInteractable = isInteractable;
        result.storyToPlay = storyToPlay;
        result.moveToInteractPosition = moveToInteractPosition;

        return result;
    }
}
