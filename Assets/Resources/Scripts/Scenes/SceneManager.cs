using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    [SerializeField] private RectTransform _pixelContainer = null;
    public RectTransform pixelContainer => _pixelContainer;

    [SerializeField] private SceneConfig _config = null;
    public SceneConfig config => _config;

    public InteractableManager interactableManager { get; private set; }
    public NPCManager npcManager { get; private set; }

    public const string SPRITES_OBJECTNAME = "Sprites";

    public GameObject currentScene = null;
    public string currentSceneName;
    public string currentBackground;
    public Player player = null;

    public string MC_NAME = "Ahlai";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            Initialize();
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    bool initialized = false;

    private void Initialize()
    {
        if (initialized) return;

        interactableManager = new InteractableManager();
        npcManager = new NPCManager();
    }

    private void Update()
    {
        if (player != null)
        {
            player.Move();
        }
    }

    public void CreateScene(string sceneName, string backgroundFilename)
    {
        GameObject backgroundPrefab = FilePaths.GetPrefabFromPath(FilePaths.backgroundPrefabPath, backgroundFilename);

        currentScene = Instantiate(backgroundPrefab, pixelContainer);

        currentSceneName = sceneName;
        currentBackground = backgroundPrefab.name;

        currentScene.SetActive(true);

        npcManager.PopulateNPCs(currentScene.transform.Find(SPRITES_OBJECTNAME));
        PutPlayerInScene();
        interactableManager.GetInteractablesInScene(currentScene);
    }

    public void SwitchScene(string sceneName)
    {
        currentSceneName = sceneName;

        npcManager.SwitchNPCs();
        PutPlayerInScene();
        interactableManager.GetInteractablesInScene(currentScene);
    }

    public void RemoveCurrentScene()
    {
        DestroyImmediate(currentScene);

        currentScene = null;
    }

    private void PutPlayerInScene()
    {
        (bool playerInScene, Vector2 playerPosition, int playerDirection) = config.GetPlayerInfo(currentSceneName, currentBackground);

        if (playerInScene)
        {
            GameObject playerPrefab = FilePaths.GetPrefabFromPath(FilePaths.spritesPrefabPath, MC_NAME);

            player = new Player(playerPrefab, currentScene.transform.Find(SPRITES_OBJECTNAME), playerPosition, playerDirection);
        }
    }
}
