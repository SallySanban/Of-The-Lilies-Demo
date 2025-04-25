using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatManager
{
    public static CombatManager Instance { get; private set; }
    private SceneManager sceneManager => SceneManager.Instance;

    public CombatSprite combatSprite;

    private const string POSITION_A = "Position A";
    private const string POSITION_B = "Position B";
    private const string POSITION_C = "Position C";

    private GameObject qteButtonBarPrefab;
    private GameObject qteSlidingBarPrefab;

    public QteButtonBar currentButtonBar;
    public QteSlidingBar currentSlidingBar;

    private const string BUTTONBAR_FILENAME = "QTE Button Bar";
    private const string SLIDINGBAR_FILENAME = "QTE Sliding Bar";

    private List<(string, Vector2)> possibleKeys = new List<(string, Vector2)>
    {
        ("Left", new Vector2(-1f, 0f)),
        ("Right", new Vector2(1f, 0f)),
        ("Up", new Vector2(0f, 1f)),
        ("Down", new Vector2(0f, -1f))
    };

    public List<(string, Vector2)> currentKeySequence = new List<(string, Vector2)>();

    public int currentButtonIndex = 0;
    public bool success = false;

    // List of animation states that are considered "attack" animations
    private readonly string[] attackAnimations = new string[] 
    {
        "AhlaiAttackEnemyHitA",
        "AhlaiAttackEnemyDodgeA",
        "EnemyAttackAhlaiHitA",
        "EnemyAttackAhlaiDodgeA",
        "AhlaiAttackEnemyHitB",
        "AhlaiAttackEnemyDodgeB",
        "EnemyAttackAhlaiHitB",
        "EnemyAttackAhlaiDodgeB",
        "AhlaiAttackEnemyHitC",
        "AhlaiAttackEnemyDodgeC",
        "EnemyAttackAhlaiHitC",
        "EnemyAttackAhlaiDodgeC",
        "AhlaiAttackEnemyDefeat",
        "EnemyAttackAhlaiDefeat"
    };

    private int health = 4;

    public CombatManager()
    {
        Instance = this;
    }

    public void Initialize(GameObject prefab, Transform parent, Vector2 position)
    {
        combatSprite = new CombatSprite(prefab, parent, position);

        qteButtonBarPrefab = FilePaths.GetPrefabFromPath(FilePaths.qteBarPrefabPath, BUTTONBAR_FILENAME);

        qteSlidingBarPrefab = FilePaths.GetPrefabFromPath(FilePaths.qteBarPrefabPath, SLIDINGBAR_FILENAME);

        health = 4;
    }

    public IEnumerator StartCombat()
    {
        Transform positionA = combatSprite.root.transform.Find(POSITION_A).transform;
        Transform positionB = combatSprite.root.transform.Find(POSITION_B).transform;
        Transform positionC = combatSprite.root.transform.Find(POSITION_C).transform;

        /* 
        * TODO: improve where to put this combat sequence to account for any combat situation
        * also take into account difficulty speed, # of keys, or length of success zone
        */

        //position A
        yield return new WaitForSeconds(1f);

        //new position A
        yield return combatSprite.ChangePositionA(65f);

        //position A
        yield return new WaitForSeconds(1f);

        //position A to C
        yield return SlidingBarSequence(positionA, 2f);

        //position C to A
        yield return ButtonBarSequence(positionC, 3, 0.05f);

        yield return new WaitForSeconds(1f);

        //new position A
        yield return combatSprite.ChangePositionA(55f);

        //position A to B
        yield return ButtonBarSequence(positionA, 4, 0.05f);

        //position B to A
        yield return SlidingBarSequence(positionB, 2f);

        yield return new WaitForSeconds(1f);

        //new position A
        yield return combatSprite.ChangePositionA(60f);

        yield return new WaitForSeconds(1f);

        //position A to C
        yield return SlidingBarSequence(positionA, 2f);

        yield return ButtonBarSequence(positionC, 4, 0.05f);

        yield return ButtonBarSequence(positionA, 4, 0.05f);

        //B to A
        yield return SlidingBarSequence(positionB, 2f);

        if (health != 0)
        {
            combatSprite.animator.SetBool("Win", true);

            yield return WaitForAnimationComplete();
        }

        GraphicPanel blackout = UIManager.Instance.CreateUI<GraphicPanel>("Blackout");

        yield return blackout.Show();
        Object.Destroy(combatSprite.root);
        SceneManager.Instance.playerCamera.Follow = SceneManager.Instance.player.root.transform;
        SceneManager.Instance.inCombat = false;
    }

    private IEnumerator WaitForAnimationComplete()
    {
        yield return null;

        bool hasCompletedOneCycle = false;

        float previousTime = 0;

        while (IsInAttackAnimation() && !hasCompletedOneCycle)
        {
            var currentInfo = combatSprite.animator.GetCurrentAnimatorStateInfo(0);
            float currentTime = currentInfo.normalizedTime;

            if (currentTime < previousTime || currentTime >= 1.0f)
            {
                hasCompletedOneCycle = true;
            }

            previousTime = currentTime;

            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
    }

    private bool IsInAttackAnimation()
    {
        var currentInfo = combatSprite.animator.GetCurrentAnimatorStateInfo(0);

        // Check if current animation is any of our attack animations
        foreach (string attackAnim in attackAnimations)
        {
            if (currentInfo.IsName(attackAnim))
            {
                return true;
            }
        }
        
        return false;
    }

    //ahlai attacking
    private IEnumerator ButtonBarSequence(Transform positionParent, int numberOfKeys, float speed)
    {
        currentButtonIndex = 0;

        GenerateKeysForButtonBar(numberOfKeys);

        currentButtonBar = new QteButtonBar(qteButtonBarPrefab, positionParent, currentKeySequence.Select(item => item.Item1).ToList(), speed);

        yield return currentButtonBar.Timer();

        Object.Destroy(currentButtonBar.root);

        if(positionParent.name == POSITION_A)   //going from A to B
        {
            sceneManager.StartCoroutine(sceneManager.PanCamera(sceneManager.playerCamera.transform.position.x + 1.5f, 0, 2f));
        }
        else if (positionParent.name == POSITION_C)  //going from C to A
        {
            sceneManager.ResetCamera(true);
        }

        if (currentButtonBar.stopTimer)
        {
            combatSprite.animator.SetBool("AhlaiMiss", true);
        }
        else
        {
            combatSprite.animator.SetBool("AhlaiHit", true);
        }

        yield return WaitForAnimationComplete();

        currentButtonBar = null;

        if (health == 0)
        {
            combatSprite.animator.SetBool("Lose", true);
            yield return WaitForAnimationComplete();
        }
    }

    //enemy attacking
    private IEnumerator SlidingBarSequence(Transform positionParent, float speed)
    {
        SliderLength length = GetLengthForSlidingBar();

        currentSlidingBar = new QteSlidingBar(qteSlidingBarPrefab, positionParent, length, speed);

        yield return currentSlidingBar.MoveArrow();

        Object.Destroy(currentSlidingBar.root);

        if(positionParent.name == POSITION_A)   //going from A to C
        {
            sceneManager.StartCoroutine(sceneManager.PanCamera(sceneManager.playerCamera.transform.position.x - 3f, 0, 2f));
        }
        else if(positionParent.name == POSITION_B)  //going from B to A
        {
            sceneManager.ResetCamera(true);
        }

        if (success)
        {
            combatSprite.animator.SetBool("EnemyMiss", true);
        }
        else
        {
            combatSprite.animator.SetBool("EnemyHit", true);
            health--;
        }

        yield return WaitForAnimationComplete();

        currentSlidingBar = null;

        if (health == 0)
        {
            combatSprite.animator.SetBool("Lose", true);
            yield return WaitForAnimationComplete();
        }
    }

    private void GenerateKeysForButtonBar(int numberOfKeys)
    {
        currentKeySequence.Clear();

        for(int i = 0; i < numberOfKeys; i++)
        {
            (string key, Vector2 direction) = possibleKeys[Random.Range(0, possibleKeys.Count)];

            currentKeySequence.Add((key, direction));
        }
    }

    private SliderLength GetLengthForSlidingBar()
    {
        int count = System.Enum.GetValues(typeof(SliderLength)).Length;

        return (SliderLength) Random.Range(0, count);
    }

    public enum SliderLength
    {
        VeryShort,
        Short,
        Medium,
        Long
    }
}
