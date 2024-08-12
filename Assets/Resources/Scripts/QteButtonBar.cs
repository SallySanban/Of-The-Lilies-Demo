using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QteButtonBar
{
    private Slider buttonBar;
    private GameObject keyParent;
    private GameObject keyTemplate;

    private float sliderTimer = 1f;
    private bool stopTimer = false;

    public GameObject root = null;

    public QteButtonBar(GameObject prefab)
    {
        if(prefab != null)
        {
            Transform sprite = GameObject.Find("Ahlai").GetComponentInChildren<Transform>();
            Vector3 spriteOffset = new Vector3(1.37f, 3f, 0f);

            Vector3 position = sprite.position + spriteOffset;

            root = Object.Instantiate(prefab, position, Quaternion.identity, sprite.parent);

            buttonBar = root.GetComponent<Slider>();
            keyParent = root.transform.Find("Keys").gameObject;
            keyTemplate = keyParent.transform.Find("Key").gameObject;
        }
    }

    //void Start()
    //{
    //    buttonBar.maxValue = sliderTimer;
    //    buttonBar.value = sliderTimer;

    //    StartTimer();
    //}

    private void StartTimer()
    {
        //StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        while(stopTimer == false)
        {
            sliderTimer -= Time.deltaTime;
            yield return new WaitForSeconds(0.05f);

            if(sliderTimer <= 0)
            {
                stopTimer = true;
                break;
            }

            buttonBar.value = sliderTimer;
        }
    }
}
