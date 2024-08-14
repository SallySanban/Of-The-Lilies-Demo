using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager: MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [SerializeField] public Vector2 sliderPosition;

    public bool playerInCombat = false;

    List<Enemy> enemiesInScene = new List<Enemy>();

    public SpriteManager spriteManager => SpriteManager.Instance;
    public SceneManager sceneManager => SceneManager.Instance;
    public BackgroundManager backgroundManager => BackgroundManager.Instance;

    private string healthBarPrefabPath => $"Art/UI/Health Bar/Health Bar";
    private string qteSlidingBarPrefabPath => $"Art/UI/Quick Time Events/QTE Sliding Bar";
    private string qteButtonBarPrefabPath => $"Art/UI/Quick Time Events/QTE Button Bar";

    private int currentStep = 0;

    private GameObject healthBar;
    private Slider healthBarSlider;
    private QteButtonBar qteButtonBar;
    private QteSlidingBar qteSlidingBar;

    private float waitAnimation = 1f;
    private float playerHealth;

    //TODO: IMPROVE IMPLEMENTATION FOR MIX AND MATCH QTES
    Dictionary<(string combatSceneName, string triggerName), List<(string keys, float speed)>> buttonBarKeys = new Dictionary<(string sceneName, string triggerName), List<(string keys, float speed)>>
    {
        { ("Prologue", "Trigger 1"), new List<(string keys, float speed)>
            {
                ("Up Up Down Down", 0.05f),
                ("Left Right Left", 0.03f),
                ("Up Down Left", 0.01f)
            }
        },
        { ("Prologue", "Trigger 3"), new List<(string keys, float speed)>
            {
                ("Right Left Down Up Up", 0.01f),
                ("Up Up Left Right Down", 0.01f),
                ("Down Right Left Left Right", 0.01f)
            }
        }
    };

    Dictionary<(string combatSceneName, string triggerName), List<(QteSlidingBar.SliderLength length, float speed)>> slidingBarSlider = new Dictionary<(string sceneName, string triggerName), List<(QteSlidingBar.SliderLength length, float speed)>>
    {
        { ("Prologue", "Trigger 2"), new List<(QteSlidingBar.SliderLength length, float speed)>
            {
                (QteSlidingBar.SliderLength.Long, 2f),
                (QteSlidingBar.SliderLength.Medium, 3f),
                (QteSlidingBar.SliderLength.Medium, 4f),
                (QteSlidingBar.SliderLength.Short, 4f),
            }
        },
        { ("Prologue", "Trigger 3"), new List<(QteSlidingBar.SliderLength length, float speed)>
            {
                (QteSlidingBar.SliderLength.Short, 4f)
            }
        }
    };

    private KeyCode[] keySequence = null;
    private bool currentlyButtonBar = false;
    private bool currentlySlidingBar = false;

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
        if(currentlyButtonBar)
        {
            if (currentStep <= keySequence.Length - 1)
            {
                if (Input.GetKeyDown(keySequence[currentStep]))
                {
                    if (currentStep == keySequence.Length - 1)
                    {
                        qteButtonBar.allButtonsCorrect = true;
                        currentStep = 0;
                    }
                    else
                    {
                        currentStep++;
                        qteButtonBar.NextButton();
                    }
                }
                else if (AnyKeyBut(keySequence[currentStep]))
                {
                    currentStep = 0;
                    qteButtonBar.ResetButton();
                }
            }
        }
        else if (currentlySlidingBar)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                qteSlidingBar.stopArrow = true;
            }
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
        if(qteButtonBar != null) Debug.Log("BUTTON BAR: " + qteButtonBar.root.transform.position);
        if (qteSlidingBar != null) Debug.Log("SLIDING BAR: " + qteSlidingBar.root.transform.position);
        if (qteSlidingBar != null) Debug.Log("SLIDING BAR SLIDER: " + qteSlidingBar.slider.anchoredPosition);
    }

    [ContextMenu("ShowSliderSize")]
    public void ShowSliderSize()
    {
        if (qteSlidingBar != null) Debug.Log("SIZE SLIDER: " + qteSlidingBar.slider.sizeDelta);
    }

    public void SetupCombatScene()
    {
        GameObject healthBarPrefab = Resources.Load<GameObject>(healthBarPrefabPath);
        healthBar = Instantiate(healthBarPrefab, sceneManager.pixelSceneContainer);
        healthBarSlider = healthBar.transform.Find("Slider").GetComponent<Slider>();

        playerHealth = healthBarSlider.maxValue;
    }

    public void StartCombat(string combatSceneName, string triggerName)
    {
        playerInCombat = true;

        if(combatSceneName == "Prologue")
        {
            switch(triggerName)
            {
                case "Trigger 1":
                    StartCoroutine(TriggerOne(combatSceneName, triggerName));

                    break;
                case "Trigger 2":
                    StartCoroutine(TriggerTwo(combatSceneName, triggerName));

                    break;
                case "Trigger 3":
                    StartCoroutine(TriggerThree(combatSceneName, triggerName));

                    break;
            }
        }
    }

    //TODO: IMPROVE IMPLEMENTATION FOR REUSABILITY FOR ALL SOLDIERS PER TRIGGER
    private IEnumerator TriggerOne(string combatSceneName, string triggerName)
    {
        Coroutine soldier1 = StartCoroutine(CreateEnemy(new Vector2(20.62f, -0.49f), 9.89f, new Vector2(0.88f, 0.88f), "left"));

        yield return soldier1;

        yield return ButtonBarSequence(combatSceneName, triggerName);

        yield return EndTrigger(false);
    }

    private IEnumerator TriggerTwo(string combatSceneName, string triggerName)
    {
        Coroutine soldier1 = StartCoroutine(CreateEnemy(new Vector2(30.76f, -0.49f), 22.76f, new Vector2(0.88f, 0.88f), "left"));
        Coroutine soldier2 = StartCoroutine(CreateEnemy(new Vector2(7.40f, -0.49f), 15.42f, new Vector2(0.88f, 0.88f), "right"));

        yield return soldier1;
        yield return soldier2;

        yield return SlidingBarSequence(combatSceneName, triggerName);

        yield return EndTrigger(false);
    }

    private IEnumerator TriggerThree(string combatSceneName, string triggerName)
    {
        Coroutine soldier1 = StartCoroutine(CreateEnemy(new Vector2(42.87f, -0.49f), 37.79f, new Vector2(0.88f, 0.88f), "left"));
        Coroutine soldier2 = StartCoroutine(CreateEnemy(new Vector2(44.50f, -0.49f), 39.77f, new Vector2(0.88f, 0.88f), "left"));
        Coroutine soldier3 = StartCoroutine(CreateEnemy(new Vector2(21.54f, -0.49f), 27.52f, new Vector2(0.88f, 0.88f), "right"));
        Coroutine soldier4 = StartCoroutine(CreateEnemy(new Vector2(23.97f, -0.49f), 29.54f, new Vector2(0.88f, 0.88f), "right"));

        yield return soldier1;
        yield return soldier2;
        yield return soldier3;
        yield return soldier4;

        yield return ButtonBarSequence(combatSceneName, triggerName);

        yield return EndTrigger(true);
    }

    private IEnumerator ButtonBarSequence(string combatSceneName, string triggerName)
    {
        GameObject buttonBarPrefab = Resources.Load<GameObject>(qteButtonBarPrefabPath);

        var key = (combatSceneName, triggerName);
        if (buttonBarKeys.TryGetValue(key, out List<(string keys, float speed)> value))
        {
            foreach (var item in value)
            {
                (string keys, float speed) = item;

                string[] buttonSequence = keys.Split(" ");
                qteButtonBar = new QteButtonBar(buttonBarPrefab, buttonSequence, speed);

                keySequence = new KeyCode[buttonSequence.Length];

                for (int i = 0; i < buttonSequence.Length; i++)
                {
                    keySequence[i] = ConvertStringToKeyCode(buttonSequence[i]);
                }

                currentlyButtonBar = true;
                yield return qteButtonBar.Timer();

                Destroy(qteButtonBar.root);

                if (qteButtonBar.stopTimer)
                {
                    Enemy randomEnemy = GetRandomEnemy();
                    randomEnemy.AnimateAttack();

                    yield return new WaitForSeconds(0.1f);

                    SpriteManager.Instance.currentPlayer.AnimateHurt();

                    float playerDamage = (100 / value.Count) + 1;
                    yield return DecreaseHealthBar(playerDamage);

                    yield return new WaitForSeconds(waitAnimation);

                    SpriteManager.Instance.currentPlayer.AnimateAttack();

                    yield return new WaitForSeconds(0.1f);

                    foreach (Enemy enemy in enemiesInScene)
                    {
                        enemy.AnimateHurt();
                    }

                    float enemyDamage = 10;
                    yield return DecreaseEnemiesHealthBar(enemyDamage);

                    yield return new WaitForSeconds(waitAnimation);
                }
                else
                {
                    Enemy randomEnemy = GetRandomEnemy();

                    SpriteManager.Instance.currentPlayer.AnimateAttack();

                    yield return new WaitForSeconds(0.1f);

                    foreach(Enemy enemy in enemiesInScene)
                    {
                        enemy.AnimateHurt();
                    }

                    float damage;

                    if(triggerName == "Trigger 3")
                    {
                        damage = (100 / (value.Count + 1)) + 1;
                    }
                    else
                    {
                        damage = (100 / value.Count) + 1;
                    }
                    
                    yield return DecreaseEnemiesHealthBar(damage);

                    yield return new WaitForSeconds(waitAnimation);
                }

                if (playerHealth <= 0)
                {
                    yield return Restart();
                    currentlyButtonBar = false;
                    yield break;
                }

                currentlyButtonBar = false;
            }

            if(triggerName == "Trigger 3")
            {
                yield return SlidingBarSequence(combatSceneName, triggerName);
            }
        }
    }

    private IEnumerator SlidingBarSequence(string combatSceneName, string triggerName)
    {
        GameObject slidingBarPrefab = Resources.Load<GameObject>(qteSlidingBarPrefabPath);

        var key = (combatSceneName, triggerName);
        if (slidingBarSlider.TryGetValue(key, out List<(QteSlidingBar.SliderLength length, float speed)> value))
        {
            foreach (var item in value)
            {
                (QteSlidingBar.SliderLength length, float speed) = item;

                qteSlidingBar = new QteSlidingBar(slidingBarPrefab, length, speed);

                currentlySlidingBar = true;
                yield return qteSlidingBar.MoveArrow();

                yield return new WaitForSeconds(0.3f);

                bool success = qteSlidingBar.CheckForSuccess();

                Destroy(qteSlidingBar.root);

                if (success)
                {
                    Enemy randomEnemy = GetRandomEnemy();

                    SpriteManager.Instance.currentPlayer.AnimateAttack();

                    yield return new WaitForSeconds(0.1f);

                    foreach (Enemy enemy in enemiesInScene)
                    {
                        enemy.AnimateHurt();
                    }

                    float damage;

                    if (triggerName == "Trigger 3")
                    {
                        damage = (100 / (value.Count + 3)) + 1;
                    }
                    else
                    {
                        damage = (100 / value.Count) + 1;
                    }

                    yield return DecreaseEnemiesHealthBar(damage);

                    yield return new WaitForSeconds(waitAnimation);
                }
                else
                {
                    Enemy randomEnemy = GetRandomEnemy();
                    randomEnemy.AnimateAttack();
                    
                    yield return new WaitForSeconds(0.1f);

                    SpriteManager.Instance.currentPlayer.AnimateHurt();

                    float playerDamage = (100 / value.Count) + 1;
                    yield return DecreaseHealthBar(playerDamage);

                    yield return new WaitForSeconds(waitAnimation);

                    SpriteManager.Instance.currentPlayer.AnimateAttack();

                    yield return new WaitForSeconds(0.1f);

                    foreach (Enemy enemy in enemiesInScene)
                    {
                        enemy.AnimateHurt();
                    }

                    float enemyDamage = 10;
                    yield return DecreaseEnemiesHealthBar(enemyDamage);

                    yield return new WaitForSeconds(waitAnimation);

                }

                if (playerHealth <= 0)
                {
                    yield return Restart();
                    currentlySlidingBar = false;
                    yield break;
                }

                currentlySlidingBar = false;
            }
        }
    }

    private IEnumerator Restart()
    {
        foreach (Enemy enemy in enemiesInScene)
        {
            enemy.Hide();
        }

        enemiesInScene.Clear();

        Destroy(healthBar);

        yield return SceneManager.Instance.SetupBackground("Kuchai Town", new Vector2(4.77f, -0.85f), new Vector2(0.88f, 0.88f), BackgroundConfigData.PlayerDirection.right);
        yield return SceneManager.Instance.ShowScene(true);
        yield return SceneManager.Instance.SetupScene();
    }

    private IEnumerator EndTrigger(bool endCombatMode)
    {
        playerInCombat = false;

        if (enemiesInScene.Count == 0) yield break;

        if (enemiesInScene[0].health > 0)
        {
            yield return Restart();
            yield break;
        }
        else
        {
            foreach (Enemy enemy in enemiesInScene)
            {
                enemy.Hide();
            }

            enemiesInScene.Clear();

            if (endCombatMode)
            {
                Destroy(healthBar);
                yield return sceneManager.PlayNextScene(sceneManager.combatSceneName, BackgroundManager.Instance.currentBackground.backgroundName, "");
            }
        }
    }

    private IEnumerator DecreaseHealthBar(float damage)
    {
        float originalHealth = playerHealth;

        while (playerHealth > originalHealth - damage || playerHealth <= 0)
        {
            playerHealth -= 1;
            yield return new WaitForSeconds(0.001f);

            if (playerHealth <= originalHealth - damage || playerHealth <= 0) break;

            if (playerHealth <= 0)
            {
                healthBarSlider.value = 0;
            }
            else
            {
                healthBarSlider.value = playerHealth;
            }
        }
    }

    private IEnumerator DecreaseEnemiesHealthBar(float damage)
    {
        foreach(Enemy enemy in enemiesInScene)
        {
            float originalHealth = enemy.health;

            while (enemy.health > originalHealth - damage || enemy.health <= 0)
            {
                enemy.health -= 1;
                yield return new WaitForSeconds(0.001f);

                if (enemy.health <= originalHealth - damage || enemy.health <= 0) break;

                if(enemy.health <= 0)
                {
                    enemy.healthBar.value = 0;
                }
                else
                {
                    enemy.healthBar.value = enemy.health;
                }
            }
        }
    }

    private IEnumerator CreateEnemy(Vector2 startPosition, float targetPosition, Vector2 scale, string direction)
    {
        Enemy Soldier = spriteManager.CreateEnemy("Soldier", startPosition, scale, BackgroundConfigData.PlayerDirection.right, backgroundManager.currentBackground.root);
        Soldier.Show();

        Soldier.AnimateWalk(true, direction);
        yield return Soldier.MoveSprite(startPosition, new Vector2(targetPosition, startPosition.y), 6f);
        Soldier.AnimateWalk(false, direction);

        enemiesInScene.Add(Soldier);
    }

    private Enemy GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemiesInScene.Count);
        return enemiesInScene[randomIndex];
    }

    private KeyCode ConvertStringToKeyCode(string key)
    {
        switch (key)
        {
            case "Up":
                return KeyCode.UpArrow;
            case "Down":
                return KeyCode.DownArrow;
            case "Left":
                return KeyCode.LeftArrow;
            case "Right":
                return KeyCode.RightArrow;
            default:
                Debug.LogError("Invalid key string: " + key);
                return KeyCode.None;
        }
    }

    private bool AnyKeyBut(KeyCode expectedKey)
    {
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode) && keyCode != expectedKey)
            {
                return true;
            }
        }
        return false;
    }
}
