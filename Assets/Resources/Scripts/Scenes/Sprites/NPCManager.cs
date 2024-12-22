using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager
{
    public static NPCManager Instance { get; private set; }

    private SceneManager sceneManager => SceneManager.Instance;

    private List<NPC> npcsInScreen = new List<NPC>();

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

    public NPCManager()
    {
        Instance = this;
    }

    public void SwitchNPCs()
    {
        NPCData[] npcDataInScene = sceneManager.config.GetNPCsInScene(sceneManager.currentSceneName, sceneManager.currentBackground);

        foreach(NPC npc in npcsInScreen)
        {
            foreach(NPCData npcData in npcDataInScene)
            {
                if(npc.name.Equals(npcData.npcName))
                {
                    bool positionChanged = (npc.lastPosition != npcData.position) && (npcData.position != Vector2.zero);
                    bool animationChanged = (npc.lastAnimationState != npcData.animationState) && (!string.IsNullOrEmpty(npcData.animationState));

                    if (positionChanged)
                    {
                        npc.lastPosition = npcData.position;
                    }

                    if(animationChanged) 
                    {
                        npc.lastAnimationState = npcData.animationState;
                    }

                    npc.SetupNPC(npc.lastPosition, npc.lastAnimationState);
                }
            }
        }
    }

    public void RemoveAllNPCs()
    {
        foreach (NPC npc in npcsInScreen)
        {
            Object.Destroy(npc.root);
        }

        npcsInScreen.Clear();
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
