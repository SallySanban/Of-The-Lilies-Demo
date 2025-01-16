using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditsPanel
{
    public TextMeshProUGUI creditsText = null;
    public UIManager uiManager => UIManager.Instance;

    protected Coroutine showingCreditsCoroutine, hidingCreditsCoroutine, switchingCreditsCoroutine;
    public bool isCreditsShowing => showingCreditsCoroutine != null;
    public bool isCreditsHiding => hidingCreditsCoroutine != null;
    public bool isCreditsSwitching => switchingCreditsCoroutine != null;

    private const float FADE_SPEED = 3f;

    public CreditsPanel(TextMeshProUGUI creditsText, string text)
    {
        this.creditsText = creditsText;

        creditsText.alpha = 0f;

        creditsText.gameObject.SetActive(true);

        ShowCredits(text);
    }

    public Coroutine ShowCredits(string text)
    {
        if (isCreditsShowing) return showingCreditsCoroutine;

        showingCreditsCoroutine = uiManager.StartCoroutine(ShowingHidingCredits(true, false, text));

        return showingCreditsCoroutine;
    }

    public Coroutine HideCredits()
    {
        if (isCreditsHiding) return hidingCreditsCoroutine;

        hidingCreditsCoroutine = uiManager.StartCoroutine(ShowingHidingCredits(false, true));

        return hidingCreditsCoroutine;
    }

    public Coroutine SwitchCredits(string text)
    {
        if (isCreditsSwitching) return switchingCreditsCoroutine;

        switchingCreditsCoroutine = uiManager.StartCoroutine(ShowingHidingCredits(true, true, text));

        return switchingCreditsCoroutine;
    }

    public IEnumerator ShowingHidingCredits(bool show, bool hide, string text = "")
    {
        if (hide)
        {
            while (creditsText.alpha != 0f)
            {
                creditsText.alpha = Mathf.MoveTowards(creditsText.alpha, 0f, FADE_SPEED * Time.deltaTime);

                yield return null;
            }
        }

        if (show)
        {
            creditsText.text = text;

            while (creditsText.alpha != 1f)
            {
                creditsText.alpha = Mathf.MoveTowards(creditsText.alpha, 1f, FADE_SPEED * Time.deltaTime);

                yield return null;
            }
        }
        
        showingCreditsCoroutine = null;
        switchingCreditsCoroutine = null;
        hidingCreditsCoroutine = null;
    }
}
