using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Dialogue
{
    public class PlayerInputManager : MonoBehaviour
    {
        [SerializeField] private GameObject playerSprite;
        private PlayerInput input;
        private List<(InputAction action, Action<InputAction.CallbackContext> command)> actions = new List<(InputAction action, Action<InputAction.CallbackContext> command)>();

        private Vector3 move = Vector3.zero;
        private float speed = 3f;
        private float steps = 2f;

        private void Awake()
        {
            input = GetComponent<PlayerInput>();

            InitializeActions();
        }

        private void InitializeActions()
        {
            actions.Add((input.actions["Next"], NextLine));
            actions.Add((input.actions["Move"], context => move = context.ReadValue<Vector2>()));
        }

        private void OnEnable()
        {
            foreach(var inputAction in actions)
            {
                inputAction.action.performed += inputAction.command;
                inputAction.action.canceled += inputAction.command;
            }
        }

        private void OnDisable()
        {
            foreach (var inputAction in actions)
            {
                inputAction.action.performed -= inputAction.command;
                inputAction.action.canceled -= inputAction.command;
            }
        }

        private void Update()
        {
            MoveSprite();
        }

        public void NextLine(InputAction.CallbackContext context)
        {
            DialogueSystem.Instance.OnUserNext();
        }

        public void MoveSprite()
        {
            if(move.x == -1)    //left
            {
                playerSprite.transform.eulerAngles = new Vector3(0, 180, 0);

                move.x -= steps;
            }
            else if(move.x == 1)    //right
            {
                playerSprite.transform.eulerAngles = new Vector3(0, 0, 0);

                move.x += steps;
            }

            playerSprite.transform.position += move * speed * Time.deltaTime;

        }
    }
}

