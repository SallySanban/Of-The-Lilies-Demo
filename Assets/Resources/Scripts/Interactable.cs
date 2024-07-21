using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject _icon;
    public GameObject icon => _icon;

    private KeyCode[] keyToPress = null;

    private string backgroundToSwitch = "";
    private Vector2 playerPosition = new Vector2(0f, 0f);
    private BackgroundConfigData.PlayerDirection playerDirection = BackgroundConfigData.PlayerDirection.right;

    public BackgroundManager backgroundManager => BackgroundManager.Instance;
    public SceneManager sceneManager => SceneManager.Instance;

    private void Start()
    {
        icon.SetActive(false);
    }

    private void Update()
    {
        if(keyToPress != null)
        {
            if ((Input.GetKey(keyToPress[0]) || Input.GetKey(keyToPress[1])))
            {
                sceneManager.SetupScene(backgroundToSwitch, playerPosition, playerDirection);
            }
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
                    playerPosition = data.playerPositionInNextBackground;
                    playerDirection = data.playerDirectionInNextBackground;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            icon.SetActive(false);
            keyToPress = null;
        }
    }
}
