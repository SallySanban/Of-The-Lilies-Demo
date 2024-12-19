using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager
{
    public static InteractableManager Instance { get; private set; }

    private SceneManager sceneManager => SceneManager.Instance;

    public List<Interactable> interactablesInScreen = new List<Interactable>();
    public List<Interactable> activeInteractables = new List<Interactable>();

    public Interactable interactableCollidingWithPlayer = null;

    public InteractableManager()
    {
        Instance = this;
    }

    public void GetInteractablesInScene(GameObject scene)
    {
        InteractableData[] interactableDataInScene = sceneManager.config.GetInteractablesInScene(sceneManager.currentSceneName, sceneManager.currentBackground);

        foreach (InteractableData interactableData in interactableDataInScene)
        {
            foreach (Interactable interactable in scene.GetComponentsInChildren<Interactable>())
            {
                if (interactable.gameObject.name.Equals(interactableData.interactableName))
                {
                    interactablesInScreen.Add(interactable);

                    interactable.SetupInteractable(interactableData.interactableName, interactableData.interactableType, interactableData.isInteractable, interactableData.storyToPlay, interactableData.moveToInteractPosition);
                }
            }
        }
    }

    public void SetInteractablesAfterInteraction(bool reset = false)
    {
        if (!reset)
        {
            foreach (Interactable interactable in interactablesInScreen)
            {
                if (interactable.isInteractable) activeInteractables.Add(interactable);

                interactable.isInteractable = false;
            }
        }
        else
        {
            foreach (Interactable interactable in activeInteractables)
            {
                interactable.isInteractable = true;
            }

            activeInteractables.Clear();
        }
    }
}
