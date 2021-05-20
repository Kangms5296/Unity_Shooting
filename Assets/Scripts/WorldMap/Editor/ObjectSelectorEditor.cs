using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ObjectSelectorEditor : EditorWindow
{
    // Object Path
    public static string BrickPrefabPath = "Assets\\Prefabs\\Bricks";

    private int _colCount = 0;
    private List<GameObject> _prefabs = new List<GameObject>();

    private bool _isInit = false; 
    public void Init(string path = "", int colCount = 3)
    {
        ReadAllObject(path);

        _colCount = colCount;

        _isInit = true;
    }

    private void ReadAllObject(string path)
    {
        _prefabs.Clear();

        // path 경로의 오브젝트 캐싱
        string[] prefabPaths = Directory.GetFiles(path);
        foreach (string prefabPath in prefabPaths)
        {
            GameObject temp = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            if (temp == null)
                continue;

            _prefabs.Add(temp);
        }
    }

    Vector2 _scrollPos = Vector2.zero;
    private void OnGUI()
    {
        if (!_isInit)
            return;

        // Editor 상하 스크롤
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        
        Texture2D t2D = null;
        int count = 0;
        foreach (GameObject prefab in _prefabs)
        {
            if (count % _colCount == 0)
                EditorGUILayout.BeginHorizontal();
            count++;

            // 오브젝트 선택 버튼
            t2D = AssetPreview.GetAssetPreview(prefab);
            if (GUILayout.Button(new GUIContent(t2D, t2D.name), GUILayout.Height(80), GUILayout.Width(80)))
            {
                EditorWindow.focusedWindow.Close();
            }

            if (count % _colCount == 0)
                EditorGUILayout.EndHorizontal();
        }

        // Editor 상하 스크롤
        EditorGUILayout.EndScrollView();
    }
}
