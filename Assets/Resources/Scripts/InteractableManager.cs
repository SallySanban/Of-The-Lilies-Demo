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

    List<(string interactableName, string background, string scene)> lockedInteractables = new List<(string, string, string)>
    {
        ("Door", "First Floor Corridor", "Scene 1")
    };

    List<(string interactableName, string background, string scene)> uninteractables = new List<(string, string, string)>
    {
        ("Left Door", "Main Shop", "Scene 1"),
        ("Right Door", "Main Shop", "Scene 2"),
        ("Left Side", "Kuchai Town", "Scene 2"),
        ("Sabina Door", "Kuchai Town", "Scene 2"),
        ("Tavern Door", "Kuchai Town", "Scene 2" ),
        ("Ingredients Door", "Kuchai Town", "Scene 2"),
        ("Left Side", "Kuchai Town", "Scene 3"),
        ("Sabina Door", "Kuchai Town", "Scene 3"),
        ("Tavern Door", "Kuchai Town", "Scene 3"),
        ("Quan Door", "Kuchai Town", "Scene 3"),
        ("Sabina Door", "Kuchai Town", "Scene 4"),
        ("Tavern Door", "Kuchai Town", "Scene 4"),
        ("Ingredients Door", "Kuchai Town", "Scene 4"),
        ("Quan Door", "Kuchai Town", "Scene 4"),
        ("Sprig 1", "Kadlagan Forest", "Scene 4"),
        ("Sprig 2", "Kadlagan Forest", "Scene 4"),
        ("Statue", "Kadlagan Forest", "Scene 5"),
        ("Left Side", "Kuchai Town", "Scene 5"),
        ("Sabina Door", "Kuchai Town", "Scene 5"),
        ("Tavern Door", "Kuchai Town", "Scene 5" ),
        ("Ingredients Door", "Kuchai Town", "Scene 5"),
        ("Left Side", "Kuchai Town", "Scene 6"),
        ("Sabina Door", "Kuchai Town", "Scene 6"),
        ("Ingredients Door", "Kuchai Town", "Scene 6"),
        ("Quan Door", "Kuchai Town", "Scene 6"),
        ("Newspaper", "Tavern", "Scene 7"),
        ("Barkeeper", "Tavern", "Scene 8"),
        ("Left Side", "Kuchai Town", "Scene 8"),
        ("Tavern Door", "Kuchai Town", "Scene 8" ),
        ("Ingredients Door", "Kuchai Town", "Scene 8"),
        ("Quan Door", "Kuchai Town", "Scene 8"),
        ("Left Side", "Kuchai Town", "Combat Scene - Prologue"),
        ("Sabina Door", "Kuchai Town", "Combat Scene - Prologue"),
        ("Tavern Door", "Kuchai Town", "Combat Scene - Prologue"),
        ("Ingredients Door", "Kuchai Town", "Combat Scene - Prologue"),
        ("Quan Door", "Kuchai Town", "Combat Scene - Prologue")
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
            if (collidingInteractable.keysToPress.Any(key => Input.GetKeyDown(key)) && collidingInteractable.isInteractable && !DialogueSystem.Instance.speechBubbleActive)
            {
                collidingInteractable.icon.gameObject.SetActive(false);

                switch (collidingInteractable.interactableType)
                {
                    case Interactable.InteractableType.BackgroundSwitcher:
                        if (!collidingInteractable.isLocked)
                        {
                            StartCoroutine(PlaySceneAfterBackgroundSwitch());

                            break;
                        }
                        else
                        {
                            sceneManager.PlayNextScene(sceneManager.sceneName, collidingInteractable.backgroundInteractableIsIn, collidingInteractable.interactableName);
                        }

                        break;
                    case Interactable.InteractableType.PixelSprite:
                        StartCoroutine(WaitForMovePlayer());

                        break;
                    case Interactable.InteractableType.Object:
                        collidingInteractable.isInteractable = false;
                        sceneManager.PlayNextScene(sceneManager.sceneName, collidingInteractable.backgroundInteractableIsIn, collidingInteractable.interactableName);

                        break;
                }
            }
        }
    }

    private IEnumerator WaitForMovePlayer()
    {
        yield return spriteManager.currentPlayer.MoveSprite(spriteManager.currentPlayer.root.transform.position, collidingInteractable.icon.transform.position, 3f, true, true);
        yield return new WaitForSeconds(0.2f);
        sceneManager.PlayNextScene(sceneManager.sceneName, collidingInteractable.backgroundInteractableIsIn, collidingInteractable.interactableName);
    }
    private IEnumerator PlaySceneAfterBackgroundSwitch()
    {
        string lastInteractableScene = sceneManager.sceneName;
        string lastInteractableBackground = collidingInteractable.backgroundInteractableIsIn;
        string lastInteractable = collidingInteractable.interactableName;

        Debug.Log("SCENE: " + lastInteractableScene + " BACKGROUND: " + lastInteractableBackground + " INTERACTABLE: " + lastInteractable);

        if (sceneManager.HasNextScene(sceneManager.sceneName, collidingInteractable.backgroundInteractableIsIn, collidingInteractable.interactableName)) //has scene right after changing background
        {
            yield return sceneManager.SetupBackground(collidingInteractable.backgroundToSwitch, collidingInteractable.playerPosition, collidingInteractable.playerScale, collidingInteractable.playerDirection);
            yield return new WaitForSeconds(0.2f);
            yield return sceneManager.PlayNextScene(lastInteractableScene, lastInteractableBackground, lastInteractable);
        }
        else
        {
            yield return sceneManager.SetupBackground(collidingInteractable.backgroundToSwitch, collidingInteractable.playerPosition, collidingInteractable.playerScale, collidingInteractable.playerDirection);
            yield return new WaitForSeconds(0.2f);
            yield return sceneManager.ShowScene(true);
        }
    }

    public void CollidingWithPlayer(bool colliding, Interactable interactable = null)
    {
        collidingWithPlayer = colliding;

        if (collidingWithPlayer)
        {
            collidingInteractable = interactable; //last interactable that was collided with
        }
    }

    public void SetupInteractablesInScene(Background background)
    {
        interactablesInScene.Clear();

        PopulateScene(background);

        interactablesInScene.AddRange(background.root.GetComponentsInChildren<Interactable>());

        foreach (Interactable interactable in interactablesInScene) //unlocks all to reset
        {
            interactable.isInteractable = false;
            interactable.isLocked = false;
        }

        for (int i = interactablesInScene.Count - 1; i >= 0; i--)
        {
            Interactable interactable = interactablesInScene[i];

            interactable.interactableName = interactable.gameObject.name;
            interactable.backgroundInteractableIsIn = background.backgroundName;

            var interactableToCheck = (interactable.interactableName, interactable.backgroundInteractableIsIn, sceneManager.sceneName);

            //locks interactables that cannot be accessed but icon will appear
            if (lockedInteractables.Contains(interactableToCheck))
            {
                Debug.Log("LOCKED " + interactable.interactableName + " ON " + sceneManager.sceneName);
                interactable.isLocked = true;
            }

            //makes some interactables uninteractable (icon will not appear)
            if (!uninteractables.Contains(interactableToCheck))
            {
                interactable.isInteractable = true;
            }
            else
            {
                Debug.Log("CANNOT INTERACT WITH " + interactable.interactableName + " ON " + sceneManager.sceneName);
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
                                interactable.playerScale = data.playerScaleInNextBackground;
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

    public void EnableDisableInteractable(string interactableName, bool enable)
    {
        foreach (Interactable interactable in interactablesInScene)
        {
            if(interactable.name == interactableName)
            {
                interactable.isInteractable = enable;
            }
        }
    }

    public void LockUnlockInteractable(string interactableName, bool unlock)
    {
        foreach (Interactable interactable in interactablesInScene)
        {
            if (interactable.name == interactableName)
            {
                interactable.isInteractable = unlock;
            }
        }
    }

    private void PopulateScene(Background background)
    {
        switch(sceneManager.sceneName)
        {
            case "Scene 1":
                if (background.backgroundName == "Main Shop")
                {
                    PixelSprite Seiji = spriteManager.CreateSprite("Seiji", new Vector2(6.05f, 1.55f), new Vector2(1f, 1f), BackgroundConfigData.PlayerDirection.right, background.root, "Scene 2");
                    Seiji.root.name = "Seiji";
                    Seiji.Show();
                }

                break;
            case "Scene 2":
                if(background.backgroundName == "Kuchai Town")
                {
                    PixelSprite Seiji = spriteManager.CreateSprite("Seiji", new Vector2(2.42f, 0.65f), new Vector2(1f, 1f), BackgroundConfigData.PlayerDirection.right, background.root, "Scene 3");
                    Seiji.root.name = "Seiji";
                    Seiji.Show();
                }
                else if(background.backgroundName == "Mr. Quan's Shop")
                {
                    PixelSprite Quan = spriteManager.CreateSprite("Mr. Quan", new Vector2(6.05f, 1.55f), new Vector2(1f, 1f), BackgroundConfigData.PlayerDirection.left, background.root, "Scene 4");
                    Quan.root.name = "Mr. Quan";
                    Quan.Show();
                }

                break;
            case "Scene 3":
                if(background.backgroundName == "Ingredients Shop")
                {
                    PixelSprite Shopkeeper = spriteManager.CreateSprite("Ingredients Shopkeeper", new Vector2(6.05f, 1.55f), new Vector2(1f, 1f), BackgroundConfigData.PlayerDirection.left, background.root, "Scene 5");
                    Shopkeeper.root.name = "Ingredients Shopkeeper";
                    Shopkeeper.Show();
                }

                break;
            case "Scene 5":
                if (background.backgroundName == "Mr. Quan's Shop")
                {
                    PixelSprite Quan = spriteManager.CreateSprite("Mr. Quan", new Vector2(6.05f, 1.55f), new Vector2(1f, 1f), BackgroundConfigData.PlayerDirection.left, background.root, "Scene 6");
                    Quan.root.name = "Mr. Quan";
                    Quan.Show();
                }

                break;
            case "Scene 6":
                if (background.backgroundName == "Tavern")
                {
                    PixelSprite Barkeeper = spriteManager.CreateSprite("Barkeeper", new Vector2(6.05f, 1.55f), new Vector2(1f, 1f), BackgroundConfigData.PlayerDirection.left, background.root, "Scene 9");
                    Barkeeper.root.name = "Barkeeper";
                    Barkeeper.Show();
                }

                break;
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
