using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace GraphicPanels
{
    public class GraphicPanel
    {
        public string filename = "";

        public GraphicPanelManager manager => GraphicPanelManager.Instance;

        public Image root => manager.graphicPanelContainer;

        private CanvasGroup graphicPanelCanvasGroup => root.GetComponent<CanvasGroup>();

        protected Coroutine showingCGCoroutine, hidingCGCoroutine;

        public bool isCGShowing => showingCGCoroutine != null;
        public bool isCGHiding => hidingCGCoroutine != null;

        public GraphicPanel(string graphicPanelFilename, string graphicPanelPath)
        {
            this.filename = graphicPanelFilename;

            if (!string.IsNullOrEmpty(graphicPanelFilename))
            {
                root.sprite = Resources.Load<Sprite>(graphicPanelPath);
                graphicPanelCanvasGroup.alpha = 0f;
            }
        }

        public Coroutine Show()
        {
            if (isCGShowing) return showingCGCoroutine;

            if (isCGHiding)
            {
                manager.StopCoroutine(hidingCGCoroutine);
            }

            showingCGCoroutine = manager.StartCoroutine(ShowingOrHiding(true));

            manager.activeGraphicPanel = this;

            return showingCGCoroutine;
        }

        public Coroutine Hide()
        {
            if (isCGHiding) return hidingCGCoroutine;

            if (isCGShowing)
            {
                manager.StopCoroutine(showingCGCoroutine);
            }

            hidingCGCoroutine = manager.StartCoroutine(ShowingOrHiding(false));

            return hidingCGCoroutine;
        }

        public IEnumerator ShowingOrHiding(bool show)
        {
            float targetAlpha = show ? 1f : 0f;

            CanvasGroup self = graphicPanelCanvasGroup;

            while (self.alpha != targetAlpha)
            {
                self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, 3f * Time.deltaTime);

                yield return null;
            }

            showingCGCoroutine = null;
            hidingCGCoroutine = null;
        }
    }
}

