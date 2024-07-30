using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Testing
{
    public class TestInputPanel : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(Running());
        }

        IEnumerator Running()
        {
            InputPanel.Instance.Show();

            while (InputPanel.Instance.isWaitingForUserInput)
            {
                yield return null;
            }

            string characterName = InputPanel.Instance.lastInput;

            Debug.Log("Hello " + characterName);
        }
    }
}

