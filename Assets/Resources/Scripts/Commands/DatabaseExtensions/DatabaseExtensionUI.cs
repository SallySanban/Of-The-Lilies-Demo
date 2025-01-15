using System;
using System.Collections;
using UnityEngine;

namespace Commands
{
    public class DatabaseExtensionUI : CommandDatabaseExtension
    {
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("Blackout", new Func<IEnumerator>(Blackout));
            database.AddCommand("ShowCG", new Func<string, IEnumerator>(ShowCG));
            database.AddCommand("HideCG", new Func<IEnumerator>(HideCG));
            database.AddCommand("SwitchCG", new Func<string, IEnumerator>(SwitchCG));
        }

        private static IEnumerator Blackout()
        {
            if (UIManager.Instance.currentCG == null)
            {
                yield return ShowCG("Blackout");
            }
            else
            {
                yield return SwitchCG("Blackout");
            }
        }

        private static IEnumerator ShowCG(string data)
        {
            GraphicPanel graphicPanel = UIManager.Instance.CreateUI<GraphicPanel>(filename: data);

            graphicPanel.Show();

            while (graphicPanel.isCGShowing)
            {
                yield return null;
            }
        }

        private static IEnumerator HideCG()
        {
            GraphicPanel currentGraphicPanel = UIManager.Instance.currentCG;

            currentGraphicPanel.Hide();

            while (currentGraphicPanel.isCGHiding)
            {
                yield return null;
            }
        }

        private static IEnumerator SwitchCG(string data)
        {
            GraphicPanel newGraphicPanel = UIManager.Instance.CreateUI<GraphicPanel>(filename: data);
            GraphicPanel currentGraphicPanel = UIManager.Instance.currentCG;

            newGraphicPanel.Show(true);

            while (newGraphicPanel.isCGShowing)
            {
                yield return null;
            }

            currentGraphicPanel.Hide();

            while (currentGraphicPanel.isCGHiding)
            {
                yield return null;
            }

            UIManager.Instance.currentCG = newGraphicPanel;
        }
    }
}

