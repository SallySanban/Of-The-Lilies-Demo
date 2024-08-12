using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatManager: MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public bool playerInCombat = false;

    List<Enemy> enemiesInScene = new List<Enemy>();

    public SpriteManager spriteManager => SpriteManager.Instance;
    public SceneManager sceneManager => SceneManager.Instance;
    public BackgroundManager backgroundManager => BackgroundManager.Instance;

    private string healthBarPrefabPath => $"Art/UI/Health Bar/Health Bar";
    private string qteSlidingBarPrefabPath => $"Art/UI/Quick Time Events/QTE Sliding Bar";
    private string qteButtonBarPrefabPath => $"Art/UI/Quick Time Events/QTE Button Bar";

    private GameObject healthBar;
    private QteButtonBar qteButtonBar;
    private QteSlidingBar qteSlidingBar;

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

    [ContextMenu("ShowEnemiesPosition")]
    public void ShowEnemiesPosition()
    {
        foreach (Enemy sprite in enemiesInScene)
        {
            Debug.Log($"{sprite.root.name} POSITION: {sprite.root.transform.position}");
        }
    }

    [ContextMenu("ShowQTEPosition")]
    public void ShowQtePosition()
    {
        Debug.Log("BUTTON BAR: " + qteButtonBar.root.transform.position);
        Debug.Log("SLIDING BAR: " + qteSlidingBar.root.transform.position);
    }

    public void SetupCombatScene()
    {
        //GameObject healthBarPrefab = Resources.Load<GameObject>(healthBarPrefabPath);
        //healthBar = Instantiate(healthBarPrefab, sceneManager.pixelSceneContainer);
    }

    public void StartCombat(string combatSceneName, string triggerName)
    {
        playerInCombat = true;

        if(combatSceneName == "Prologue")
        {
            switch(triggerName)
            {
                case "Trigger 1":
                    Vector2 currentPosition = new Vector2(20.62f, -0.58f);
                    StartCoroutine(TriggerOne(currentPosition, new Vector2(11.35f, currentPosition.y), new Vector2(0.88f, 0.88f)));
                    
                    break;
                case "Trigger 2":
                    break;
                case "Trigger 3":
                    break;
            }

            
        }
    }

    private IEnumerator TriggerOne(Vector2 enemyPosition, Vector2 enemyTargetPosition, Vector2 enemyScale)
    {
        Enemy Soldier = spriteManager.CreateEnemy("Soldier", enemyPosition, enemyScale, BackgroundConfigData.PlayerDirection.left, backgroundManager.currentBackground.root);
        Soldier.Show();
        yield return Soldier.MoveSprite(enemyPosition, enemyTargetPosition, 6f);

        GameObject slidingBarPrefab = Resources.Load<GameObject>(qteSlidingBarPrefabPath);
        GameObject buttonBarPrefab = Resources.Load<GameObject>(qteButtonBarPrefabPath);

        //first attack
        qteButtonBar = new QteButtonBar(buttonBarPrefab);

        enemiesInScene.Add(Soldier);
    }
}
