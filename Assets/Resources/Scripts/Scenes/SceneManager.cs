using Cinemachine;
using System.Collections;
using UnityEngine;
using Dialogue;
using UnityEngine.UI;
using FMODUnity;
using System.Xml.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    [SerializeField] private Transform _pixelContainer = null;
    [SerializeField] private StudioListener studioListener;
    public Transform pixelContainer => _pixelContainer;

    [SerializeField] private SceneConfig _config = null;
    public SceneConfig config => _config;

    public InteractableManager interactableManager { get; private set; }
    public NPCManager npcManager { get; private set; }

    public const string SPRITES_OBJECTNAME = "Sprites";

    [SerializeField] private Camera pixelCamera;
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private CinemachineVirtualCamera panCamera;
    private CinemachineConfiner playerCamConfiner;
    private CinemachineConfiner panCamConfiner;
    private CinemachineBrain cinemachineBrain;

    [SerializeField] private Volume globalVolume;
    [SerializeField] private Light2D globalLight;


    [HideInInspector] public GameObject currentScene = null;
    [HideInInspector] public string currentSceneName;
    [HideInInspector] public string currentBackground;
    public Player player = null;

    [HideInInspector] public string MC_NAME = "Ahlai";
    [HideInInspector] public float TRANSITION_WAIT_TIME = 0.3f;
    [HideInInspector] public bool scrollBackground = false;
    [HideInInspector] public bool followPlayer = false;
    [HideInInspector] public NPC follower = null;

    private const float DEFAULT_BLEND = 2f;

    private bool isRunningConversation => DialogueManager.Instance.conversationManager.isRunning;

    [HideInInspector] public string currentBackgroundInteractableName = "";

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

        playerCamConfiner = playerCamera.GetComponent<CinemachineConfiner>();
        panCamConfiner = panCamera.GetComponent<CinemachineConfiner>();
        cinemachineBrain = pixelCamera.GetComponent<CinemachineBrain>();
    }

    private void Update()
    {
        if (player != null && !player.movingToInteract && (!isRunningConversation || InteractableManager.Instance.playerInsideStoryTrigger))
        {
            if (InteractableManager.Instance.playerInsideStopTrigger)
            {
                player.StopMoving();
            }
            else
            {
                player.Move();
            }
        }
    }

    //creates, sets up, and shows a background, including its NPCs, player, and interactables
    public void CreateScene(string sceneName, string backgroundFilename, Vector2 playerPositionInNextBackground = default, int playerDirectionInNextBackground = 0)
    {
        GameObject backgroundPrefab = FilePaths.GetPrefabFromPath(FilePaths.backgroundPrefabPath, backgroundFilename);

        currentScene = Instantiate(backgroundPrefab, pixelContainer);

        currentSceneName = sceneName;
        currentBackground = backgroundPrefab.name;

        Debug.Log("SCENE NAME: " + currentSceneName);
        Debug.Log("BACKGROUND NAME: " + currentBackground);

        currentScene.SetActive(true);

        npcManager.PopulateNPCs(currentScene.transform.Find(SPRITES_OBJECTNAME));
        PutPlayerInScene(playerPositionInNextBackground, playerDirectionInNextBackground);
        interactableManager.GetInteractablesInScene(currentScene);

        if (player != null)
        {
            playerCamera.OnTargetObjectWarped(playerCamera.Follow, player.root.transform.position - playerCamera.transform.position);
            playerCamera.Follow = player.root.transform;
        }

        playerCamConfiner.m_BoundingShape2D = currentScene.GetComponent<PolygonCollider2D>();
        panCamConfiner.m_BoundingShape2D = currentScene.GetComponent<PolygonCollider2D>();

        cinemachineBrain.m_DefaultBlend.m_Time = DEFAULT_BLEND;
    }

    //changes the background
    public IEnumerator SwitchBackground(BackgroundData.KeyToPress keyToPress, Interactable collidingInteractable)
    {
        currentBackgroundInteractableName = collidingInteractable.name;

        string storyToPlay = collidingInteractable.storyToPlay;
        Vector2 moveToInteractPosition = collidingInteractable.moveToInteractPosition;

        BackgroundData backgroundData = GetBackgroundData(currentBackgroundInteractableName);

        if (keyToPress == backgroundData.keyToPress)
        {
            if (!string.IsNullOrEmpty(storyToPlay)) //background will switch through the txt file & NPC will follow Ahlai through txt file
            {
                yield return VNManager.Instance.PlayCollidingInteractableStory(storyToPlay, moveToInteractPosition);
            }
            else
            {
                GraphicPanel blackout = UIManager.Instance.CreateUI<GraphicPanel>("Blackout");

                yield return blackout.Show();

                RemoveScene();

                CreateScene(currentSceneName, backgroundData.backgroundToGo, playerPositionInNextBackground: backgroundData.playerPositionInNextBackground, playerDirectionInNextBackground: backgroundData.playerDirectionInNextBackground);

                yield return new WaitForSeconds(TRANSITION_WAIT_TIME);

                if (!string.IsNullOrEmpty(backgroundData.followPlayer)) FollowPlayer(backgroundData.followPlayer, backgroundData.followPlayerPosition);

                yield return blackout.Hide();
            }
        }

        currentBackgroundInteractableName = "";
    }

    public BackgroundData GetBackgroundData(string collidingInteractableName)
    {
        if (string.IsNullOrEmpty(collidingInteractableName))
        {
            return null;
        }

        BackgroundData[] backgroundsToGoInScene = config.GetBackgroundsToGoInScene(currentSceneName, currentBackground);

        foreach (BackgroundData backgroundData in backgroundsToGoInScene)
        {
            if (backgroundData.interactableName.Equals(collidingInteractableName))
            {
                return backgroundData.Copy();
            }
        }

        return null;
    }

    public void RemoveScene()
    {
        player = null;
        followPlayer = false;
        follower = null;

        playerCamera.Follow = null;

        DialogueManager.Instance.currentTextbox = null;

        DestroyImmediate(currentScene);
    }

    private void PutPlayerInScene(Vector2 playerPositionOverride = default, int playerDirectionOverride = 0)
    {
        (bool playerInScene, Vector2 playerPosition, int playerDirection, string playerAnimationState) = config.GetPlayerInfo(currentSceneName, currentBackground);

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
            GameObject playerPrefab = null;

            switch (UIManager.Instance.inputPanel.subjectPronoun)
            {
                case "she":
                    playerPrefab = FilePaths.GetPrefabFromPath(FilePaths.spritesPrefabPath, "Fem" + MC_NAME);
                    break;
                case "he":
                    playerPrefab = FilePaths.GetPrefabFromPath(FilePaths.spritesPrefabPath, MC_NAME);
                    break;
                case "they":
                    playerPrefab = FilePaths.GetPrefabFromPath(FilePaths.spritesPrefabPath, MC_NAME);
                    break;
            }

            player = new Player(playerPrefab, currentScene.transform.Find(SPRITES_OBJECTNAME), playerPosition, playerDirection, playerAnimationState);

            studioListener.attenuationObject = player.root;
        }
        else
        {
            player = null;
        }
    }

    public void FollowPlayer(string npcName, Vector2 followNPCPosition = default)
    {
        follower = NPCManager.Instance.GetNPC(npcName);

        if(followNPCPosition != Vector2.zero)
        {
            follower.SetPosition(follower.root, followNPCPosition);
        }

        followPlayer = true;

        StartCoroutine(follower.FollowPlayer(follower.root));
    }

    public IEnumerator PanCamera(float targetX, float targetY, float duration)
    {
        Vector3 originalPosition;

        if (panCamera.Priority == 2)
        {
            originalPosition = panCamera.transform.position;
        }
        else
        {
            originalPosition = playerCamera.transform.position;
        }

        panCamera.Priority = 2;
        cinemachineBrain.m_DefaultBlend.m_Time = DEFAULT_BLEND;

        Vector3 targetPosition = Vector3.zero;

        if (targetX == 0)
        {
            targetPosition = new Vector2(originalPosition.x, targetY);
        }
        else if(targetY == 0)
        {
            targetPosition = new Vector2(targetX, originalPosition.y);
        }
        
        float startTime = Time.time;
        float elapsedTime = 0;

        while(elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t = t * t * (3f - 2f * t);

            panCamera.transform.position = Vector3.Lerp(originalPosition, targetPosition, t);

            elapsedTime = Time.time - startTime;
            yield return null;
        }

        panCamera.transform.position = targetPosition;
    }

    public void SetCamera(Vector2 position)
    {
        panCamera.Priority = 2;
        cinemachineBrain.m_DefaultBlend.m_Time = 0;

        position.x = position.x == 0 ? playerCamera.transform.position.x : position.x;
        position.y = position.y == 0 ? playerCamera.transform.position.y : position.y;

        panCamera.transform.position = new Vector2(position.x, position.y);
    }

    public void ResetCamera(bool smooth)
    {
        if(smooth)
        {
            cinemachineBrain.m_DefaultBlend.m_Time = DEFAULT_BLEND;
            panCamera.Priority = 0;
        }
        else
        {
            cinemachineBrain.m_DefaultBlend.m_Time = 0;
            panCamera.Priority = 0;
        }
    }

    public void SetCameraFollow(Transform npc)
    {
        panCamera.Follow = npc;
    }

    public void ChangeRender(string render, float value)
    {
        switch (render)
        {
            case "Color Adjustments":
                ColorAdjustments colorAdjustments;

                globalVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
                colorAdjustments.saturation.value = value;

                break;
            case "Vignette":
                break;
        }

        
    }

    public void ChangeLighting(float lighting)
    {
        globalLight.intensity = lighting;
    }
}
