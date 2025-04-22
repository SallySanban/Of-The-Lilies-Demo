using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class QteSlidingBar
{
    public RectTransform slider;
    private GameObject arrow;
    private GameObject leftPoint;
    private GameObject rightPoint;

    private float speed;
    private Vector3 targetArrowPosition;
    public bool stopArrow = false;

    public GameObject root = null;

    public QteSlidingBar(GameObject prefab, SliderLength sliderLength, float speed)
    {
        if (prefab != null)
        {
            root = Object.Instantiate(prefab, new Vector2(3f, 3f), Quaternion.identity, CombatTest.Instance.combatSprite.transform.parent);

            slider = root.transform.Find("Slider").GetComponent<RectTransform>();
            arrow = root.transform.Find("Arrow").gameObject;
            leftPoint = root.transform.Find("Left Point").gameObject;
            rightPoint = root.transform.Find("Right Point").gameObject;

            this.speed = speed;

            SetupSlidingBar(sliderLength);
        }
    }

    private void SetupSlidingBar(SliderLength sliderLength)
    {
        targetArrowPosition = rightPoint.transform.position;

        float[] lengths = { -214.27f, -92.70f, 48.72f, 229.84f };
        float[] maxPosition = { 358.42f, 335.4f, 301.03f, 260.9f };

        slider.sizeDelta = new Vector2(lengths[(int)sliderLength], slider.sizeDelta.y);

        float sliderPosition = Random.Range(0, maxPosition[(int)sliderLength]);

        slider.anchoredPosition = new Vector2(sliderPosition, slider.position.y);

        slider.GetComponent<BoxCollider2D>().size = new Vector2(slider.rect.width, slider.rect.height);
        slider.GetComponent<BoxCollider2D>().offset = slider.rect.center;
    }

    public IEnumerator MoveArrow()
    {
        while (stopArrow == false)
        {
            arrow.transform.position = Vector3.MoveTowards(arrow.transform.position, targetArrowPosition, speed * Time.deltaTime);

            if (Vector3.Distance(arrow.transform.position, rightPoint.transform.position) < 0.1f)
            {
                targetArrowPosition = leftPoint.transform.position;
            }
            else if (Vector3.Distance(arrow.transform.position, leftPoint.transform.position) < 0.1f)
            {
                targetArrowPosition = rightPoint.transform.position;
            }

            yield return null;
        }
    }

    public bool CheckForSuccess()
    {
        BoxCollider2D sliderCollider = slider.GetComponent<BoxCollider2D>();
        Collider2D arrowCollider = Physics2D.OverlapBox(sliderCollider.bounds.center, sliderCollider.bounds.size, 0f);

        if (arrowCollider != null && arrowCollider.CompareTag("Arrow"))
        {
            return true;
        }

        return false;
    }

    public enum SliderLength
    {
        VeryShort,
        Short,
        Medium,
        Long
    }
}