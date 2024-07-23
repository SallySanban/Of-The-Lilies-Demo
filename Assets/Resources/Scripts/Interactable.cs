using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    public InteractableManager interactableManager => InteractableManager.Instance;

    [SerializeField] private Image _icon;
    [SerializeField] private InteractableType _interactableType;
    public Image icon => _icon;
    public InteractableType interactableType => _interactableType;

    [HideInInspector] public string interactableName;
    [HideInInspector] public string backgroundInteractableIsIn;
    [HideInInspector] public bool isInteractable = true; //icon will not appear if false
    [HideInInspector] public List<KeyCode> keysToPress = null;

    //for BackgroundSwitcher Interactables
    [HideInInspector] public string backgroundToSwitch = "";
    [HideInInspector] public Vector2 playerPosition = new Vector2(0f, 0f);
    [HideInInspector] public BackgroundConfigData.PlayerDirection playerDirection = BackgroundConfigData.PlayerDirection.right;
    [HideInInspector] public bool isLocked = false;

    private void Start()
    {
        icon.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isInteractable)
        {
            icon.gameObject.SetActive(true);
            interactableManager.CollidingWithPlayer(true, this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isInteractable)
        {
            icon.gameObject.SetActive(false);

            interactableManager.CollidingWithPlayer(false);
        }
    }

    public enum InteractableType
    {
        BackgroundSwitcher,
        Object,
        PixelSprite
    }
}
