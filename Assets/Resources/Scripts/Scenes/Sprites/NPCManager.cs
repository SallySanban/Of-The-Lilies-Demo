using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager
{
    private SceneManager sceneManager => SceneManager.Instance;

    private List<NPC> npcsInScreen = new List<NPC>();

    public void PopulateNPCs(Transform parent)
    {
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

            //TODO: what abt if npc got removed the next scene?
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
