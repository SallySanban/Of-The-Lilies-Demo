using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager
{
    private SceneManager sceneManager => SceneManager.Instance;

    private List<Interactable> interactablesInScreen = new List<Interactable>();

    private const string INTERACTABLES_OBJECTNAME = "Interactables";

    public void GetInteractablesInScene(GameObject scene)
    {
        InteractableData[] interactableDataInScene = sceneManager.config.GetInteractablesInScene(sceneManager.currentSceneName, sceneManager.currentBackground);

        Transform interactablesContainer = scene.transform.Find(INTERACTABLES_OBJECTNAME);

        if (interactablesContainer != null)
        {
            foreach (InteractableData interactableData in interactableDataInScene)
            {
                Interactable interactable = interactablesContainer.Find(interactableData.interactableName).GetComponent<Interactable>();

                interactablesInScreen.Add(interactable);

                interactable.SetupInteractable(interactableData.interactableName, interactableData.locked, interactableData.isInteractable, interactableData.sceneToPlay);
            }
        }
    }
}
