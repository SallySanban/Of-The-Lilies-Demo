using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GraphicPanels;

namespace Commands
{
    public class DatabaseExtensionGraphicPanel : CommandDatabaseExtension
    {
        private static float transitionTime = 0.5f;

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("ShowCG", new Func<string, IEnumerator>(Show));
            database.AddCommand("HideCG", new Func<IEnumerator>(Hide));
        }

        private static IEnumerator Show(string data)
        {
            GraphicPanel graphicPanel = GraphicPanelManager.Instance.GetGraphicPanel(data);

            graphicPanel.Show();

            while (graphicPanel.isCGShowing)
            {
                yield return null;
            }
        }

        private static IEnumerator Hide()
        {
            GraphicPanel currentGraphicPanel = GraphicPanelManager.Instance.activeGraphicPanel;

            currentGraphicPanel.Hide();

            while (currentGraphicPanel.isCGHiding)
            {
                yield return null;
            }
        }
    }
}

