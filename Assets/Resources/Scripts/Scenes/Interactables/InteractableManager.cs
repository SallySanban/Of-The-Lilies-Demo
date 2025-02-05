using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableManager
{
    public static InteractableManager Instance { get; private set; }

    private SceneManager sceneManager => SceneManager.Instance;

    public List<Interactable> interactablesInScreen = new List<Interactable>();
    public List<Interactable> activeInteractables = new List<Interactable>();

    public Interactable interactableCollidingWithPlayer = null;
    public bool playerInsideStopTrigger = false;
    public bool playerInsideStoryTrigger = false;

    public InteractableManager()
    {
        Instance = this;
    }

    public void GetInteractablesInScene(GameObject scene)
    {
        interactablesInScreen.Clear();

        InteractableData[] interactableDataInScene = sceneManager.config.GetInteractablesInScene(sceneManager.currentSceneName, sceneManager.currentBackground);

        foreach (Interactable interactable in scene.GetComponentsInChildren<Interactable>())
        {
            bool existsInData = false;

            foreach (InteractableData interactableData in interactableDataInScene)
            {
                if (interactable.gameObject.name.Equals(interactableData.interactableName))
                {
                    existsInData = true;

                    interactablesInScreen.Add(interactable);
                    interactable.SetupInteractable(
                        interactableData.interactableName,
                        interactableData.interactableType,
                        interactableData.isInteractable,
                        interactableData.storyToPlay,
                        interactableData.moveToInteractPosition
                    );
                }
            }

            if(NPCManager.Instance.GetNPC(interactable.gameObject.name) != null)
            {
                existsInData = true;
            }

            if (!existsInData)
            {
                Object.Destroy(interactable.gameObject);
            }
        }
    }

    public void RefreshInteractables()
    {
        GetInteractablesInScene(sceneManager.currentScene);
    }

    public Interactable GetInteractable(string interactableName)
    {
        foreach (Interactable interactable in interactablesInScreen)
        {
            if (interactable.interactableName.Equals(interactableName))
            {
                return interactable;
            }
        }

        return null;
    }

    public void SetInteractablesAfterInteraction(bool reset = false)
    {
        if (!reset)
        {
            foreach (Interactable interactable in interactablesInScreen)
            {
                if (interactable.isInteractable) 
                {
                    activeInteractables.Add(interactable);

                    interactable.isInteractable = false;
                    interactable.ShowHideIcon(false);
                }
            }

            interactableCollidingWithPlayer = null;
        }
        else
        {
            foreach (Interactable interactable in activeInteractables)
            {
                if (!interactable.stateChangedDuringStory)
                {
                    interactable.isInteractable = true;
                }
            }

            foreach(Interactable interactable in interactablesInScreen)
            {
                interactable.stateChangedDuringStory = false;
            }

            activeInteractables.Clear();

            playerInsideStopTrigger = false;
            playerInsideStoryTrigger = false;
        }
    }

    public void RemoveInteractable(string name)
    {
        foreach (Interactable interactable in interactablesInScreen)
        {
            if (interactable.interactableName.Equals(name))
            {
                Object.Destroy(interactable.gameObject);
                interactablesInScreen.Remove(interactable);
                sceneManager.config.RemoveInteractableFromScene(sceneManager.currentSceneName, sceneManager.currentBackground, name);

                break;
            }
        }
    }
}
