using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelSprite
{
    public SceneManager sceneManager => SceneManager.Instance;
    public SpriteManager spriteManager => SpriteManager.Instance;

    public Transform root = null;

    public SpriteRenderer rootSpriteRenderer;

    protected Coroutine showingSpriteCoroutine, hidingSpriteCoroutine;

    public bool isSpriteShowing => showingSpriteCoroutine != null;
    public bool isSpriteHiding => hidingSpriteCoroutine != null;

    private float fadeSpeed = 3f;

    public PixelSprite(GameObject prefab, Vector2 spritePosition, BackgroundConfigData.PlayerDirection spriteDirection, Transform backgroundSpriteIsOn)
    {
        if (prefab != null)
        {
            GameObject ob = Object.Instantiate(prefab, backgroundSpriteIsOn);

            ob.SetActive(true);

            root = ob.GetComponent<Transform>();

            rootSpriteRenderer = root.GetComponentInChildren<SpriteRenderer>();

            Color spriteColor = rootSpriteRenderer.color;
            spriteColor.a = 0f;
            rootSpriteRenderer.color = spriteColor;

            SetPositionDirection(spritePosition, spriteDirection);
        }
    }

    public void SetPositionDirection(Vector2 position, BackgroundConfigData.PlayerDirection direction)
    {
        if (root == null) return;

        root.transform.position = position;

        if (direction == BackgroundConfigData.PlayerDirection.left)
        {
            root.Find("Sprite").transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (direction == BackgroundConfigData.PlayerDirection.right)
        {
            root.Find("Sprite").transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    public Coroutine Show(bool immediate = false)
    {
        if (isSpriteShowing) return showingSpriteCoroutine;

        if (isSpriteHiding)
        {
            spriteManager.StopCoroutine(hidingSpriteCoroutine);
        }

        showingSpriteCoroutine = spriteManager.StartCoroutine(ShowingOrHiding(true, immediate));

        return showingSpriteCoroutine;
    }

    public Coroutine Hide(bool immediate = false)
    {
        if (isSpriteHiding) return hidingSpriteCoroutine;

        if (isSpriteShowing)
        {
            spriteManager.StopCoroutine(showingSpriteCoroutine);
        }

        hidingSpriteCoroutine = spriteManager.StartCoroutine(ShowingOrHiding(false, immediate));

        return hidingSpriteCoroutine;
    }

    public IEnumerator ShowingOrHiding(bool show, bool immediate)
    {
        float targetAlpha = show ? 1f : 0f;

        Color spriteColor = rootSpriteRenderer.color;

        if (immediate)
        {
            spriteColor.a = targetAlpha;
        }
        else
        {
            while (spriteColor.a != targetAlpha)
            {
                spriteColor.a = Mathf.MoveTowards(spriteColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
                rootSpriteRenderer.color = spriteColor;

                if (spriteColor.a == 0f)
                {
                    Object.Destroy(rootSpriteRenderer.transform.parent.gameObject);
                    break;
                }

                yield return null;
            }
        }

        showingSpriteCoroutine = null;
        hidingSpriteCoroutine = null;
    }
}
