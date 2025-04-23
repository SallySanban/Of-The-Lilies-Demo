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

    public CombatManager()
    {
        Instance = this;
    }

    public void Initialize(GameObject prefab, Transform parent, Vector2 position)
    {
        combatSprite = new CombatSprite(prefab, parent, position);

        qteButtonBarPrefab = FilePaths.GetPrefabFromPath(FilePaths.qteBarPrefabPath, BUTTONBAR_FILENAME);

        qteSlidingBarPrefab = FilePaths.GetPrefabFromPath(FilePaths.qteBarPrefabPath, SLIDINGBAR_FILENAME);

        sceneManager.StartCoroutine(StartCombat());
    }

    private IEnumerator StartCombat()
    {
        Vector2 positionA = combatSprite.root.transform.Find(POSITION_A).transform.position;
        Vector2 positionB = combatSprite.root.transform.Find(POSITION_B).transform.position;
        Vector2 positionC = combatSprite.root.transform.Find(POSITION_C).transform.position;

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
        combatSprite.animator.SetBool("EnemyMiss", true);

        //position C to A
        yield return ButtonBarSequence(positionC, 3, 0.05f);

        //position A to B
        yield return ButtonBarSequence(positionA, 4, 0.05f);

        //position B to A
        yield return SlidingBarSequence(positionB, 2f);

        


    }

    //ahlai attacking
    private IEnumerator ButtonBarSequence(Vector2 position, int numberOfKeys, float speed)
    {
        currentButtonIndex = 0;

        GenerateKeysForButtonBar(numberOfKeys);

        currentButtonBar = new QteButtonBar(qteButtonBarPrefab, position, currentKeySequence.Select(item => item.Item1).ToList(), speed);

        yield return currentButtonBar.Timer();

        Object.Destroy(currentButtonBar.root);

        if (currentButtonBar.stopTimer)
        {
            combatSprite.animator.SetBool("AhlaiMiss", true);
            //loss += 1;
        }
        else
        {
            combatSprite.animator.SetBool("AhlaiHit", true);
            //wins += 1;
        }

        currentButtonBar = null;

        //if (count == 5)
        //{
        //    if (wins > loss)
        //    {
        //        combatSprite.SetBool("Win", true);
        //    }
        //    else
        //    {
        //        combatSprite.SetBool("Lose", true);
        //    }

        //    yield break;
        //}

        //count += 1;
    }

    //enemy attacking
    private IEnumerator SlidingBarSequence(Vector2 position, float speed)
    {
        SliderLength length = GetLengthForSlidingBar();

        currentSlidingBar = new QteSlidingBar(qteSlidingBarPrefab, position, length, speed);

        yield return currentSlidingBar.MoveArrow();

        bool success = currentSlidingBar.CheckForSuccess();

        if (success)
        {
            combatSprite.animator.SetBool("EnemyMiss", true);
            //wins += 1;
        }
        else
        {
            combatSprite.animator.SetBool("EnemyHit", true);
            //loss += 1;

        }

        Object.Destroy(currentSlidingBar.root);

        currentSlidingBar = null;
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
