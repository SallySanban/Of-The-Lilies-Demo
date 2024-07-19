using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject _icon;
    public GameObject icon => _icon;

    private KeyCode keyToPress = KeyCode.None;

    private string backgroundToSwitch = "";

    public BackgroundManager backgroundManager => BackgroundManager.Instance;
    public SceneManager sceneManager => SceneManager.Instance;

    private void Start()
    {
        icon.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(keyToPress) && keyToPress != KeyCode.None)
        {
            sceneManager.SetupScene(backgroundToSwitch);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BackgroundConfigData config = backgroundManager.config.GetConfig(backgroundManager.currentBackground.backgroundName);

        if (collision.CompareTag("Player"))
        {
            icon.SetActive(true);

            foreach(BackgroundConfigData.InteractableToBackgroundMap data in config.map)
            {
                if (data.interactableName == gameObject.name)
                {
                    keyToPress = data.keyToPress;
                    backgroundToSwitch = data.backgroundPrefab.name;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            icon.SetActive(false);
            keyToPress = KeyCode.None;
        }
    }
}
