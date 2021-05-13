using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ObjectSelectorEditor : EditorWindow
{
    public List<GameObject> _prefabs = new List<GameObject>();

    public void Init(string path = "", int rows = 2, int width = 100, int height = 100)
    {
        ReadAllObject(path);
    }

    private void ReadAllObject(string path)
    {
        string[] prefabPaths = Directory.GetFiles(path);
        foreach (string prefabPath in prefabPaths)
        {
            GameObject temp = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            if (temp == null)
                continue;

            _prefabs.Add(temp);
        }
    }

    private void OnGUI()
    {
        
    }
}
