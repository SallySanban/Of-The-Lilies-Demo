using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Background
{
    public string backgroundName;

    public SceneManager sceneManager => SceneManager.Instance;
    public BackgroundManager backgroundManager => BackgroundManager.Instance;

    public GameObject root = null;

    public CanvasGroup rootCanvasGroup;

    protected Coroutine showingBackgroundCoroutine, hidingBackgroundCoroutine;

    public bool isBackgroundShowing => showingBackgroundCoroutine != null;
    public bool isBackgroundHiding => hidingBackgroundCoroutine != null;

    private float fadeSpeed = 3f;

    public Background(GameObject prefab)
    {
        if (prefab != null)
        {
            root = Object.Instantiate(prefab, sceneManager.pixelSceneContainer);

            root.SetActive(true);

            rootCanvasGroup = root.GetComponent<CanvasGroup>();

            rootCanvasGroup.alpha = 0f;

            backgroundName = prefab.name;

            //Debug.Log("PREFAB NAME: " + backgroundName);
        }
    }

    public Coroutine Show(bool immediate = false)
    {
        if (isBackgroundShowing) return showingBackgroundCoroutine;

        if (isBackgroundHiding)
        {
            backgroundManager.StopCoroutine(hidingBackgroundCoroutine);
        }

        showingBackgroundCoroutine = backgroundManager.StartCoroutine(ShowingOrHiding(true, immediate));

        backgroundManager.currentBackground = this;

        return showingBackgroundCoroutine;
    }

    public Coroutine Hide(bool immediate = false)
    {
        if (isBackgroundHiding) return hidingBackgroundCoroutine;

        if (isBackgroundShowing)
        {
            backgroundManager.StopCoroutine(showingBackgroundCoroutine);
        }

        hidingBackgroundCoroutine = backgroundManager.StartCoroutine(ShowingOrHiding(false, immediate));

        return hidingBackgroundCoroutine;
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

        showingBackgroundCoroutine = null;
        hidingBackgroundCoroutine = null;
    }
}
