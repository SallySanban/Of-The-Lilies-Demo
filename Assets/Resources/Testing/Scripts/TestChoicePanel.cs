using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Testing
{
    public class TestChoicePanel : MonoBehaviour
    {
        ChoicePanel choicePanel;

        void Start()
        {
            choicePanel = ChoicePanel.Instance;

            StartCoroutine(Running());
        }

        IEnumerator Running()
        {
            string[] choices = new string[]
            {
                "she/her",
                "he/him",
                "they/them"
            };

            choicePanel.Show(choices, ChoicePanel.ChoicePosition.Center);

            while (choicePanel.isWaitingForUserChoice)
            {
                yield return null;
            }

            var decision = choicePanel.lastChoicePicked;

            Debug.Log($"Choice picked: {decision.answerIndex} - {decision.choices[decision.answerIndex]}");
        }
    }
}

