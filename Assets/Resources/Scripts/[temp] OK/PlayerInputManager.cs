using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Dialogue;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

    private PlayerInput input;
    private List<(InputAction action, Action<InputAction.CallbackContext> command)> actions = new List<(InputAction action, Action<InputAction.CallbackContext> command)>();

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
    }

    private void OnEnable()
    {
        foreach(var inputAction in actions)
        {
            inputAction.action.performed += inputAction.command;

            if(inputAction.action == input.actions["Move"])
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

            if(axisValue < 0)
            {
                choiceContainer.MoveSelection(1);
            }
            else if(axisValue > 0)
            {
                choiceContainer.MoveSelection(-1);
            }
            
        }
    }

    public void NextLine(InputAction.CallbackContext context)
    {
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
        if (Player.Instance == null)
        {
            return;
        }

        Player.Instance.move = context.ReadValue<Vector2>();

        if (context.canceled)
        {
            Player.Instance.move = Vector2.zero;
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {

    }
}

