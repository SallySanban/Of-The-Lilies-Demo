using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dialogue;
using UnityEngine.SceneManagement;

public class Interactable : MonoBehaviour
{
    public InteractableManager interactableManager => InteractableManager.Instance;

    [SerializeField] private Image _icon;
    [SerializeField] private InteractableType _interactableType;
    public Image icon => _icon;
    public InteractableType interactableType => _interactableType;

    [HideInInspector] public string interactableName;
    [HideInInspector] public string backgroundInteractableIsIn;
    [HideInInspector] public bool isInteractable = false; //icon will not appear if false
    [HideInInspector] public List<KeyCode> keysToPress = null;

    //for BackgroundSwitcher Interactables
    [HideInInspector] public string backgroundToSwitch = "";
    [HideInInspector] public Vector2 playerPosition = new Vector2(0f, 0f);
    [HideInInspector] public Vector2 playerScale = new Vector2(1f, 1f);
    [HideInInspector] public BackgroundConfigData.PlayerDirection playerDirection = BackgroundConfigData.PlayerDirection.right;
    [HideInInspector] public bool isLocked = false; //icon will appear but cannot go inside room

    private void Start()
    {
        if(interactableType != InteractableType.CombatTrigger)
        {
            icon.gameObject.SetActive(false);
            isInteractable = false;
            isLocked = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isInteractable && !DialogueSystem.Instance.speechBubbleActive && interactableType != InteractableType.CombatTrigger)
        {
            icon.gameObject.SetActive(true);
            interactableManager.CollidingWithPlayer(true, this);
        }
        else if(collision.CompareTag("Player") && interactableType == InteractableType.CombatTrigger && SceneManager.Instance.inCombatMode)
        {
            CombatManager.Instance.StartCombat(SceneManager.Instance.combatSceneName, gameObject.name);
            Destroy(this);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isInteractable && !DialogueSystem.Instance.speechBubbleActive && interactableType != InteractableType.CombatTrigger)
        {
            icon.gameObject.SetActive(true);
            interactableManager.CollidingWithPlayer(true, this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isInteractable && interactableType != InteractableType.CombatTrigger)
        {
            icon.gameObject.SetActive(false);

            interactableManager.CollidingWithPlayer(false);
        }
    }

    public enum InteractableType
    {
        BackgroundSwitcher,
        Object,
        PixelSprite,
        CombatTrigger
    }
}
