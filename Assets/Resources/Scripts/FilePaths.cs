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
}
