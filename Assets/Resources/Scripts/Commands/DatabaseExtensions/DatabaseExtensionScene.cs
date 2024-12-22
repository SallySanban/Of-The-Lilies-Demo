using System;
using System.Collections;
using UnityEngine;

namespace Commands
{
    public class DatabaseExtensionScene : CommandDatabaseExtension
    {
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("ShowScene", new Action<string[]>(ShowScene));
            database.AddCommand("SwitchScene", new Action<string>(SwitchScene));
            database.AddCommand("SwitchBackground", new Func<string[], IEnumerator>(SwitchBackground));
            database.AddCommand("ToggleInteractable", new Action<string[]>(ToggleInteractable));
            database.AddCommand("ChangeAnimationState", new Action<string[]>(ChangeAnimationState));
            database.AddCommand("RemoveFromScene", new Action<string[]>(RemoveFromScene));
        }

        private static void ShowScene(string[] data)
        {
            string sceneName = data[0];
            string backgroundName = data[1];

            SceneManager.Instance.CreateScene(sceneName, backgroundName);
        }

        private static void SwitchScene(string data)
        {
            SceneManager.Instance.SwitchScene(data);
        }

        private static IEnumerator SwitchBackground(string[] data)
        {
            string sceneName = data[0];
            string backgroundName = data[1];

            GraphicPanel blackout = UIManager.Instance.CreateUI<GraphicPanel>("Blackout");

            yield return blackout.Show();

            SceneManager.Instance.RemoveScene();

            SceneManager.Instance.CreateScene(sceneName, backgroundName);

            yield return blackout.Hide();
        }

        private static void ToggleInteractable(string[] data)
        {
            string sceneName = data[0];
            string backgroundName = data[1];
            string interactableName = data[2];

            if (bool.TryParse(data[3], out bool toggle))
            {
                SceneManager.Instance.config.ChangeInteractable(sceneName, backgroundName, interactableName, toggle);

                Interactable currentInteractable = InteractableManager.Instance.GetInteractable(interactableName);

                if (currentInteractable != null)
                {
                    currentInteractable.stateChangedDuringStory = true;
                }

                InteractableManager.Instance.RefreshInteractables();
            }
        }

        private static void ChangeAnimationState(string[] data)
        {
            string name = data[0];
            string state = data[1];

            Interactable interactable = InteractableManager.Instance.GetInteractable(name);

            if (interactable == null)
            {
                NPC npc = NPCManager.Instance.GetNPC(name);

                npc.root.GetComponentInChildren<Animator>().SetTrigger(state);
            }
            else
            {
                interactable.GetComponentInChildren<Animator>().SetTrigger(state);
            }
        }

        private static void RemoveFromScene(string[] data)
        {
            string name = data[0];

            Interactable interactable = InteractableManager.Instance.GetInteractable(name);

            if (interactable == null)
            {
                NPCManager.Instance.RemoveNPC(name);
            }
            else
            {
                InteractableManager.Instance.RemoveInteractable(name);
            }
        }
    }
}

