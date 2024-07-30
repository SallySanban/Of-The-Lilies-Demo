using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using Cinemachine;
using UnityEngine.UIElements;

public class SceneManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    private CinemachineConfiner confiner;

    private const string dialogueFile = "Scene 1";
    private const string playerSpriteName = "Ahlai";
    public static SceneManager Instance { get; private set; }
    public BackgroundManager backgroundManager => BackgroundManager.Instance;
    public SpriteManager spriteManager => SpriteManager.Instance;
    public InteractableManager interactableManager => InteractableManager.Instance;

    [HideInInspector] public string sceneName = "";

    [SerializeField] private RectTransform _pixelScene;
    [SerializeField] private CanvasGroup vnScene;

    public RectTransform pixelSceneContainer => _pixelScene;

    protected Coroutine showingVNCoroutine, hidingVNCoroutine;
    protected Coroutine settingBackgroundCoroutine, settingSceneCoroutine;

    public bool isVNShowing => showingVNCoroutine != null;
    public bool isVNHiding => hidingVNCoroutine != null;
    public bool isSettingBackground => settingBackgroundCoroutine != null;
    public bool isSettingScene => settingSceneCoroutine != null;

    public bool inVNMode => vnScene.alpha == 1f;

    private float fadeSpeed = 3f;

    Player newPlayer = null;

    Dictionary<(string sceneName, string background, string interactable), string> vnSceneProgression = new Dictionary<(string, string, string), string>
    {
        { ("Scene 1", "Main Shop", "Seiji"), "Scene 2" }
    };

    Dictionary<(string sceneName, string background, string interactable), List<(string name, string dialogue)>> speechBubbleProgression = new Dictionary<(string sceneName, string background, string interactable), List<(string name, string dialogue)>>
    {
        {
            ("Scene 1", "First Floor Corridor", "Door"), new List<(string name, string dialogue)>
            {
                ("Ahlai", "The door is locked...")
            }
        },
        {
            ("Scene 2", "Kuchai Town", "Seiji"), new List<(string name, string dialogue)>
            {
                ("Seiji", "Remember, Mr. Quan's house is the only one with the red walls."),
                ("Seiji", "He likes to stand out... It's not that hard to miss.")
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
        TextAsset startingScene = Resources.Load<TextAsset>(FilePaths.storyFiles + dialogueFile);

        List<string> lines = FileManager.ReadTextAsset(startingScene);

        DialogueSystem.Instance.SayTextbox(lines);

        //sceneName = "Scene 1";
        //SetupBackground("First Floor Corridor", new Vector2(0.01f, 0.44f), BackgroundConfigData.PlayerDirection.right, true);
        //-4.88f, -0.14
    }

    public Coroutine SetupBackground(string background, Vector2 playerPosition, BackgroundConfigData.PlayerDirection playerDirection, bool endVN = false)
    {
        if (isSettingBackground) return settingBackgroundCoroutine;

        settingBackgroundCoroutine = StartCoroutine(SwitchBackground(background, playerPosition, playerDirection, endVN));

        return settingBackgroundCoroutine;
    }

    private IEnumerator SwitchBackground(string background, Vector2 playerPosition, BackgroundConfigData.PlayerDirection playerDirection, bool endVN)
    {
        if (backgroundManager.currentBackground != null)
        {
            backgroundManager.RemoveCurrentBackground();
            spriteManager.RemoveCurrentPlayer();
            spriteManager.RemoveAllSprites();
        }

        yield return new WaitForSeconds(0.5f);

        Background newBackground = backgroundManager.CreateBackground(background);
        interactableManager.SetupInteractablesInScene(newBackground);

        newPlayer = spriteManager.CreatePlayer(playerSpriteName, playerPosition, playerDirection, newBackground.root);

        newBackground.Show();

        if (inVNMode && endVN) //change background and hide VN
        {
            HideVN();
            newPlayer.Show();
        }
        else if(!inVNMode)  //only change background, VN already hidden
        {
            newPlayer.Show();
        }

        virtualCamera.OnTargetObjectWarped(virtualCamera.Follow, spriteManager.currentPlayer.root.transform.position - virtualCamera.transform.position);

        virtualCamera.Follow = spriteManager.currentPlayer.root.transform;
        confiner.m_BoundingShape2D = backgroundManager.currentBackground.root.GetComponent<PolygonCollider2D>();

        settingBackgroundCoroutine = null;
    }

    public Coroutine SetupScene()
    {
        if (isSettingScene) return settingSceneCoroutine;

        settingSceneCoroutine = StartCoroutine(SwitchScene());

        return settingSceneCoroutine;
    }

    private IEnumerator SwitchScene()
    {
        if (backgroundManager.currentBackground != null)
        {
            spriteManager.RemoveAllSprites();
        }

        yield return new WaitForSeconds(0.5f);

        interactableManager.SetupInteractablesInScene(backgroundManager.currentBackground);

        settingSceneCoroutine = null;
    }

    public void PlayNextScene(Interactable interactableClicked)
    {
        string currentBackground = backgroundManager.currentBackground.backgroundName;
        string currentScene = sceneName;

        Debug.Log(currentBackground);
        Debug.Log(currentScene);

        var key = (currentScene, currentBackground, interactableClicked.interactableName);
        if (vnSceneProgression.TryGetValue(key, out string nextScene))
        {
            TextAsset sceneToPlay = Resources.Load<TextAsset>(FilePaths.storyFiles + nextScene);

            List<string> lines = FileManager.ReadTextAsset(sceneToPlay);

            ShowVN();

            DialogueSystem.Instance.SayTextbox(lines);

            return;
        }

        if (speechBubbleProgression.TryGetValue(key, out List<(string, string)> dialogue))
        {
            DialogueSystem.Instance.SaySpeechBubble(dialogue);

            return;
        }

        Debug.LogError("Next scene was not found!");
        return;
    }

    public Coroutine ShowVN()
    {
        if (isVNShowing) return showingVNCoroutine;

        if (isVNHiding)
        {
            StopCoroutine(hidingVNCoroutine);
        }

        showingVNCoroutine = StartCoroutine(ShowOrHideVNScene(true));

        return showingVNCoroutine;
    }

    public Coroutine HideVN()
    {
        if (isVNHiding) return hidingVNCoroutine;

        if (isVNShowing)
        {
            StopCoroutine(showingVNCoroutine);
        }

        newPlayer.Show();

        hidingVNCoroutine = StartCoroutine(ShowOrHideVNScene(false));

        return hidingVNCoroutine;
    }

    private IEnumerator ShowOrHideVNScene(bool show)
    {
        float targetAlpha = show ? 1f : 0f;

        CanvasGroup self = vnScene;

        while (self.alpha != targetAlpha)
        {
            self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, fadeSpeed * Time.deltaTime);

            yield return null;
        }

        showingVNCoroutine = null;
        hidingVNCoroutine = null;
    }
}
