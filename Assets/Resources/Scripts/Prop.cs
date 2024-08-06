using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prop
{
    public string backgroundName;

    public SceneManager sceneManager => SceneManager.Instance;

    public PropManager propManager => PropManager.Instance;

    public RectTransform root = null;

    private Image rootImage;
    private CanvasGroup rootCanvasGroup;

    protected Coroutine showingPropCoroutine, hidingPropCoroutine;

    public bool isPropShowing => showingPropCoroutine != null;
    public bool isPropHiding => hidingPropCoroutine != null;

    private float fadeSpeed = 3f;

    public Prop(string imagePath, GameObject prefab)
    {
        if (!string.IsNullOrEmpty(imagePath) && prefab != null)
        {
            GameObject ob = Object.Instantiate(prefab, propManager.propContainer);

            ob.SetActive(true);

            root = ob.GetComponent<RectTransform>();

            rootImage = root.GetComponentInChildren<Image>();
            rootCanvasGroup = root.GetComponentInChildren<CanvasGroup>();

            rootImage.sprite = Resources.Load<Sprite>(imagePath);

            rootCanvasGroup.alpha = 0f;
        }
    }

    public Coroutine Show(bool immediate = false)
    {
        if (isPropShowing) return showingPropCoroutine;

        if (isPropHiding)
        {
            propManager.StopCoroutine(hidingPropCoroutine);
        }

        showingPropCoroutine = propManager.StartCoroutine(ShowingOrHiding(true, immediate));

        propManager.activeProp = this;

        return showingPropCoroutine;
    }

    public Coroutine Hide(bool immediate = false)
    {
        if (isPropHiding) return hidingPropCoroutine;

        if (isPropShowing)
        {
            propManager.StopCoroutine(showingPropCoroutine);
        }

        hidingPropCoroutine = propManager.StartCoroutine(ShowingOrHiding(false, immediate));

        return hidingPropCoroutine;
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

        showingPropCoroutine = null;
        hidingPropCoroutine = null;
    }
}
