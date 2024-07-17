using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    [SerializeField] private CanvasGroup pixelScene;
    [SerializeField] private CanvasGroup vnScene;

    protected Coroutine showingVNCoroutine, hidingVNCoroutine;

    public bool isVNShowing => showingVNCoroutine != null;
    public bool isVNHiding => hidingVNCoroutine != null;

    public bool inVNMode => vnScene.alpha == 1f;

    private float fadeSpeed = 3f;

    private void Awake()
    {
        Instance = this;

        pixelScene.gameObject.SetActive(true);
        pixelScene.alpha = 0f;

        vnScene.gameObject.SetActive(true);
        vnScene.alpha = 1f;
    }

    public void SwitchToPixel()
    {
        pixelScene.alpha = 1f;

        HideVN();
    }

    public Coroutine ShowVN()
    {
        if (isVNShowing) return showingVNCoroutine;

        if (isVNHiding)
        {
            StopCoroutine(hidingVNCoroutine);
        }

        showingVNCoroutine = StartCoroutine(ShowOrHideVNScene(true));

        return showingVNCoroutine;
    }

    public Coroutine HideVN()
    {
        if (isVNHiding) return hidingVNCoroutine;

        if (isVNShowing)
        {
            StopCoroutine(showingVNCoroutine);
        }

        hidingVNCoroutine = StartCoroutine(ShowOrHideVNScene(false));

        return hidingVNCoroutine;
    }

    private IEnumerator ShowOrHideVNScene(bool show)
    {
        float targetAlpha = show ? 1f : 0f;

        CanvasGroup self = vnScene;

        while (self.alpha != targetAlpha)
        {
            self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, fadeSpeed * Time.deltaTime);

            yield return null;
        }

        showingVNCoroutine = null;
        hidingVNCoroutine = null;
    }
}
