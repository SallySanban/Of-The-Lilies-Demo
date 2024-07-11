using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GraphicPanels
{
    public class GraphicPanelManager : MonoBehaviour
    {
        public static GraphicPanelManager Instance { get; private set; }

        private Dictionary<string, GraphicPanel> graphicPanels = new Dictionary<string, GraphicPanel>();

        public GraphicPanel activeGraphicPanel = null;

        private const string graphicPanelNameId = "<graphicPanelName>";
        private string graphicPanelRootPath => $"Art/CG/{graphicPanelNameId}";

        [SerializeField] private Image _graphicPanelContainer = null;
        public Image graphicPanelContainer => _graphicPanelContainer;

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
            if (graphicPanels.ContainsKey(graphicPanelFilename.ToLower()))
            {
                return graphicPanels[graphicPanelFilename.ToLower()];
            }
            else
            {
                return CreateGraphicPanel(graphicPanelFilename);
            }
        }

        public GraphicPanel CreateGraphicPanel(string graphicPanelFilename)
        {
            if (graphicPanels.ContainsKey(graphicPanelFilename.ToLower()))
            {
                Debug.LogError($"{graphicPanelFilename} already exists");
                return null;
            }

            string graphicPanelPath = FormatCGPath(graphicPanelRootPath, graphicPanelFilename);

            GraphicPanel graphicPanel = new GraphicPanel(graphicPanelFilename, graphicPanelPath);

            graphicPanels.Add(graphicPanelFilename.ToLower(), graphicPanel);

            return graphicPanel;
        }

        private string FormatCGPath(string path, string filename) => path.Replace(graphicPanelNameId, filename);
    }
}

