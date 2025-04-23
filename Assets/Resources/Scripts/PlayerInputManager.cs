using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Dialogue;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

    public SceneManager sceneManager => SceneManager.Instance;

    private PlayerInput input;
    private List<(InputAction action, Action<InputAction.CallbackContext> command)> actions = new List<(InputAction action, Action<InputAction.CallbackContext> command)>();
    private bool isRunningConversation => DialogueManager.Instance.conversationManager.isRunning;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            input = GetComponent<PlayerInput>();

            InitializeActions();
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void InitializeActions()
    {
        actions.Add((input.actions["Next"], NextLine));
        actions.Add((input.actions["MoveArrow"], MoveArrow));
        actions.Add((input.actions["Move"], MovePlayer));
        actions.Add((input.actions["Interact"], Interact));
        actions.Add((input.actions["SwitchBackground"], SwitchBackground));
        actions.Add((input.actions["ButtonBarCombat"], ButtonBarCombat));
        actions.Add((input.actions["SlidingBarCombat"], SlidingBarCombat));
    }

    private void OnEnable()
    {
        foreach (var inputAction in actions)
        {
            inputAction.action.performed += inputAction.command;

            if (inputAction.action == input.actions["Move"])
            {
                inputAction.action.canceled += inputAction.command;
            }
        }
    }

    private void OnDisable()
    {
        foreach (var inputAction in actions)
        {
            inputAction.action.performed -= inputAction.command;

            if (inputAction.action == input.actions["Move"])
            {
                inputAction.action.canceled -= inputAction.command;
            }
        }
    }

    private void MoveArrow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        ChoiceContainer choiceContainer = DialogueManager.Instance.currentTextbox?.currentChoiceContainer;

        if (choiceContainer != null && choiceContainer.isWaitingForUserChoice)
        {
            float axisValue = context.ReadValue<float>();

            if (axisValue < 0)
            {
                choiceContainer.MoveSelection(1);
            }
            else if (axisValue > 0)
            {
                choiceContainer.MoveSelection(-1);
            }
        }
    }

    public void NextLine(InputAction.CallbackContext context)
    {
        if (!isRunningConversation) return;

        ChoiceContainer choiceContainer = DialogueManager.Instance.currentTextbox?.currentChoiceContainer;

        if (choiceContainer != null && choiceContainer.isWaitingForUserChoice)
        {
            choiceContainer.AcceptChoice();
            return;
        }

        DialogueManager.Instance.OnUserNext();
    }

    public void MovePlayer(InputAction.CallbackContext context)
    {
        if (SceneManager.Instance.player == null)
        {
            return;
        }

        if ((isRunningConversation && !InteractableManager.Instance.playerInsideStoryTrigger) || InteractableManager.Instance.playerInsideStopTrigger)
        {
            SceneManager.Instance.player.StopMoving();

            return;
        }

        SceneManager.Instance.player.move = context.ReadValue<Vector2>();

        if (context.canceled)
        {
            SceneManager.Instance.player.StopMoving();
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (InteractableManager.Instance.interactableCollidingWithPlayer == null) return;

        if (InteractableManager.Instance.interactableCollidingWithPlayer.interactableType == Interactable.InteractableType.Background) return;

        Interactable collidingInteractable = InteractableManager.Instance.interactableCollidingWithPlayer;

        if (collidingInteractable.isInteractable)
        {
            StartCoroutine(VNManager.Instance.PlayCollidingInteractableStory(collidingInteractable.storyToPlay, collidingInteractable.moveToInteractPosition));
        }
    }

    public void SwitchBackground(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (InteractableManager.Instance.interactableCollidingWithPlayer == null) return;

        if (InteractableManager.Instance.interactableCollidingWithPlayer.interactableType != Interactable.InteractableType.Background) return;

        Interactable collidingInteractable = InteractableManager.Instance.interactableCollidingWithPlayer;

        if (collidingInteractable.isInteractable)
        {
            Vector2 direction = context.ReadValue<Vector2>();
            BackgroundData.KeyToPress keyToPress;

            switch ((direction.x, direction.y))
            {
                case (0f, 1f):
                    keyToPress = BackgroundData.KeyToPress.Up;
                    break;
                case (0f, -1f):
                    keyToPress = BackgroundData.KeyToPress.Down;
                    break;
                case (-1f, 0f):
                    keyToPress = BackgroundData.KeyToPress.Left;
                    break;
                case (1f, 0f):
                    keyToPress = BackgroundData.KeyToPress.Right;
                    break;
                default:
                    keyToPress = BackgroundData.KeyToPress.None;
                    break;
            }

            StartCoroutine(sceneManager.SwitchBackground(keyToPress, collidingInteractable));
        }
    }

    public void ButtonBarCombat(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (!SceneManager.Instance.inCombat) return;

        if (CombatManager.Instance.currentButtonBar == null) return;

        Vector2 direction = context.ReadValue<Vector2>();

        CombatManager combatManager = CombatManager.Instance;
        List<(string, Vector2)> keySequence = combatManager.currentKeySequence;

        if (combatManager.currentButtonIndex <= keySequence.Count - 1)
        {
            //if the key pressed is correct
            if (keySequence[combatManager.currentButtonIndex].Item2 == direction)
            {
                //if the key is the last in the sequence
                if (combatManager.currentButtonIndex == keySequence.Count - 1)
                {
                    combatManager.currentButtonBar.allButtonsCorrect = true;
                    combatManager.currentButtonIndex = 0;
                }
                else
                {
                    combatManager.currentButtonIndex++;
                    combatManager.currentButtonBar.NextButton();
                }
            }
            else //if key pressed is not correct
            {
                combatManager.currentButtonIndex = 0;
                combatManager.currentButtonBar.ResetButton();
            }
        }
    }

    public void SlidingBarCombat(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (!SceneManager.Instance.inCombat) return;

        if (CombatManager.Instance.currentSlidingBar == null) return;

        CombatManager combatManager = CombatManager.Instance;

        combatManager.currentSlidingBar.stopArrow = true;
    }
}

