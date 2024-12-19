using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private RectTransform _graphicsContainer = null;
    [SerializeField] private GameObject inputPanelPrefab;
    [SerializeField] private GameObject graphicPanelPrefab;
    public RectTransform graphicsContainer => _graphicsContainer;

    public InputPanel inputPanel;
    public GraphicPanel currentCG;

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

    public T CreateUI<T>(string filename = "") where T : class
    {
        if (typeof(T) == typeof(InputPanel))
        {
            inputPanel = new InputPanel(inputPanelPrefab);

            return inputPanel as T;
        }
        else if(typeof(T) == typeof(GraphicPanel))
        {
            string graphicPanelImagePath = FilePaths.FormatPath(FilePaths.graphicPanelImagesPath, filename);

            bool blackout = filename == "Blackout" ? true : false;

            GraphicPanel graphicPanel = new GraphicPanel(graphicPanelImagePath, graphicPanelPrefab, blackout);

            return graphicPanel as T;
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
}
