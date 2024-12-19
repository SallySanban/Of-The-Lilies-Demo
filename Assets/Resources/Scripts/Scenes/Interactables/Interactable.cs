using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private string interactableName;
    private bool locked;
    private bool isInteractable;
    private string sceneToPlay;

    [SerializeField] private GameObject icon;

    public void Awake()
    {
        icon.SetActive(false);
    }

    public void SetupInteractable(string name, bool locked, bool isInteractable, string sceneToPlay)
    {
        interactableName = name;
        this.locked = locked;
        this.isInteractable = isInteractable;
        this.sceneToPlay = sceneToPlay;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(isInteractable);

        if (collision.CompareTag("Player") && isInteractable)
        {
            icon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isInteractable)
        {
            icon.SetActive(false);
        }
    }
}
