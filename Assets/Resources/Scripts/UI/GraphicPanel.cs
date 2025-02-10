using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicPanel
{
    public GameObject root;

    private CanvasGroup rootCanvasGroup;
    private Image rootImage;

    public UIManager uiManager => UIManager.Instance;

    protected Coroutine showingCGCoroutine, hidingCGCoroutine;
    public bool isCGShowing => showingCGCoroutine != null;
    public bool isCGHiding => hidingCGCoroutine != null;

    private const float FADE_SPEED = 3f;

    public GraphicPanel(string imagePath, GameObject prefab, bool blackout = false)
    {
        if (!string.IsNullOrEmpty(imagePath) && prefab != null)
        {
            root = Object.Instantiate(prefab, uiManager.graphicsContainer);

            if (LIScreen.liScreenVisible)
            {
                root.transform.SetSiblingIndex(1);
            }
            else
            {
                root.transform.SetSiblingIndex(0);
            }
            
            root.SetActive(true);

            rootImage = root.GetComponentInChildren<Image>();
            rootCanvasGroup = root.GetComponentInChildren<CanvasGroup>();

            if (!blackout)
            {
                rootImage.sprite = Resources.Load<Sprite>(imagePath);
                rootImage.color = Color.white;
            }
            else
            {
                rootImage.sprite = null;
                rootImage.color = Color.black;
            }

            rootCanvasGroup.alpha = 0f;
        }
    }

    public Coroutine Show(bool immediate = false)
    {
        if (isCGShowing) return showingCGCoroutine;

        if (isCGHiding)
        {
            uiManager.StopCoroutine(hidingCGCoroutine);
        }

        showingCGCoroutine = uiManager.StartCoroutine(ShowingOrHiding(true, immediate));

        uiManager.currentCG = this;

        return showingCGCoroutine;
    }

    public Coroutine Hide(bool immediate = false)
    {
        if (isCGHiding) return hidingCGCoroutine;

        if (isCGShowing)
        {
            uiManager.StopCoroutine(showingCGCoroutine);
        }

        hidingCGCoroutine = uiManager.StartCoroutine(ShowingOrHiding(false, immediate));

        UIManager.Instance.currentCG = null;

        return hidingCGCoroutine;
    }

    public IEnumerator ShowingOrHiding(bool show, bool immediate)
    {
        float targetAlpha = show ? 1f : 0f;

        CanvasGroup self = rootCanvasGroup;

        if (immediate)
        {
            self.alpha = targetAlpha;

            if (self.alpha == 0f)
            {
                Object.Destroy(self.gameObject);
            }
        }
        else
        {
            while (self.alpha != targetAlpha)
            {
                self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, FADE_SPEED * Time.deltaTime);

                if (self.alpha == 0f)
                {
                    Object.Destroy(self.gameObject);
                    break;
                }

                yield return null;
            }
        }

        showingCGCoroutine = null;
        hidingCGCoroutine = null;
    }
}
