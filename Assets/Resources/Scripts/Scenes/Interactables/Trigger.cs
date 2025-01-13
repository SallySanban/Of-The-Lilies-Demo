using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [HideInInspector] public string triggerName;
    [HideInInspector] public TriggerType triggerType;
    [HideInInspector] public string storyToPlay;

    public void SetupInteractable(string name, InteractableType type, bool isInteractable, string storyToPlay, Vector2 moveToInteractPosition)
    {
        interactableName = name;
        interactableType = type;
        this.isInteractable = isInteractable;
        this.storyToPlay = storyToPlay;
        this.moveToInteractPosition = moveToInteractPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isInteractable && !DialogueManager.Instance.conversationManager.isRunning)
        {
            ShowHideIcon(true);
            InteractableManager.Instance.interactableCollidingWithPlayer = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isInteractable)
        {
            ShowHideIcon(false);
            InteractableManager.Instance.interactableCollidingWithPlayer = null;
        }
    }

    public void ShowHideIcon(bool show)
    {
        if (show)
        {
            icon.SetActive(true);
        }
        else
        {
            icon.SetActive(false);
        }
    }
}
