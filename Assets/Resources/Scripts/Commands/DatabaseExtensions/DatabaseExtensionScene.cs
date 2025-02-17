using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Commands
{
    public class DatabaseExtensionScene : CommandDatabaseExtension
    {
        private static readonly string xParameter = "-x";
        private static readonly string yParameter = "-y";
        private static readonly string spdParameter = "-spd";


        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("ShowScene", new Action<string[]>(ShowScene));
            database.AddCommand("SwitchScene", new Func<string[], IEnumerator>(SwitchScene));
            database.AddCommand("RemoveScene", new Action(RemoveScene));
            database.AddCommand("ToggleInteractable", new Action<string[]>(ToggleInteractable));
            database.AddCommand("ToggleNPC", new Action<string[]>(ToggleNPC));
            database.AddCommand("ChangeAnimationState", new Action<string[]>(ChangeAnimationState));
            database.AddCommand("RemoveFromScene", new Action<string[]>(RemoveFromScene));
            database.AddCommand("MoveNPC", new Func<string[], IEnumerator>(MoveNPC));
            database.AddCommand("MovePlayerToInteract", new Func<string[], IEnumerator>(MovePlayerToInteract));
            database.AddCommand("PanCamera", new Func<string[], IEnumerator>(PanCamera));
            database.AddCommand("ResetCamera", new Action<string>(ResetCamera));
            database.AddCommand("SetCamera", new Action<string[]>(SetCamera));
            database.AddCommand("ScrollBackground", new Action<string>(ScrollBackground));
            database.AddCommand("SetNPCPosition", new Action<string[]>(SetNPCPosition));
            database.AddCommand("SetCameraFollow", new Action<string>(SetCameraFollow));
            database.AddCommand("FlipNPC", new Action<string[]>(FlipNPC));
            database.AddCommand("FollowPlayer", new Action<string>(FollowPlayer));
        }

        private static void ShowScene(string[] data)
        {
            string sceneName = data[0];
            string backgroundName = data[1];

            SceneManager.Instance.CreateScene(sceneName, backgroundName);
        }

        private static void RemoveScene()
        {
            SceneManager.Instance.RemoveScene();
        }

        private static IEnumerator SwitchScene(string[] data)
        {
            string sceneName = data[0];
            string backgroundName = data[1];

            GraphicPanel blackout = UIManager.Instance.CreateUI<GraphicPanel>("Blackout");

            yield return blackout.Show();

            SceneManager.Instance.RemoveScene();

            SceneManager.Instance.CreateScene(sceneName, backgroundName);

            yield return new WaitForSeconds(SceneManager.Instance.TRANSITION_WAIT_TIME);

            yield return blackout.Hide();
        }

        private static void FollowPlayer(string data)
        {
            SceneManager.Instance.FollowPlayer(data);
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

        private static void ToggleNPC(string[] data)
        {
            string sceneName = data[0];
            string backgroundName = data[1];
            string npcName = data[2];

            if (bool.TryParse(data[3], out bool toggle))
            {
                SceneManager.Instance.config.ChangeNPC(sceneName, backgroundName, npcName, toggle);
            }
        }

        private static void ChangeAnimationState(string[] data)
        {
            string name = data[0];
            string parameter = data[1];

            if (bool.TryParse(data[2], out bool state))
            {
                Interactable interactable = InteractableManager.Instance.GetInteractable(name);

                if (interactable == null)
                {
                    NPC npc = NPCManager.Instance.GetNPC(name);

                    npc.root.GetComponentInChildren<Animator>().SetBool(parameter, state);
                }
                else
                {
                    interactable.GetComponentInChildren<Animator>().SetBool(parameter, state);
                }
            }
        }

        private static void RemoveFromScene(string[] data)
        {
            string name = data[0];

            Interactable interactable = InteractableManager.Instance.GetInteractable(name);

            if (interactable != null)
            {
                InteractableManager.Instance.RemoveInteractable(name);
            }

            NPC npc = NPCManager.Instance.GetNPC(name);

            if (npc != null)
            {
                NPCManager.Instance.RemoveNPC(name);
            }
        }

        private static IEnumerator MoveNPC(string[] data)
        {
            string npcName = data[0];

            var parameters = ConvertDataToParameters(data);

            float x;
            float y;
            float spd;

            parameters.TryGetValue(xParameter, out x, defaultValue: 0);
            parameters.TryGetValue(yParameter, out y, defaultValue: 0);
            parameters.TryGetValue(spdParameter, out spd, defaultValue: 4);

            Vector2 position = new Vector2(x, y);

            if (npcName.Equals(SceneManager.Instance.MC_NAME))
            {
                PixelSprite player = SceneManager.Instance.player;
                GameObject root = SceneManager.Instance.player.root;

                yield return player.MoveToPosition(player, position, speed: spd);
            }
            else
            {
                NPC npc = NPCManager.Instance.GetNPC(npcName);

                yield return npc.MoveToPosition(npc, position, speed: spd);
            }
        }

        private static void SetNPCPosition(string[] data)
        {
            string npcName = data[0];

            var parameters = ConvertDataToParameters(data);

            float x;
            float y;

            parameters.TryGetValue(xParameter, out x, defaultValue: 0);
            parameters.TryGetValue(yParameter, out y, defaultValue: 0);

            Vector2 position = new Vector2(x, y);

            if (npcName.Equals(SceneManager.Instance.MC_NAME))
            {
                PixelSprite player = SceneManager.Instance.player;
                GameObject root = SceneManager.Instance.player.root;

                player.SetPosition(root, position);
            }
            else
            {
                NPC npc = NPCManager.Instance.GetNPC(npcName);

                npc.SetPosition(npc.root, position);
            }
            
        }

        private static IEnumerator MovePlayerToInteract(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            float x;
            float y;

            parameters.TryGetValue(xParameter, out x, defaultValue: 0);
            parameters.TryGetValue(yParameter, out y, defaultValue: 0);

            Vector2 position = new Vector2(x, y);

            yield return SceneManager.Instance.player.MoveToInteract(position);
        }

        private static IEnumerator PanCamera(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            float x;
            float y;
            float spd;

            parameters.TryGetValue(xParameter, out x, defaultValue: 0);
            parameters.TryGetValue(yParameter, out y, defaultValue: 0);
            parameters.TryGetValue(spdParameter, out spd, defaultValue: 0);

            yield return SceneManager.Instance.PanCamera(x, y, spd);
        }

        private static void SetCamera(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            float x;
            float y;

            parameters.TryGetValue(xParameter, out x, defaultValue: 0);
            parameters.TryGetValue(yParameter, out y, defaultValue: 0);

            Vector2 newPosition = new Vector2(x, y);

            SceneManager.Instance.SetCamera(newPosition);
        }

        private static void ResetCamera(string data)
        {
            if (bool.TryParse(data, out bool smooth))
            {
                SceneManager.Instance.ResetCamera(smooth);
            }
            
        }

        private static void ScrollBackground(string data)
        {
            if(bool.TryParse(data, out bool scroll))
            {
                SceneManager.Instance.scrollBackground = scroll;

                SceneManager.Instance.StartCoroutine(SceneManager.Instance.ScrollBackground());
            }
        }

        private static void SetCameraFollow(string data)
        {
            NPC npc = NPCManager.Instance.GetNPC(data);

            SceneManager.Instance.SetCameraFollow(npc.root.transform);
        }

        private static void FlipNPC(string[] data)
        {
            int direction = 0;

            if (data[1].Equals("Left"))
            {
                direction = -1;
            }
            else
            {
                direction = 1;
            }

            if (data[0].Equals("Ahlai"))
            {
                SceneManager.Instance.player.Flip(direction);
            }
            else
            {
                NPC npc = NPCManager.Instance.GetNPC(data[0]);

                npc.Flip(direction);
            }
        }
    }
}

