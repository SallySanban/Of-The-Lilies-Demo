using Cinemachine;
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

    [SerializeField] CinemachineVirtualCamera virtualCamera;
    private CinemachineConfiner confiner;

    [HideInInspector] public GameObject currentScene = null;
    [HideInInspector] public string currentSceneName;
    [HideInInspector] public string currentBackground;
    public Player player = null;

    [HideInInspector] public string MC_NAME = "Ahlai";

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

        confiner = virtualCamera.GetComponent<CinemachineConfiner>();
    }

    private void Update()
    {
        if (player != null)
        {
            player.Move();
        }
    }

    //creates, sets up, and shows a background, including its NPCs, player, and interactables
    public void CreateScene(string sceneName, string backgroundFilename, bool playerOverride = false, Vector2 playerPositionInNextBackground = default, int playerDirectionInNextBackground = 0)
    {
        GameObject backgroundPrefab = FilePaths.GetPrefabFromPath(FilePaths.backgroundPrefabPath, backgroundFilename);

        currentScene = Instantiate(backgroundPrefab, pixelContainer);

        currentSceneName = sceneName;
        currentBackground = backgroundPrefab.name;

        Debug.Log("SCENE NAME: " + currentSceneName);
        Debug.Log("BACKGROUND NAME: " + currentBackground);

        currentScene.SetActive(true);

        npcManager.PopulateNPCs(currentScene.transform.Find(SPRITES_OBJECTNAME));
        PutPlayerInScene(playerOverride, playerPositionInNextBackground, playerDirectionInNextBackground);
        interactableManager.GetInteractablesInScene(currentScene);

        if(player != null)
        {
            virtualCamera.OnTargetObjectWarped(virtualCamera.Follow, player.root.transform.position - virtualCamera.transform.position);
            virtualCamera.Follow = player.root.transform;
            confiner.m_BoundingShape2D = currentScene.GetComponent<PolygonCollider2D>();
        }
    }

    //changes the NPCs, player, and interactables for the SAME background
    public void SwitchScene(string sceneName)
    {
        currentSceneName = sceneName;

        npcManager.SwitchNPCs();
        PutPlayerInScene();
        interactableManager.GetInteractablesInScene(currentScene);
    }

    //changes the background
    public IEnumerator SwitchBackground(BackgroundData.KeyToPress keyToPress, Interactable collidingInteractable)
    {
        string storyToPlay = collidingInteractable.storyToPlay;
        Vector2 moveToInteractPosition = collidingInteractable.moveToInteractPosition;

        BackgroundData[] backgroundsToGoInScene = config.GetBackgroundsToGoInScene(currentSceneName, currentBackground);

        foreach (BackgroundData backgroundData in backgroundsToGoInScene)
        {
            if (backgroundData.interactableName.Equals(collidingInteractable.interactableName))
            {
                if (keyToPress == backgroundData.keyToPress)
                {
                    if (!string.IsNullOrEmpty(storyToPlay)) //background will switch through the txt file
                    {
                        yield return VNManager.Instance.PlayCollidingInteractableStory(storyToPlay, moveToInteractPosition);
                    }
                    else
                    {
                        GraphicPanel blackout = UIManager.Instance.CreateUI<GraphicPanel>("Blackout");

                        yield return blackout.Show();

                        DestroyImmediate(currentScene);

                        CreateScene(currentSceneName, backgroundData.backgroundToGo, playerPositionInNextBackground: backgroundData.playerPositionInNextBackground, playerDirectionInNextBackground: backgroundData.playerDirectionInNextBackground);

                        yield return blackout.Hide();
                    }

                    break;
                }
            }
        }
    }

    public void RemoveScene()
    {
        DestroyImmediate(currentScene);
    }

    private void PutPlayerInScene(bool playerOverride = false, Vector2 playerPositionOverride = default, int playerDirectionOverride = 0)
    {
        (bool playerInScene, Vector2 playerPosition, int playerDirection) = config.GetPlayerInfo(currentSceneName, currentBackground);

        if(playerPositionOverride != playerPosition && playerPositionOverride != Vector2.zero)
        {
            playerPosition = playerPositionOverride;
        }

        if(playerDirectionOverride != playerDirection && playerDirectionOverride != 0)
        {
            playerDirection = playerDirectionOverride;
        }

        if (playerInScene)
        {
            GameObject playerPrefab = FilePaths.GetPrefabFromPath(FilePaths.spritesPrefabPath, MC_NAME);

            player = new Player(playerPrefab, currentScene.transform.Find(SPRITES_OBJECTNAME), playerPosition, playerDirection);
        }
    }
}
