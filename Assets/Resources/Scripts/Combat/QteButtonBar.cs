using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QteButtonBar
{
    private SceneManager sceneManager => SceneManager.Instance;

    private Slider buttonBar;
    private GameObject keyParent;
    private GameObject keyTemplate;

    private float sliderTimer = 1f;
    private float speed;
    public bool stopTimer = false;

    public GameObject root = null;

    private QteButton currentButton = null;
    private List<QteButton> currentButtons = new List<QteButton>();
    private int currentButtonIndex = 0;

    public bool allButtonsCorrect = false;

    public QteButtonBar(GameObject prefab, Transform positionParent, List<string> buttonSequence, float speed)
    {
        if (prefab != null)
        {
            root = Object.Instantiate(prefab, positionParent.position, Quaternion.identity, positionParent);

            buttonBar = root.GetComponent<Slider>();
            keyParent = root.transform.Find("Keys").gameObject;
            keyTemplate = keyParent.transform.Find("Key").gameObject;

            SetupButtonBar(buttonSequence, speed);
        }
    }

    private void SetupButtonBar(List<string> buttonSequence, float speed)
    {
        buttonBar.maxValue = sliderTimer;
        buttonBar.value = sliderTimer;

        this.speed = speed;

        bool isFirstButton = true;
        foreach (string item in buttonSequence)
        {
            QteButton button = new QteButton(item, keyTemplate, keyParent.transform, isFirstButton);

            currentButtons.Add(button);

            isFirstButton = false;
        }

        currentButton = currentButtons[currentButtonIndex];
    }

    public void NextButton()
    {
        currentButton.arrow.SetActive(false);
        currentButton.rootCanvasGroup.alpha = 0f;

        currentButtonIndex++;
        currentButton = currentButtons[currentButtonIndex];

        currentButton.arrow.SetActive(true);
    }

    public void ResetButton()
    {
        currentButton.arrow.SetActive(false);

        foreach (QteButton button in currentButtons)
        {
            button.rootCanvasGroup.alpha = 1f;
        }

        currentButtonIndex = 0;
        currentButton = currentButtons[currentButtonIndex];

        currentButton.arrow.SetActive(true);
    }

    public IEnumerator Timer()
    {
        while (!stopTimer)
        {
            if (allButtonsCorrect) break;

            sliderTimer -= Time.deltaTime;
            yield return new WaitForSeconds(speed);

            if (sliderTimer <= 0)
            {
                stopTimer = true;
                break;
            }

            buttonBar.value = sliderTimer;
        }
    }
}