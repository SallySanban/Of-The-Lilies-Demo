using System.Collections;
using UnityEngine;
using FMODUnity;

public class QteSlidingBar
{
    private SceneManager sceneManager => SceneManager.Instance;

    private const string SUCCESS_ZONE = "Success Zone";
    private const string ARROW = "Arrow";
    private const string FULL_BAR = "Full Bar";
    private const string RIGHT_POINT = "Right Point";
    private const string LEFT_POINT = "Left Point";

    private RectTransform successZone;
    private RectTransform fullBar;
    private GameObject arrow;
    private GameObject leftPoint;
    private GameObject rightPoint;

    private float speed;
    private Vector3 targetArrowPosition;
    public bool stopArrow = false;

    public GameObject root = null;

    private const float minZoneWidth = 350f;
    private const float maxZoneWidth = 1000f;

    public QteSlidingBar(GameObject prefab, Transform positionTransform, CombatManager.SliderLength sliderLength, float speed)
    {
        if (prefab != null)
        {
            root = Object.Instantiate(prefab, positionTransform.position, Quaternion.identity, positionTransform);

            fullBar = root.transform.Find(FULL_BAR).GetComponent<RectTransform>();
            successZone = fullBar.Find(SUCCESS_ZONE).GetComponent<RectTransform>();
            arrow = root.transform.Find(ARROW).gameObject;
            leftPoint = root.transform.Find(LEFT_POINT).gameObject;
            rightPoint = root.transform.Find(RIGHT_POINT).gameObject;

            this.speed = speed;

            SetupSlidingBar(sliderLength);
        }
    }

    private void SetupSlidingBar(CombatManager.SliderLength sliderLength)
    {
        targetArrowPosition = rightPoint.transform.position;

        float barWidth = fullBar.rect.width;

        float zoneWidth = Random.Range(minZoneWidth, maxZoneWidth);
        zoneWidth = Mathf.Min(zoneWidth, barWidth);

        float maxStart = barWidth - zoneWidth;
        float startPos = Random.Range(0, maxStart);

        successZone.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, zoneWidth);
        successZone.anchoredPosition = new Vector2(startPos, successZone.anchoredPosition.y);

        successZone.GetComponent<BoxCollider2D>().size = new Vector2(successZone.rect.width, successZone.rect.height);
        successZone.GetComponent<BoxCollider2D>().offset = successZone.rect.center;
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

        yield return new WaitForSeconds(0.5f);
    }

    public bool CheckForSuccess()
    {
        BoxCollider2D sliderCollider = successZone.GetComponent<BoxCollider2D>();
        Collider2D arrowCollider = Physics2D.OverlapBox(sliderCollider.bounds.center, sliderCollider.bounds.size, 0f);

        if (arrowCollider != null && arrowCollider.CompareTag("Arrow"))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_CombatCorrectBar"); //FMOD correct sfx
            return true;
        }
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SFX_CombatWrongBar"); //FMOD wrong sound sfx
        return false;
    }

    
}