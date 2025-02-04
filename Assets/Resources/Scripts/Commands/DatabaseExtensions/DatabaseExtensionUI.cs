using System;
using System.Collections;
using System.Data.Common;
using UnityEngine;

namespace Commands
{
    public class DatabaseExtensionUI : CommandDatabaseExtension
    {
        private static string immediateParameter => "-i";


        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("Blackout", new Func<string[], IEnumerator>(Blackout));
            database.AddCommand("ShowCG", new Func<string, IEnumerator>(ShowCG));
            database.AddCommand("HideCG", new Func<IEnumerator>(HideCG));
            database.AddCommand("SwitchCG", new Func<string, IEnumerator>(SwitchCG));
            database.AddCommand("ShowCredits", new Func<string, IEnumerator>(ShowCredits));
            database.AddCommand("SwitchCredits", new Func<string, IEnumerator>(SwitchCredits));
            database.AddCommand("HideCredits", new Func<IEnumerator>(HideCredits));
        }

        private static IEnumerator Blackout(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            bool immediate;

            parameters.TryGetValue(immediateParameter, out immediate, defaultValue: false);

            if (immediate)
            {
                if (UIManager.Instance.currentCG == null)
                {
                    GraphicPanel graphicPanel = UIManager.Instance.CreateUI<GraphicPanel>("Blackout");

                    graphicPanel.Show(true);

                    while (graphicPanel.isCGShowing)
                    {
                        yield return null;
                    }
                }
                else
                {
                    GraphicPanel newGraphicPanel = UIManager.Instance.CreateUI<GraphicPanel>("Blackout");
                    GraphicPanel currentGraphicPanel = UIManager.Instance.currentCG;

                    newGraphicPanel.Show(true);

                    while (newGraphicPanel.isCGShowing)
                    {
                        yield return null;
                    }

                    currentGraphicPanel.Hide(true);

                    while (currentGraphicPanel.isCGHiding)
                    {
                        yield return null;
                    }

                    UIManager.Instance.currentCG = newGraphicPanel;
                }
            }
            else
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
        }

        private static IEnumerator ShowCG(string data)
        {
            GraphicPanel graphicPanel = UIManager.Instance.CreateUI<GraphicPanel>(data);

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
            GraphicPanel newGraphicPanel = UIManager.Instance.CreateUI<GraphicPanel>(data);
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

        private static IEnumerator ShowCredits(string data)
        {
            yield return UIManager.Instance.CreateUI<CreditsPanel>(data);
        }

        private static IEnumerator HideCredits()
        {
            yield return UIManager.Instance.creditsPanel.HideCredits();
        }

        private static IEnumerator SwitchCredits(string data)
        {
            yield return UIManager.Instance.creditsPanel.SwitchCredits(data);
        }
    }
}

