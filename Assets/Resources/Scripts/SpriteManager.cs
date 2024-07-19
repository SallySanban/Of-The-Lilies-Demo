using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance { get; private set; }

    public SceneManager sceneManager => SceneManager.Instance;

    private const string spriteNameId = "<spriteName>";
    private string spritePrefabPath => $"Art/Sprites/{spriteNameId}/{spriteNameId}";
    private const string spriteContainer = "Sprites";
    private const string playerContainer = "Player";

    public Player currentPlayer = null;


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
        if (!sceneManager.inVNMode && currentPlayer != null)
        {
            currentPlayer.MoveSprite();
        }
    }

    public PixelSprite CreateSprite(string spriteName, GameObject backgroundSpriteIsOn)
    {
        string prefabPath = FormatCGPath(spritePrefabPath, spriteName);
        GameObject spritePrefab = Resources.Load<GameObject>(prefabPath);

        PixelSprite sprite = new PixelSprite(spritePrefab, backgroundSpriteIsOn.transform.Find(spriteContainer));

        return sprite;
    }

    public Player CreatePlayer(string spriteName, GameObject backgroundSpriteIsOn)
    {
        string prefabPath = FormatCGPath(spritePrefabPath, spriteName);
        GameObject spritePrefab = Resources.Load<GameObject>(prefabPath);

        Player playerSprite = new Player(spritePrefab, backgroundSpriteIsOn.transform.Find(playerContainer));

        currentPlayer = playerSprite;

        return playerSprite;
    }

    public void RemoveCurrentPlayer()
    {
        currentPlayer.Hide();

        currentPlayer = null;
    }

    private string FormatCGPath(string path, string filename) => filename != "" ? path.Replace(spriteNameId, filename) : "";
}
