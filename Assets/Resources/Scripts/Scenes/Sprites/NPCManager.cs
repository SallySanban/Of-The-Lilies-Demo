using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager
{
    public static NPCManager Instance { get; private set; }

    private SceneManager sceneManager => SceneManager.Instance;

    private List<NPC> npcsInScreen = new List<NPC>();

    public NPCManager()
    {
        Instance = this;
    }

    public void PopulateNPCs(Transform parent)
    {
        npcsInScreen.Clear();

        NPCData[] npcDataInScene = sceneManager.config.GetNPCsInScene(sceneManager.currentSceneName, sceneManager.currentBackground);

        foreach (NPCData npcData in npcDataInScene)
        {
            GameObject npcPrefab = FilePaths.GetPrefabFromPath(FilePaths.spritesPrefabPath, npcData.npcName);

            if (npcPrefab != null)
            {
                NPC npc = new NPC(npcPrefab, parent, npcData);

                npcsInScreen.Add(npc);
            }
        }
    }

    public void RemoveNPC(string name)
    {
        foreach (NPC npc in npcsInScreen)
        {
            if (npc.name.Equals(name))
            {
                Object.Destroy(npc.root);
                npcsInScreen.Remove(npc);
                sceneManager.config.RemoveNPCFromScene(sceneManager.currentSceneName, sceneManager.currentBackground, name);
                break;
            }
        }
    }

    public NPC GetNPC(string npcName)
    {
        foreach(NPC npc in npcsInScreen)
        {
            if (npc.name.Equals(npcName))
            {
                return npc;
            }
        }

        return null;
    }
}
