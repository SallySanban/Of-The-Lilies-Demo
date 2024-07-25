using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Dialogue
{
    public class PlayerInputManager : MonoBehaviour
    {
        private PlayerInput input;
        private List<(InputAction action, Action<InputAction.CallbackContext> command)> actions = new List<(InputAction action, Action<InputAction.CallbackContext> command)>();

        private void Awake()
        {
            input = GetComponent<PlayerInput>();

            InitializeActions();
        }

        private void InitializeActions()
        {
            actions.Add((input.actions["Next"], NextLine));
            actions.Add((input.actions["Move"], context => Player.move = context.ReadValue<Vector2>()));
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

        public void NextLine(InputAction.CallbackContext context)
        {
            if (SceneManager.Instance.inVNMode || DialogueSystem.Instance.speechBubbleActive)
            {
                DialogueSystem.Instance.OnUserNext();
            }
        }
    }
}

