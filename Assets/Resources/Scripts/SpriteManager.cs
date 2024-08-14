using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance { get; private set; }

    public SceneManager sceneManager => SceneManager.Instance;

    private const string spriteNameId = "<spriteName>";
    private string spritePrefabPath => $"Art/Sprites/{spriteNameId}/{spriteNameId}";
    private const string spriteContainer = "Sprites";
    private const string playerContainer = "Player";

    public Player currentPlayer = null;
    public List<PixelSprite> spritesInScene = new List<PixelSprite>();

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
        if (!DialogueSystem.Instance.speechBubbleActive && !sceneManager.inVNMode && currentPlayer != null && BackgroundManager.Instance.currentBackground != null && !CombatManager.Instance.playerInCombat)
        {
            if(Player.playerBeingMoved == false)
            {
                currentPlayer.MovePlayer();
            }
        }

        if(currentPlayer != null)
        {
            currentPlayer.AnimatePlayerWalk();
        }
        
    }

    [ContextMenu("ShowPlayerPosition")]
    public void ShowPlayerPosition()
    {
        Debug.Log(currentPlayer.root.transform.position);
    }

    [ContextMenu("ShowSpritesPosition")]
    public void ShowSpritesPosition()
    {
        foreach (PixelSprite sprite in spritesInScene)
        {
            Debug.Log($"{sprite.root.name} POSITION: {sprite.root.transform.position}");
        }
    }

    public PixelSprite CreateSprite(string spriteName, Vector2 spritePosition, Vector2 spriteScale, BackgroundConfigData.PlayerDirection spriteDirection, GameObject backgroundSpriteIsOn, string sceneToDisappear)
    {
        string prefabPath = FormatCGPath(spritePrefabPath, spriteName);
        GameObject spritePrefab = Resources.Load<GameObject>(prefabPath);

        PixelSprite sprite = new PixelSprite(spritePrefab, spritePosition, spriteScale, spriteDirection, backgroundSpriteIsOn.transform.Find(spriteContainer), sceneToDisappear);
        sprite.root.name = spriteName;

        spritesInScene.Add(sprite);

        return sprite;
    }

    public Enemy CreateEnemy(string spriteName, Vector2 spritePosition, Vector2 spriteScale, BackgroundConfigData.PlayerDirection spriteDirection, GameObject backgroundSpriteIsOn)
    {
        string prefabPath = FormatCGPath(spritePrefabPath, spriteName);
        GameObject spritePrefab = Resources.Load<GameObject>(prefabPath);

        Enemy sprite = new Enemy(spritePrefab, spritePosition, spriteScale, spriteDirection, backgroundSpriteIsOn.transform.Find(spriteContainer));
        sprite.root.name = spriteName;

        return sprite;
    }

    public Player CreatePlayer(string spriteName, Vector2 playerPosition, Vector2 playerScale, BackgroundConfigData.PlayerDirection playerDirection, GameObject backgroundSpriteIsOn)
    {
        string prefabPath = FormatCGPath(spritePrefabPath, spriteName);
        GameObject spritePrefab = Resources.Load<GameObject>(prefabPath);

        Player playerSprite = new Player(spritePrefab, playerPosition, playerScale, playerDirection, backgroundSpriteIsOn.transform.Find(playerContainer));
        playerSprite.root.name = spriteName;

        currentPlayer = playerSprite;

        return playerSprite;
    }

    public void RemoveCurrentPlayer()
    {
        currentPlayer.Hide();

        currentPlayer = null;
    }

    public void RemoveAllSprites(bool switchingBackground)
    {
        for (int i = spritesInScene.Count - 1; i >= 0; i--)
        {
            PixelSprite sprite = spritesInScene[i];

            if (switchingBackground)
            {
                sprite.Hide();
                spritesInScene.RemoveAt(i);
            }
            else
            {
                if (sprite.sceneToDisappear == sceneManager.sceneName)
                {
                    sprite.Hide();
                    spritesInScene.RemoveAt(i);
                }
            }
        }
    }

    public void RemoveSprite(string spriteName)
    {
        for (int i = spritesInScene.Count - 1; i >= 0; i--)
        {
            PixelSprite sprite = spritesInScene[i];

            if(sprite.root.name == spriteName)
            {
                sprite.Hide();
                spritesInScene.RemoveAt(i);

                break;
            }
        }
    }

    private string FormatCGPath(string path, string filename) => filename != "" ? path.Replace(spriteNameId, filename) : "";
}
