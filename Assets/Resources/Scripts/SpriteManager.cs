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
        if (!DialogueSystem.Instance.speechBubbleActive && !sceneManager.inVNMode && currentPlayer != null)
        {
            currentPlayer.MoveSprite();
            currentPlayer.AnimatePlayer();
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

    public PixelSprite CreateSprite(string spriteName, Vector2 spritePosition, BackgroundConfigData.PlayerDirection spriteDirection, GameObject backgroundSpriteIsOn)
    {
        string prefabPath = FormatCGPath(spritePrefabPath, spriteName);
        GameObject spritePrefab = Resources.Load<GameObject>(prefabPath);

        PixelSprite sprite = new PixelSprite(spritePrefab, spritePosition, spriteDirection, backgroundSpriteIsOn.transform.Find(spriteContainer));
        sprite.root.name = spriteName;

        spritesInScene.Add(sprite);

        return sprite;
    }

    public Player CreatePlayer(string spriteName, Vector2 playerPosition, BackgroundConfigData.PlayerDirection playerDirection, GameObject backgroundSpriteIsOn)
    {
        string prefabPath = FormatCGPath(spritePrefabPath, spriteName);
        GameObject spritePrefab = Resources.Load<GameObject>(prefabPath);

        Player playerSprite = new Player(spritePrefab, playerPosition, playerDirection, backgroundSpriteIsOn.transform.Find(playerContainer));
        playerSprite.root.name = spriteName;

        currentPlayer = playerSprite;

        return playerSprite;
    }

    public void RemoveCurrentPlayer()
    {
        currentPlayer.Hide();

        currentPlayer = null;
    }

    public void RemoveAllSprites()
    {
        foreach(PixelSprite sprite in spritesInScene)
        {
            sprite.Hide();
        }

        spritesInScene.Clear();
    }

    private string FormatCGPath(string path, string filename) => filename != "" ? path.Replace(spriteNameId, filename) : "";
}
