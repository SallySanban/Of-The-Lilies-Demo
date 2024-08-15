using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GraphicPanels;

public class LIScreenButton : Button, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject colouredButton;

    private bool thanksShown = false;
    private Coroutine currentCoroutine = null;

    private new void Start()
    {
        colouredButton.SetActive(false);

        onClick.AddListener(ShowCredits);
    }

    public new void OnPointerEnter(PointerEventData eventData)
    {
        colouredButton.SetActive(true);
    }

    public new void OnPointerExit(PointerEventData eventData)
    {
        colouredButton.SetActive(false);
    }

    private void ShowCredits()
    {
        foreach(Button button in transform.parent.GetComponentsInChildren<Button>())
        {
            button.interactable = false;
        }

        if (currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(ShowCreditsCoroutine());
        }
    }

    private IEnumerator ShowCreditsCoroutine()
    {
        transform.parent.GetComponent<CanvasGroup>().alpha = 0f;
        SceneManager.Instance.vnScene.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        yield return SceneManager.Instance.ShowVN();

        GraphicPanel graphicPanel = GraphicPanelManager.Instance.GetGraphicPanel("Credits");

        graphicPanel.Show();

        while (graphicPanel.isCGShowing)
        {
            yield return null;
        }

        yield return ListenForInput();
    }

    private IEnumerator ShowThanksCoroutine()
    {
        GraphicPanel newCG = GraphicPanelManager.Instance.GetGraphicPanel("Thanks");
        GraphicPanel currentCG = GraphicPanelManager.Instance.activeGraphicPanel;

        currentCG.Hide();

        while (currentCG.isCGHiding)
        {
            yield return null;
        }

        newCG.Show();

        while (newCG.isCGShowing)
        {
            yield return null;
        }
    }

    private IEnumerator ListenForInput()
    {
        while (!Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.Z) && !thanksShown)
        {
            yield return null;
        }

        thanksShown = true;
        yield return ShowThanksCoroutine();
    }
}
