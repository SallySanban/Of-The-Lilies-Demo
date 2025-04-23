using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QteButton
{
    public GameObject root = null;

    private Image rootImage;
    public CanvasGroup rootCanvasGroup;
    public GameObject arrow;

    public QteButton(string button, GameObject template, Transform parent, bool isFirstButton)
    {
        root = Object.Instantiate(template, parent);

        rootImage = root.GetComponent<Image>();
        rootCanvasGroup = root.GetComponent<CanvasGroup>();
        arrow = rootImage.transform.Find("Arrow").gameObject;

        string buttonPath = FilePaths.FormatPath(FilePaths.qteButtonPath, button);
        rootImage.sprite = Resources.Load<Sprite>(buttonPath);

        root.SetActive(true);

        if (isFirstButton)
        {
            arrow.SetActive(true);
        }
        else
        {
            arrow.SetActive(false);
        }

        rootCanvasGroup.alpha = 1f;
    }
}