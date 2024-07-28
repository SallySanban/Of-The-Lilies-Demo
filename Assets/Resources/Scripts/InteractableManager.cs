using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Dialogue;

public class InteractableManager: MonoBehaviour
{
    public static InteractableManager Instance { get; private set; }
    public SceneManager sceneManager => SceneManager.Instance;
    public SpriteManager spriteManager => SpriteManager.Instance;
    public BackgroundManager backgroundManager => BackgroundManager.Instance;

    List<Interactable> interactablesInScene = new List<Interactable>();

    private const string iconNameId = "<iconName>";
    private string iconImagePath => $"Art/UI/Interact Icons/{iconNameId}";

    private bool collidingWithPlayer = false;
    private Interactable collidingInteractable = null;

    Dictionary<(string interactableName, string background), string> lockedInteractables = new Dictionary<(string, string), string>
    {
        { ("Door", "First Floor Corridor"), "Scene 1" }
    };

    Dictionary<(string interactableName, string background), string> uninteractables = new Dictionary<(string, string), string>
    {
        { ("Left Door", "Main Shop"), "Scene 1" },
        { ("Right Door", "Main Shop"), "Scene 2" }
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    private void Update()
    {
        if (collidingWithPlayer && collidingInteractable != null)
        {
            switch (collidingInteractable.interactableType)
            {
                case Interactable.InteractableType.BackgroundSwitcher:
                    if (collidingInteractable.keysToPress.Any(key => Input.GetKeyDown(key)) && collidingInteractable.isInteractable && !DialogueSystem.Instance.speechBubbleActive)
                    {
                        collidingInteractable.icon.gameObject.SetActive(false);

                        if (!collidingInteractable.isLocked)
                        {
                            sceneManager.SetupScene(collidingInteractable.backgroundToSwitch, collidingInteractable.playerPosition, collidingInteractable.playerDirection);
                        }
                        else
                        {
                            sceneManager.PlayNextScene(collidingInteractable);
                        }
                    }

                    break;
                case Interactable.InteractableType.PixelSprite:
                case Interactable.InteractableType.Object:
                    if (collidingInteractable.keysToPress.Any(key => Input.GetKeyDown(key)) && collidingInteractable.isInteractable && !DialogueSystem.Instance.speechBubbleActive)
                    {
                        collidingInteractable.icon.gameObject.SetActive(false);

                        sceneManager.PlayNextScene(collidingInteractable);
                    }

                    break;
            }
        }
    }

    public void CollidingWithPlayer(bool colliding, Interactable interactable = null)
    {
        collidingWithPlayer = colliding;

        if (collidingWithPlayer)
        {
            collidingInteractable = interactable;
        }
        else
        {
            collidingInteractable = null;
        }
    }

    public void SetupInteractablesInScene(Background background)
    {
        interactablesInScene.Clear();

        PopulateScene(background);

        interactablesInScene.AddRange(background.root.GetComponentsInChildren<Interactable>());

        for (int i = interactablesInScene.Count - 1; i >= 0; i--)
        {
            Interactable interactable = interactablesInScene[i];

            interactable.interactableName = interactable.gameObject.name;
            interactable.backgroundInteractableIsIn = background.backgroundName;

            //locks interactables that cannot be accessed but icon will appear
            var key = (interactable.interactableName, interactable.backgroundInteractableIsIn);
            if (lockedInteractables.TryGetValue(key, out string lockedInteractableScene))
            {
                if (lockedInteractableScene == sceneManager.sceneName)
                {
                    interactable.isLocked = true;
                }
            }

            //makes some interactables uninteractable (icon will not appear)
            key = (interactable.interactableName, interactable.backgroundInteractableIsIn);
            if (uninteractables.TryGetValue(key, out string uninteractableScene))
            {
                if (uninteractableScene == sceneManager.sceneName)
                {
                    interactable.isInteractable = false;
                    interactablesInScene.RemoveAt(i);
                }
            }
        }

        SetupInteractableIconsAndKeys();
    }

    //sets up the key to press and the icons of the interactables (and other relevant info)
    private void SetupInteractableIconsAndKeys()
    {
        foreach(Interactable interactable in interactablesInScene)
        {
            switch (interactable.interactableType)
            {
                case Interactable.InteractableType.BackgroundSwitcher:
                    if (interactable.isLocked)
                    {
                        string iconPathLockedBGSwitcher = FormatCGPath(iconImagePath, BackgroundConfigData.KeyToPress.Question);
                        interactable.icon.sprite = Resources.Load<Sprite>(iconPathLockedBGSwitcher);

                        interactable.keysToPress = GetKeysToPress(BackgroundConfigData.KeyToPress.Question);
                    }
                    else
                    {
                        BackgroundConfigData config = backgroundManager.config.GetConfig(backgroundManager.currentBackground.backgroundName);

                        foreach (BackgroundConfigData.InteractableToBackgroundMap data in config.map)
                        {
                            if (data.interactableName == interactable.interactableName)
                            {
                                string iconPathBGSwitcher = FormatCGPath(iconImagePath, data.keyToPress);
                                interactable.icon.sprite = Resources.Load<Sprite>(iconPathBGSwitcher);

                                interactable.keysToPress = GetKeysToPress(data.keyToPress);
                                interactable.backgroundToSwitch = data.backgroundPrefab.name;
                                interactable.playerPosition = data.playerPositionInNextBackground;
                                interactable.playerDirection = data.playerDirectionInNextBackground;

                                break;
                            }
                        }
                    }

                    break;
                case Interactable.InteractableType.PixelSprite:
                case Interactable.InteractableType.Object:
                    string iconPath = FormatCGPath(iconImagePath, BackgroundConfigData.KeyToPress.Question);
                    interactable.icon.sprite = Resources.Load<Sprite>(iconPath);

                    interactable.keysToPress = GetKeysToPress(BackgroundConfigData.KeyToPress.Question);

                    break;
            }
        }
    }

    private void PopulateScene(Background background)
    {
        //SCENE 1
        if (sceneManager.sceneName == "Scene 1")
        {
            if (background.backgroundName == "Main Shop")
            {
                PixelSprite Seiji = spriteManager.CreateSprite("Seiji", new Vector2(6.05f, 1.55f), BackgroundConfigData.PlayerDirection.right, background.root);
                Seiji.root.name = "Seiji";
                Seiji.Show();
            }
        }
    }

    private List<KeyCode> GetKeysToPress(BackgroundConfigData.KeyToPress keyToPress)
    {
        List<KeyCode> keys = new List<KeyCode>();

        switch (keyToPress)
        {
            case BackgroundConfigData.KeyToPress.Left:
                keys.Add(KeyCode.LeftArrow);
                keys.Add(KeyCode.A);
                break;
            case BackgroundConfigData.KeyToPress.Right:
                keys.Add(KeyCode.RightArrow);
                keys.Add(KeyCode.D);
                break;
            case BackgroundConfigData.KeyToPress.Up:
                keys.Add(KeyCode.UpArrow);
                keys.Add(KeyCode.W);
                break;
            case BackgroundConfigData.KeyToPress.Down:
                keys.Add(KeyCode.DownArrow);
                keys.Add(KeyCode.S);
                break;
            case BackgroundConfigData.KeyToPress.Question:
                keys.Add(KeyCode.Z);
                break;
        }

        return keys;
    }

    private string FormatCGPath(string path, BackgroundConfigData.KeyToPress filename) => path.Replace(iconNameId, filename.ToString());
}
