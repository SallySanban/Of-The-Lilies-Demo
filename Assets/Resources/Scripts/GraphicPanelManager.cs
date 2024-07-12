using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GraphicPanels
{
    public class GraphicPanelManager : MonoBehaviour
    {
        public static GraphicPanelManager Instance { get; private set; }

        public GraphicPanel activeGraphicPanel = null;

        private const string graphicPanelNameId = "<graphicPanelName>";
        private string graphicPanelRootPath => $"Art/CG/Images/{graphicPanelNameId}";
        private string graphicPanelPrefabPath => "Art/CG/Graphic Panel";

        [SerializeField] private RectTransform _graphicPanelContainer = null;
        public RectTransform graphicPanelContainer => _graphicPanelContainer;

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

        public GraphicPanel GetGraphicPanel(string graphicPanelFilename)
        {
            return CreateGraphicPanel(graphicPanelFilename);
        }

        public GraphicPanel CreateGraphicPanel(string graphicPanelFilename)
        {
            string graphicPanelImagePath = FormatCGPath(graphicPanelRootPath, graphicPanelFilename);
            GameObject graphicPanelPrefab = Resources.Load<GameObject>(graphicPanelPrefabPath);

            bool blackout = graphicPanelFilename == "Blackout" ? true : false;

            GraphicPanel graphicPanel = new GraphicPanel(graphicPanelImagePath, graphicPanelPrefab, blackout);

            return graphicPanel;
        }

        private string FormatCGPath(string path, string filename) => filename != "" ? path.Replace(graphicPanelNameId, filename) : "";
    }
}

