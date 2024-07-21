using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using UnityEngine.UIElements;

public class SceneManager : MonoBehaviour
{
    private const string dialogueFile = "Test";
    private const string playerSpriteName = "Ahlai";
    public static SceneManager Instance { get; private set; }
    public BackgroundManager backgroundManager => BackgroundManager.Instance;
    public SpriteManager spriteManager => SpriteManager.Instance;

    public string sceneName = "";

    [SerializeField] private RectTransform _pixelScene;
    [SerializeField] private CanvasGroup vnScene;

    public RectTransform pixelSceneContainer => _pixelScene;

    protected Coroutine showingVNCoroutine, hidingVNCoroutine;
    protected Coroutine settingSceneCoroutine;

    public bool isVNShowing => showingVNCoroutine != null;
    public bool isVNHiding => hidingVNCoroutine != null;
    public bool isSettingScene => settingSceneCoroutine != null;

    public bool inVNMode => vnScene.alpha == 1f;

    private float fadeSpeed = 3f;

    Player newPlayer = null;

    private void Awake()
    {
        Instance = this;

        pixelSceneContainer.gameObject.SetActive(true);

        vnScene.gameObject.SetActive(true);
        vnScene.alpha = 1f;
    }

    private void Start()
    {
        TextAsset startOfStory = Resources.Load<TextAsset>(FilePaths.storyFiles + dialogueFile);

        List<string> lines = FileManager.ReadTextAsset(startOfStory);

        DialogueSystem.Instance.Say(lines);

        //SetupScene("Ahlai's Bedroom", new Vector2(-4.88f, -0.14f));
    }

    public Coroutine SetupScene(string background, Vector2 playerPosition, BackgroundConfigData.PlayerDirection playerDirection, bool endVN = false)
    {
        if (isSettingScene) return settingSceneCoroutine;

        settingSceneCoroutine = StartCoroutine(SwitchScene(background, playerPosition, playerDirection, endVN));

        return settingSceneCoroutine;
    }

    private IEnumerator SwitchScene(string background, Vector2 playerPosition, BackgroundConfigData.PlayerDirection playerDirection, bool endVN)
    {
        if (backgroundManager.currentBackground != null)
        {
            backgroundManager.RemoveCurrentBackground();
            spriteManager.RemoveCurrentPlayer();
        }

        yield return new WaitForSeconds(0.5f);

        Background newBackground = backgroundManager.CreateBackground(background);
        newPlayer = spriteManager.CreatePlayer(playerSpriteName, playerPosition, playerDirection, newBackground.root);

        newBackground.Show();

        if(inVNMode && endVN) //change background and hide VN
        {
            HideVN();
        }
        else if(!inVNMode)  //only change background, VN already hidden
        {
            newPlayer.Show();
        }

        settingSceneCoroutine = null;
    }

    public Coroutine ShowVN()
    {
        if (isVNShowing) return showingVNCoroutine;

        if (isVNHiding)
        {
            StopCoroutine(hidingVNCoroutine);
        }

        newPlayer.Hide();

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
