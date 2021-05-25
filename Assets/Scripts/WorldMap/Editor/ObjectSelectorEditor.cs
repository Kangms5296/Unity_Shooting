using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class ObjectSelectorEditor : EditorWindow
{
    // Object Path
    public static string BrickPrefabPath = "Assets\\Prefabs\\Bricks";

    private List<GameObject> _prefabs = new List<GameObject>();

    private int _index = 0;
    private Action<int, object> _callback = null;
    public void Init(string path = "", int index = 0, Action<int, object> callback = null)
    {
        _index = index;
        _callback = callback;

        ReadAllObject(path);

        EditorWindow.GetWindow(typeof(ObjectSelectorEditor));
    }

    private bool _readFinish = false;
    private void ReadAllObject(string path)
    {
        _readFinish = false;

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

        _readFinish = true;
    }

    Vector2 _scrollPos = Vector2.zero;
    private void OnGUI()
    {
        if (!_readFinish)
            return;

        // Editor 상하 스크롤
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        
        Texture2D t2D = null;
        int count = 0;
        foreach (GameObject prefab in _prefabs)
        {
            if (count % 3 == 0)
                EditorGUILayout.BeginHorizontal();

            // 오브젝트 선택 버튼
            t2D = AssetPreview.GetAssetPreview(prefab);
            if (t2D == null)
                continue;

            if (GUILayout.Button(new GUIContent(t2D, t2D.name), GUILayout.Height(80), GUILayout.Width(80)))
            {
                _callback(_index, _prefabs[count]);

                EditorWindow.focusedWindow.Close();
            }

            count++;
            if (count % 3 == 0)
                EditorGUILayout.EndHorizontal();
        }

        // Editor 상하 스크롤
        EditorGUILayout.EndScrollView();
    }
}
