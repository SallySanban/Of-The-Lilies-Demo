//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PropManager : MonoBehaviour
//{
//    public static PropManager Instance { get; private set; }

//    public Prop activeProp = null;

//    private const string propNameId = "<propName>";
//    private string propRootPath => $"Art/UI/Props/Images/{propNameId}";
//    private string propPrefabPath => "Art/UI/Props/Prop";

//    [SerializeField] private RectTransform _propContainer = null;
//    public RectTransform propContainer => _propContainer;


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

//    public Prop GetProp(string propFilename)
//    {
//        return CreateProp(propFilename);
//    }

//    private Prop CreateProp(string propFilename)
//    {
//        string propImagePath = FormatCGPath(propRootPath, propFilename);
//        GameObject propPrefab = Resources.Load<GameObject>(propPrefabPath);

//        Prop prop = new Prop(propImagePath, propPrefab);

//        return prop;
//    }

//    private string FormatCGPath(string path, string filename) => filename != "" ? path.Replace(propNameId, filename) : "";
//}
