using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace GraphicPanels
{
    public class GraphicPanel
    {
        public GraphicPanelManager manager => GraphicPanelManager.Instance;

        public RectTransform root = null;

        private Image rootImage;
        private CanvasGroup rootCanvasGroup;

        protected Coroutine showingCGCoroutine, hidingCGCoroutine;

        public bool isCGShowing => showingCGCoroutine != null;
        public bool isCGHiding => hidingCGCoroutine != null;

        private float fadeSpeed = 3f;

        public GraphicPanel(string imagePath, GameObject prefab, bool blackout = false)
        {
            if (!string.IsNullOrEmpty(imagePath) && prefab != null)
            {
                GameObject ob = Object.Instantiate(prefab, manager.graphicPanelContainer);

                ob.transform.SetSiblingIndex(0);

                ob.SetActive(true);

                root = ob.GetComponent<RectTransform>();

                rootImage = root.GetComponentInChildren<Image>();
                rootCanvasGroup = root.GetComponentInChildren<CanvasGroup>();

                if (!blackout)
                {
                    rootImage.sprite = Resources.Load<Sprite>(imagePath);
                    rootImage.color = Color.white;
                }
                else
                {
                    rootImage.sprite = null;
                    rootImage.color = Color.black;
                }

                rootCanvasGroup.alpha = 0f;
            }
        }

        public Coroutine Show(bool immediate = false)
        {
            if (isCGShowing) return showingCGCoroutine;

            if (isCGHiding)
            {
                manager.StopCoroutine(hidingCGCoroutine);
            }

            showingCGCoroutine = manager.StartCoroutine(ShowingOrHiding(true, immediate));

            manager.activeGraphicPanel = this;

            return showingCGCoroutine;
        }

        public Coroutine Hide(bool immediate = false)
        {
            if (isCGHiding) return hidingCGCoroutine;

            if (isCGShowing)
            {
                manager.StopCoroutine(showingCGCoroutine);
            }

            hidingCGCoroutine = manager.StartCoroutine(ShowingOrHiding(false, immediate));

            return hidingCGCoroutine;
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
           
            showingCGCoroutine = null;
            hidingCGCoroutine = null;
        }
    }
}

