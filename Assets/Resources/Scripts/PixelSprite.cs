using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelSprite
{
    public SceneManager sceneManager => SceneManager.Instance;
    public SpriteManager spriteManager => SpriteManager.Instance;

    public RectTransform root = null;

    public CanvasGroup rootCanvasGroup;

    protected Coroutine showingSpriteCoroutine, hidingSpriteCoroutine;

    public bool isSpriteShowing => showingSpriteCoroutine != null;
    public bool isSpriteHiding => hidingSpriteCoroutine != null;

    private float fadeSpeed = 3f;

    public PixelSprite(GameObject prefab, Transform backgroundSpriteIsOn)
    {
        if (prefab != null)
        {
            GameObject ob = Object.Instantiate(prefab, backgroundSpriteIsOn);

            ob.SetActive(true);

            root = ob.GetComponent<RectTransform>();

            rootCanvasGroup = root.GetComponent<CanvasGroup>();

            rootCanvasGroup.alpha = 0f;
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

        CanvasGroup self = rootCanvasGroup;

        if (immediate)
        {
            self.alpha = targetAlpha;
        }
        else
        {
            while (self.alpha != targetAlpha)
            {
                self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, fadeSpeed * Time.deltaTime);

                if (self.alpha == 0f)
                {
                    Object.Destroy(self.gameObject);
                    break;
                }

                yield return null;
            }
        }

        showingSpriteCoroutine = null;
        hidingSpriteCoroutine = null;
    }
}
