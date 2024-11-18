//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class BackgroundManager : MonoBehaviour
//{
//    public static BackgroundManager Instance { get; private set; }

//    [SerializeField] private BackgroundConfig _config;
//    public BackgroundConfig config => _config;

//    private const string backgroundNameId = "<backgroundName>";
//    private string backgroundPrefabPath => $"Art/Backgrounds/{backgroundNameId}/{backgroundNameId}";

//    public Background currentBackground = null;


//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            DestroyImmediate(gameObject);
//        }
//    }

//    public Background GetCurrentBackground()
//    {
//        return currentBackground;
//    }

//    public Background CreateBackground(string backgroundFilename)
//    {
//        string prefabPath = FormatCGPath(backgroundPrefabPath, backgroundFilename);
//        GameObject backgroundPrefab = Resources.Load<GameObject>(prefabPath);

//        Background background = new Background(backgroundPrefab);

//        currentBackground = background;

//        return background;
//    }

//    public void RemoveCurrentBackground()
//    {
//        currentBackground.Hide();

//        currentBackground = null;
//    }

//    private string FormatCGPath(string path, string filename) => filename != "" ? path.Replace(backgroundNameId, filename) : "";
//}
