using UnityEngine;

public class FilePaths
{
    public static readonly string root = $"{Application.dataPath}/Game Data/";

    //Runtime Paths
    public static readonly string gameSaves = $"{runtimePath}Save Files/";

    //Resources Paths
    public static readonly string storyFiles = $"Story/";
    public static readonly string textboxPrefabs = $"UI/Textbox/";

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

    public static string FormatPath(string path, string filename) => path + filename;
}
