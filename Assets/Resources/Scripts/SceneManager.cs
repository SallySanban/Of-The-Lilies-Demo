using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using Cinemachine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    private CinemachineConfiner confiner;

    private const string dialogueFile = "Scene 12";
    private const string playerSpriteName = "Ahlai";
    private const string combatSceneIdentifier = "Combat Scene - ";
    public static SceneManager Instance { get; private set; }
    public BackgroundManager backgroundManager => BackgroundManager.Instance;
    public SpriteManager spriteManager => SpriteManager.Instance;
    public InteractableManager interactableManager => InteractableManager.Instance;

    [HideInInspector] public string sceneName = "";
    [HideInInspector] public string combatSceneName = "";
    [HideInInspector] public bool inCombatMode = false;

    [SerializeField] private RectTransform _pixelScene;
    [SerializeField] private CanvasGroup vnScene;

    public RectTransform pixelSceneContainer => _pixelScene;

    protected Coroutine showingVNCoroutine, hidingVNCoroutine;
    protected Coroutine showingPixelCoroutine, hidingPixelCoroutine;
    protected Coroutine settingBackgroundCoroutine, settingSceneCoroutine;

    public bool isVNShowing => showingVNCoroutine != null;
    public bool isVNHiding => hidingVNCoroutine != null;
    public bool isPixelShowing => showingPixelCoroutine != null;
    public bool isPixelHiding => hidingPixelCoroutine != null;
    public bool isSettingBackground => settingBackgroundCoroutine != null;
    public bool isSettingScene => settingSceneCoroutine != null;

    public bool inVNMode = true;

    private float fadeSpeed = 3f;

    Player newPlayer = null;
    Background newBackground = null;

    Dictionary<(string sceneName, string background, string interactable), string> vnSceneProgression = new Dictionary<(string, string, string), string>
    {
        { ("Scene 1", "Main Shop", "Seiji"), "Scene 2" },
        { ("Scene 2", "Kuchai Town", "Quan Door"), "Scene 3" },
        { ("Scene 3", "Ingredients Shop", "Ingredients Shopkeeper"), "Scene 4" },
        { ("Scene 4", "Kadlagan Forest", "Statue"), "Scene 5" },
        { ("Scene 5", "Kuchai Town", "Quan Door"), "Scene 6" },
        { ("Scene 6", "Kuchai Town", "Tavern Door"), "Scene 7" },
        { ("Scene 7", "Tavern", "Newspaper"), "Scene 8" },
        { ("Bad Ending 1", "Tavern", "Left Door"), "Bad Ending 1" },
        { ("Scene 8", "Kuchai Town", "Sabina Door"), "Scene 9" },
        { ("Prologue", "Kuchai Town", ""), "Scene 11" },
        { ("Scene 13", "First Floor Corridor", "Stairs"), "Scene 14" },
        { ("Expected Ending", "First Floor Corridor", "Door"), "Expected Ending" }
    };

    Dictionary<(string sceneName, string background, string interactable), List<(string command, string key, string value)>> speechBubbleProgression = new Dictionary<(string sceneName, string background, string interactable), List<(string command, string key, string value)>>
    {
        {
            ("Scene 1", "First Floor Corridor", "Door"), new List<(string command, string key, string value)>
            {
                ("Speech Bubble", "Ahlai", "The door is locked...")
            }
        },
        {
            ("Scene 2", "Kuchai Town", "Seiji"), new List<(string command, string key, string value)>
            {
                ("Speech Bubble", "Seiji", "Mr. Quan's house is the only one with the red walls."),
                ("Speech Bubble", "Seiji", "He likes to stand out... It's not that hard to miss.")
            }
        },
        {
            ("Scene 4", "Ingredients Shop", "Ingredients Shopkeeper"), new List<(string command, string key, string value)>
            {
                ("Speech Bubble", "Ingredients Shopkeeper", "The Kadlagan Forest is not too far from here."),
                ("Speech Bubble", "Ingredients Shopkeeper", "But remember to only take what you need...")
            }
        },
        {
            ("Scene 5", "Kadlagan Forest", "Sprig 1"), new List<(string command, string key, string value)>
            {
                ("Speech Bubble", "Ahlai", "Got a sprig.")
            }
        },
        {
            ("Scene 5", "Kadlagan Forest", "Sprig 2"), new List<(string command, string key, string value)>
            {
                ("Speech Bubble", "Ahlai", "Got a sprig.")
            }
        },
        {
            ("Scene 7", "Tavern", "Barkeeper"), new List<(string command, string key, string value)>
            {
                ("Speech Bubble", "Ahlai", "Excuse me, can we have some drinks to go?"),
                ("Speech Bubble", "Ahlai", "Around three will do."),
                ("Speech Bubble", "Barkeeper", "Three's more than the usual, pal."),
                ("Speech Bubble", "Barkeeper", "It'll take me some time."),
                ("Speech Bubble", "Ahlai", "No problem."),
                ("Command", "Enable Interactable", "Newspaper"),
                ("Command", "Disable Interactable", "Barkeeper")
            }
        },
        {
            ("Scene 13", "First Floor Corridor", "Door"), new List<(string command, string key, string value)>
            {
                ("Speech Bubble", "Ahlai", "The door is locked...")
            }
        }
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            pixelSceneContainer.gameObject.SetActive(true);

            vnScene.gameObject.SetActive(true);
            vnScene.alpha = 1f;

            confiner = virtualCamera.GetComponent<CinemachineConfiner>();
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
        //TextAsset startingScene = Resources.Load<TextAsset>(FilePaths.storyFiles + dialogueFile);

        //List<string> lines = FileManager.ReadTextAsset(startingScene);

        //DialogueSystem.Instance.SayTextbox(lines);

        //sceneName = "Combat Scene - Prologue";
        sceneName = "Expected Ending";
        StartCoroutine(Test());
    }

    private IEnumerator Test()
    {
        yield return SetupBackground("First Floor Corridor", new Vector2(0.01f, -0.04f), new Vector2(1f, 1f), BackgroundConfigData.PlayerDirection.right);
        //yield return SetupBackground("Kuchai Town", new Vector2(4.77f, -0.85f), new Vector2(0.88f, 0.88f), BackgroundConfigData.PlayerDirection.right);
        yield return ShowScene(true);
        yield return SetupScene();
    }

    public Coroutine SetupBackground(string background, Vector2 playerPosition, Vector2 playerScale, BackgroundConfigData.PlayerDirection playerDirection)
    {
        if (isSettingBackground) return settingBackgroundCoroutine;

        settingBackgroundCoroutine = StartCoroutine(SwitchBackground(background, playerPosition, playerScale, playerDirection));

        return settingBackgroundCoroutine;
    }

    private IEnumerator SwitchBackground(string background, Vector2 playerPosition, Vector2 playerScale, BackgroundConfigData.PlayerDirection playerDirection)
    {
        if (backgroundManager.currentBackground != null)
        {
            backgroundManager.RemoveCurrentBackground();
            spriteManager.RemoveCurrentPlayer();
            spriteManager.RemoveAllSprites(true);
        }

        yield return new WaitForSeconds(0.5f);

        newBackground = backgroundManager.CreateBackground(background);

        newPlayer = spriteManager.CreatePlayer(playerSpriteName, playerPosition, playerScale, playerDirection, newBackground.root);

        virtualCamera.OnTargetObjectWarped(virtualCamera.Follow, spriteManager.currentPlayer.root.transform.position - virtualCamera.transform.position);

        virtualCamera.Follow = newPlayer.root.transform;
        confiner.m_BoundingShape2D = newBackground.root.GetComponent<PolygonCollider2D>();

        settingBackgroundCoroutine = null;
    }

    public Coroutine ShowScene(bool endVN = false)
    {
        if (isPixelShowing)
        {
            return showingPixelCoroutine;
        }

        if (isPixelHiding)
        {
            StopCoroutine(hidingPixelCoroutine);
        }

        showingPixelCoroutine = StartCoroutine(ShowingScene(endVN));

        return showingPixelCoroutine;
    }

    private IEnumerator ShowingScene(bool endVN = false)
    {
        yield return HideVN();
        newBackground.Show();
        newPlayer.Show();
        interactableManager.SetupInteractablesInScene(newBackground);

        if (!endVN) //change background and VN not done
        {
            yield return new WaitForSeconds(0.5f);
            ShowVN();
        }

        showingPixelCoroutine = null;
    }

    public Coroutine HideScene()
    {
        if (isPixelHiding) return hidingPixelCoroutine;

        newPlayer.Hide();

        if (backgroundManager.currentBackground != null)
        {
            spriteManager.RemoveAllSprites(false);
        }

        if (isPixelShowing)
        {
            StopCoroutine(showingPixelCoroutine);
        }

        hidingPixelCoroutine = StartCoroutine(ShowOrHideVNScene(pixelSceneContainer.GetComponent<CanvasGroup>(), false));

        return hidingPixelCoroutine;
    }

    //doesn't change the background
    public Coroutine SetupScene()
    {
        if (isSettingScene) return settingSceneCoroutine;

        settingSceneCoroutine = StartCoroutine(SwitchScene());

        if (sceneName.StartsWith(combatSceneIdentifier))
        {
            combatSceneName = sceneName.Substring(combatSceneIdentifier.Length);
            inCombatMode = true;
            CombatManager.Instance.SetupCombatScene();
        }

        return settingSceneCoroutine;
    }

    private IEnumerator SwitchScene()
    {
        Debug.Log("CURRENT SCENE: " + sceneName);

        if (backgroundManager.currentBackground != null)
        {
            spriteManager.RemoveAllSprites(false);
        }

        yield return new WaitForSeconds(0.5f);

        interactableManager.SetupInteractablesInScene(backgroundManager.currentBackground);

        settingSceneCoroutine = null;
    }

    public Coroutine PlayNextScene(string sceneClicked, string backgroundInteractableIsIn, string interactableClicked)
    {
        var key = (sceneClicked, backgroundInteractableIsIn, interactableClicked);
        if (vnSceneProgression.TryGetValue(key, out string nextScene))
        {
            TextAsset sceneToPlay = Resources.Load<TextAsset>(FilePaths.storyFiles + nextScene);

            List<string> lines = FileManager.ReadTextAsset(sceneToPlay);

            ShowVN();

            return DialogueSystem.Instance.SayTextbox(lines);
        }

        if (speechBubbleProgression.TryGetValue(key, out List<(string, string, string)> actions))
        {
            return DialogueSystem.Instance.SaySpeechBubble(actions);
        }

        Debug.LogError("Next scene was not found!");
        return null;
    }

    public bool HasNextScene(string sceneClicked, string backgroundInteractableIsIn, string interactableClicked)
    {
        var key = (sceneClicked, backgroundInteractableIsIn, interactableClicked);
        if (vnSceneProgression.TryGetValue(key, out string nextScene))
        {
            return true;
        }

        if (speechBubbleProgression.TryGetValue(key, out List<(string, string, string)> actions))
        {
            return true;
        }

        return false;
    }

    public Coroutine ShowVN()
    {
        if (isVNShowing) return showingVNCoroutine;

        if (isVNHiding)
        {
            StopCoroutine(hidingVNCoroutine);
        }

        showingVNCoroutine = StartCoroutine(ShowOrHideVNScene(vnScene, true));

        inVNMode = true;

        return showingVNCoroutine;
    }

    public Coroutine HideVN()
    {
        if (isVNHiding) return hidingVNCoroutine;

        if (isVNShowing)
        {
            StopCoroutine(showingVNCoroutine);
        }

        hidingVNCoroutine = StartCoroutine(ShowOrHideVNScene(vnScene, false));

        inVNMode = false;

        return hidingVNCoroutine;
    }

    private IEnumerator ShowOrHideVNScene(CanvasGroup objectToHide, bool show)
    {
        float targetAlpha = show ? 1f : 0f;

        while (objectToHide.alpha != targetAlpha)
        {
            objectToHide.alpha = Mathf.MoveTowards(objectToHide.alpha, targetAlpha, fadeSpeed * Time.deltaTime);

            yield return null;
        }

        showingVNCoroutine = null;
        hidingVNCoroutine = null;
        hidingPixelCoroutine = null;
    }
}
