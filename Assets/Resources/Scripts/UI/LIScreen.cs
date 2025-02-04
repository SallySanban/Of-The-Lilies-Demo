using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LIScreen
{
    public GameObject root;
    public UIManager uiManager => UIManager.Instance;
    private List<Image> imageList = new List<Image>();

    public LIScreen(GameObject prefab)
    {
        if (prefab != null)
        {
            root = Object.Instantiate(prefab, uiManager.graphicsContainer);
            root.SetActive(true);

            // Collect all Image components that are children of the root
            imageList.AddRange(root.GetComponentsInChildren<Image>());

            // Add event listeners to each image
            foreach (var image in imageList)
            {
                AddEventListeners(image);
            }
        }
    }

    private void AddEventListeners(Image image)
    {
        EventTrigger trigger = image.gameObject.AddComponent<EventTrigger>();

        // Pointer Enter event
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => { OnHoverEnter(image); });
        trigger.triggers.Add(pointerEnter);

        // Pointer Exit event
        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) => { OnHoverExit(image); });
        trigger.triggers.Add(pointerExit);

        // Pointer Click event
        EventTrigger.Entry pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) => { OnClick(image); });
        trigger.triggers.Add(pointerClick);
    }

    private void OnHoverEnter(Image image)
    {
        // Move the image upwards
        image.transform.localPosition += new Vector3(0, 10, 0);
    }

    private void OnHoverExit(Image image)
    {
        // Move the image back to its original position
        image.transform.localPosition -= new Vector3(0, 10, 0);
    }

    private void OnClick(Image image)
    {
        VNManager.Instance.PlayCollidingInteractableStory("Main 8 - " + image.name + " Paper Interaction");
    }
}
