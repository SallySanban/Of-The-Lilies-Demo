using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Testing
{
    public class TestInputPanel : MonoBehaviour
    {
        public InputPanel inputPanel;

        void Start()
        {
            StartCoroutine(Running());
        }

        IEnumerator Running()
        {
            inputPanel.Show("Enter your name");

            while (inputPanel.isWaitingForUserInput)
            {
                yield return null;
            }

            string characterName = inputPanel.lastInput;

            Debug.Log("Hello " + characterName);
        }
    }
}

