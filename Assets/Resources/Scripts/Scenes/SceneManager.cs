using Cinemachine;
using System.Collections;
using UnityEngine;
using Dialogue;
using UnityEngine.UI;
using FMODUnity;
using System.Xml.Linq;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    [SerializeField] private RectTransform _pixelContainer = null;
    [SerializeField] private FollowCamera _vnContainerFollowCamera;
    [SerializeField] private StudioListener studioListener;
    public RectTransform pixelContainer => _pixelContainer;

    [SerializeField] private SceneConfig _config = null;
    public SceneConfig config => _config;

    public InteractableManager interactableManager { get; private set; }
    public NPCManager npcManager { get; private set; }

    public const string SPRITES_OBJECTNAME = "Sprites";

    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private CinemachineVirtualCamera panCamera;
    private CinemachineConfiner playerCamConfiner;
    private CinemachineConfiner panCamConfiner;
    private CinemachineComponentBase playerCamComponentBase;
    private CinemachineBrain cinemachineBrain;

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
        playerCamComponentBase = playerCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
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
        string storyToPlay = collidingInteractable.storyToPlay;
        Vector2 moveToInteractPosition = collidingInteractable.moveToInteractPosition;

        BackgroundData[] backgroundsToGoInScene = config.GetBackgroundsToGoInScene(currentSceneName, currentBackground);

        foreach (BackgroundData backgroundData in backgroundsToGoInScene)
        {
            if (backgroundData.interactableName.Equals(collidingInteractable.interactableName))
            {
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

                    break;
                }
            }
        }
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

    private void PutPlayerInScene(bool playerOverride = false, Vector2 playerPositionOverride = default, int playerDirectionOverride = 0)
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
            GameObject playerPrefab = FilePaths.GetPrefabFromPath(FilePaths.spritesPrefabPath, MC_NAME);

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
        panCamera.Priority = 2;
        cinemachineBrain.m_DefaultBlend.m_Time = DEFAULT_BLEND;

        Vector3 originalPosition = playerCamera.transform.position;
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

        panCamera.transform.position = new Vector2(position.x, playerCamera.transform.position.y);
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

    public IEnumerator ScrollBackground()
    {
        RawImage scrollImage = currentScene.GetComponentInChildren<RawImage>();

        while (scrollBackground == true)
        {
            scrollImage.uvRect = new Rect(scrollImage.uvRect.position + new Vector2(0.05f, scrollImage.uvRect.y) * Time.deltaTime, scrollImage.uvRect.size);

            if(scrollBackground == false)
            {
                break;
            }

            yield return null;
        }
    }
}
