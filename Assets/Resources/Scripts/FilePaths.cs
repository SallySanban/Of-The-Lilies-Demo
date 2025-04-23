using UnityEngine;

public class FilePaths
{
    private static readonly string id = "<id>";

    public static readonly string root = $"{Application.dataPath}/Game Data/";

    //Runtime Paths
    public static readonly string gameSaves = $"{runtimePath}Save Files/";

    //Resources Paths
    public static readonly string storyPath = $"Story/";
    public static string portraitRootPath => $"Art/Portraits/{id}";
    public static string portraitPrefabPath => $"{portraitRootPath}/Character - [{id}]";
    public static string portraitAssetsPath = $"{id}/Images";
    public static string graphicPanelImagesPath => $"Art/CG/Images/{id}";
    public static string backgroundPrefabPath => $"Art/Backgrounds/{id}/{id}";
    public static string spritesPrefabPath => $"Art/Sprites/{id}/{id}";
    public static string combatPrefabPath => $"Art/UI/Combat/{id}";
    public static string qteButtonPath => $"Art/UI/Combat/QTE/Images/{id}";
    public static string qteBarPrefabPath => $"Art/UI/Combat/QTE/{id}";

    public static string runtimePath
    {
        get
        {
            #if UNITY_EDITOR
                return "Assets/appdata/";
            #else
                return Application.persistentDataPath + "/appdata/";
            #endif
        }
    }

    public static string FormatPath(string path, string textToReplace) => path.Replace(id, textToReplace);

    public static GameObject GetPrefabFromPath(string prefabPath, string filename)
    {
        string path = FormatPath(prefabPath, filename);

        return Resources.Load<GameObject>(path);
    }
}
