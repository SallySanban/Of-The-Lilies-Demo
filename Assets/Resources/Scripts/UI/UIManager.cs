using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private RectTransform _graphicsContainer = null;
    [SerializeField] private GameObject inputPanelPrefab;
    [SerializeField] private GameObject graphicPanelPrefab;
    [SerializeField] public TextMeshProUGUI creditsText;
    public RectTransform graphicsContainer => _graphicsContainer;

    public InputPanel inputPanel;
    public GraphicPanel currentCG;
    public CreditsPanel creditsPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public T CreateUI<T>(string data = "") where T : class
    {
        if (typeof(T) == typeof(InputPanel))
        {
            inputPanel = new InputPanel(inputPanelPrefab);

            return inputPanel as T;
        }
        else if(typeof(T) == typeof(GraphicPanel))
        {
            string graphicPanelImagePath = FilePaths.FormatPath(FilePaths.graphicPanelImagesPath, data);

            bool blackout = data == "Blackout" ? true : false;

            GraphicPanel graphicPanel = new GraphicPanel(graphicPanelImagePath, graphicPanelPrefab, blackout);

            return graphicPanel as T;
        }
        else if(typeof(T) == typeof(CreditsPanel))
        {
            creditsPanel = new CreditsPanel(creditsText, data);

            return creditsPanel as T;
        }
        else
        {
            throw new System.ArgumentException($"Unsupported type: {typeof(T)}");
        }
    }

    public void DestroyUI<T>(T uiElement)
    {
        if (typeof(T) == typeof(InputPanel))
        {
            Object.DestroyImmediate(inputPanel.root);
        }
    }

    public void ShowCredits(string text)
    {

    }
}
