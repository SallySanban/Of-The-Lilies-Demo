//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using System;
//using UnityEditor.Animations;

//public class CombatTest : MonoBehaviour
//{
//    public static CombatTest Instance { get; private set; }

//    [SerializeField] public Animator combatSprite;

//    private string qteSlidingBarPrefabPath => $"Testing/QTE Sliding Bar";
//    private string qteButtonBarPrefabPath => $"Testing/QTE Button Bar";

//    private int currentStep = 0;

//    private QteButtonBar qteButtonBar;
//    private QteSlidingBar qteSlidingBar;

//    private float waitAnimation = 1f;
//    private float playerHealth;

//    public bool waitingResult;

//    private int wins = 0;
//    private int loss = 0;
//    private int count = 0;

//    //TODO: IMPROVE IMPLEMENTATION FOR MIX AND MATCH QTES
//    Dictionary<(string combatSceneName, string triggerName), List<(string keys, float speed)>> buttonBarKeys = new Dictionary<(string sceneName, string triggerName), List<(string keys, float speed)>>
//    {
//        { ("Prologue", "Trigger 1"), new List<(string keys, float speed)>
//            {
//                ("Up Up Down Down", 0.05f),
//                ("Left Right Left", 0.03f),
//                ("Up Down Left", 0.01f)
//            }
//        },
//        { ("Prologue", "Trigger 2"), new List<(string keys, float speed)>
//            {
//                ("Right Left Down Up Up", 0.05f),
//                ("Up Up Left Right Down", 0.03f),
//                ("Down Right Left Left Right", 0.01f)
//            }
//        }
//    };

//    Dictionary<(string combatSceneName, string triggerName), List<(QteSlidingBar.SliderLength length, float speed)>> slidingBarSlider = new Dictionary<(string sceneName, string triggerName), List<(QteSlidingBar.SliderLength length, float speed)>>
//    {
//        { ("Prologue", "Trigger 1"), new List<(QteSlidingBar.SliderLength length, float speed)>
//            {
//                (QteSlidingBar.SliderLength.Long, 2f),
//                (QteSlidingBar.SliderLength.Medium, 3f),
//                (QteSlidingBar.SliderLength.Medium, 4f),
//                (QteSlidingBar.SliderLength.Short, 4f),
//            }
//        },
//        { ("Prologue", "Trigger 2"), new List<(QteSlidingBar.SliderLength length, float speed)>
//            {
//                (QteSlidingBar.SliderLength.VeryShort, 5f)
//            }
//        }
//    };

//    private KeyCode[] keySequence = null;
//    private bool currentlyButtonBar = false;
//    private bool currentlySlidingBar = false;

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            DestroyImmediate(gameObject);
//        }
//    }

//    private void Update()
//    {
//        if (currentlyButtonBar)
//        {
//            if (currentStep <= keySequence.Length - 1)
//            {
//                if (Input.GetKeyDown(keySequence[currentStep]))
//                {
//                    if (currentStep == keySequence.Length - 1)
//                    {
//                        qteButtonBar.allButtonsCorrect = true;
//                        currentStep = 0;
//                    }
//                    else
//                    {
//                        currentStep++;
//                        qteButtonBar.NextButton();
//                    }
//                }
//                else if (AnyKeyBut(keySequence[currentStep]))
//                {
//                    currentStep = 0;
//                    qteButtonBar.ResetButton();
//                }
//            }
//        }
//        else if (currentlySlidingBar)
//        {
//            if (Input.GetKeyDown(KeyCode.Space))
//            {
//                qteSlidingBar.stopArrow = true;
//            }
//        }

        
//        if (Input.GetKeyDown(KeyCode.Z))
//        {
//            StartCoroutine(StartCombat("Button"));
//        }

//        if (Input.GetKeyDown(KeyCode.X))
//        {
//            StartCoroutine(StartCombat("Sliding"));
//        }

//        int STEPS = 2;
//        int SPEED = 3;

//        if (Input.GetKey(KeyCode.D))
//        {
//            combatSprite.SetBool("Forward", true);

//            Vector3 movement = new Vector3(1 * STEPS, 0, 0) * SPEED * Time.deltaTime;
//            combatSprite.transform.position += movement;
//        }
//        else
//        {
//            combatSprite.SetBool("Forward", false);
//        }

//        if (Input.GetKey(KeyCode.A))
//        {
//            combatSprite.SetBool("Back", true);

//            Vector3 movement = new Vector3(-1 * STEPS, 0, 0) * SPEED * Time.deltaTime;
//            combatSprite.transform.position += movement;
//        }
//        else
//        {
//            combatSprite.SetBool("Back", false);
//        }
//    }

//    private IEnumerator StartCombat(string type)
//    {
//        System.Random rand = new System.Random();


//        if (type == "Button")
//        {
//            yield return ButtonBarSequence("Prologue", "Trigger " + rand.Next(1, 3));
//        }
//        else
//        {
//            yield return SlidingBarSequence("Prologue", "Trigger " + rand.Next(1, 3));
//        }
//    }

//    [ContextMenu("ShowQTEPosition")]
//    public void ShowQtePosition()
//    {
//        if (qteButtonBar != null) Debug.Log("BUTTON BAR: " + qteButtonBar.root.transform.position);
//        if (qteSlidingBar != null) Debug.Log("SLIDING BAR: " + qteSlidingBar.root.transform.position);
//        if (qteSlidingBar != null) Debug.Log("SLIDING BAR SLIDER: " + qteSlidingBar.slider.anchoredPosition);
//    }

//    [ContextMenu("ShowSliderSize")]
//    public void ShowSliderSize()
//    {
//        if (qteSlidingBar != null) Debug.Log("SIZE SLIDER: " + qteSlidingBar.slider.sizeDelta);
//    }

//    private IEnumerator ButtonBarSequence(string combatSceneName, string triggerName)
//    {
//        GameObject buttonBarPrefab = Resources.Load<GameObject>(qteButtonBarPrefabPath);

//        var key = (combatSceneName, triggerName);

//        waitingResult = false;

//        if (buttonBarKeys.TryGetValue(key, out List<(string keys, float speed)> value))
//        {
//            foreach (var item in value)
//            {
//                //yield return WaitForAnimation();

//                (string keys, float speed) = item;

//                string[] buttonSequence = keys.Split(" ");
//                qteButtonBar = new QteButtonBar(buttonBarPrefab, buttonSequence, speed);

//                keySequence = new KeyCode[buttonSequence.Length];

//                for (int i = 0; i < buttonSequence.Length; i++)
//                {
//                    keySequence[i] = ConvertStringToKeyCode(buttonSequence[i]);
//                }

//                currentlyButtonBar = true;
//                yield return qteButtonBar.Timer();

//                Destroy(qteButtonBar.root);

//                if (count == 5)
//                {
//                    if (wins > loss)
//                    {
//                        combatSprite.SetBool("Win", true);
//                    }
//                    else
//                    {
//                        combatSprite.SetBool("Lose", true);
//                    }

//                    yield break;
//                }

//                if (qteButtonBar.stopTimer)
//                {
//                    combatSprite.SetBool("EnemyDodge", true);
//                    loss += 1;
//                }
//                else
//                {
//                    combatSprite.SetBool("EnemyHit", true);
//                    wins += 1;
//                }

//                waitingResult = true;

//                currentlyButtonBar = false;

//                count += 1;

//                yield break;
//            }
//        }
//    }

//    private IEnumerator SlidingBarSequence(string combatSceneName, string triggerName)
//    {
//        GameObject slidingBarPrefab = Resources.Load<GameObject>(qteSlidingBarPrefabPath);

//        var key = (combatSceneName, triggerName);
//        if (slidingBarSlider.TryGetValue(key, out List<(QteSlidingBar.SliderLength length, float speed)> value))
//        {
//            foreach (var item in value)
//            {
//                (QteSlidingBar.SliderLength length, float speed) = item;

//                qteSlidingBar = new QteSlidingBar(slidingBarPrefab, length, speed);

//                currentlySlidingBar = true;
//                yield return qteSlidingBar.MoveArrow();

//                bool success = qteSlidingBar.CheckForSuccess();

//                Destroy(qteSlidingBar.root);

//                if (count == 5)
//                {
//                    if (wins > loss)
//                    {
//                        combatSprite.SetBool("Win", true);
//                    }
//                    else
//                    {
//                        combatSprite.SetBool("Lose", true);
//                    }

//                    yield break;
//                }

//                if (success)
//                {
//                    combatSprite.SetBool("AhlaiDodge", true);
//                    wins += 1;
//                }
//                else
//                {
//                    combatSprite.SetBool("AhlaiHit", true);
//                    loss += 1;

//                }

//                waitingResult = true;

//                currentlySlidingBar = false;

//                count += 1;

//                yield break;
//            }
//        }
//    }

//    private IEnumerator WaitForAnimation()
//    {
//        while (waitingResult)
//        {
//            yield return null;
//        }
//    }

//    private KeyCode ConvertStringToKeyCode(string key)
//    {
//        switch (key)
//        {
//            case "Up":
//                return KeyCode.UpArrow;
//            case "Down":
//                return KeyCode.DownArrow;
//            case "Left":
//                return KeyCode.LeftArrow;
//            case "Right":
//                return KeyCode.RightArrow;
//            default:
//                Debug.LogError("Invalid key string: " + key);
//                return KeyCode.None;
//        }
//    }

//    private bool AnyKeyBut(KeyCode expectedKey)
//    {
//        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
//        {
//            if (Input.GetKeyDown(keyCode) && keyCode != expectedKey)
//            {
//                return true;
//            }
//        }
//        return false;
//    }
//}
