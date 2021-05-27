﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGrid))]
public class MapGridEditor : Editor
{
    private MapGrid _grid = null;

    private void OnEnable()
    {
        _grid = (MapGrid)target;
    }

    #region Inspector GUI
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Start Map Editor"))
            StartMapEditor();
        if (GUILayout.Button("End Map Editor"))
            EndMapEditor();
        EditorGUILayout.EndHorizontal();

    }

    private void StartMapEditor()
    {
        _grid.StartMapEditor();
    }

    private void EndMapEditor()
    {
        _grid.EndMapEditor();
    }
    #endregion

    #region Scene GUI
    void OnSceneGUI()
    {
        CheckCreateObejct();

        DrawObjectMenuGUI();
    }

    /// <summary>
    /// 오브젝트 생성 입력 확인 및 오브젝트 생성
    /// </summary>
    private void CheckCreateObejct()
    {
        Event e = Event.current;

        // 마우스 좌표 계산
        var mousePosition = Event.current.mousePosition * EditorGUIUtility.pixelsPerPoint;
        mousePosition.y = Camera.current.pixelHeight - mousePosition.y;

        var Ray = Camera.current.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(Ray, out RaycastHit hit))
        {
            // Grid는 마우스 클릭 Event만 처리
            if (e.type == EventType.MouseDown ||
                e.type == EventType.MouseDrag)
            {
                if (e.control)
                {
                    // Brick 생성
                    if (e.button == 0)
                    {
                        Debug.Log("Brick 생성 : " + _grid.CalGridPosition(hit.point));


                    }
                    // Brick 제거
                    else if (e.button == 1)
                    {
                        Debug.Log("Brick 제거 : " + _grid.CalGridPosition(hit.point));


                    }

                    // Hierachy Focus를 Grid로 유지
                    int crtID = GUIUtility.GetControlID(FocusType.Passive);
                    GUIUtility.hotControl = crtID;

                    // Event 사용 처리
                    Event.current.Use();
                }
            }
        }
    }

    /// <summary>
    /// Object Menu GUI 표시
    /// </summary>
    private void DrawObjectMenuGUI()
    {
        if (!_grid.MapEditorStart)
            return;

        UnityEditor.Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(20, 20, 100 * _grid.ObjectCacheCount + 40, 190));

        Color initBoxTextColor = GUI.skin.box.normal.textColor;
        Color initBtnTextColor = GUI.skin.button.normal.textColor;
        Color initBgColor = GUI.backgroundColor;

        GUI.skin.box.normal.textColor = Color.white;
        GUI.backgroundColor = new Color(0.2f, 0, 0, 0.3f);
        GUI.Box(new Rect(20, 20, 100 * _grid.ObjectCacheCount + 20, 170), "Object Menu");

        GUI.skin.button.normal.textColor = Color.white;

        // Object List
        Texture2D t2D = null;
        for (int index = 0; index < _grid.ObjectCacheCount; ++index)
        {
            // 캐시 오브젝트 선택
            if (GUI.Button(new Rect(40 + 100 * index, 130, 80, 20), "선택"))
            {
                if (_grid._cachedObjects[index] != null)
                    _grid._curSelectIndex = index;
            }
            // 캐시 오브젝트 변경
            if (GUI.Button(new Rect(40 + 100 * index, 155, 80, 20), "변경"))
            {
                // 새 Object를 Cache 목록에 올린다.
                ObjectSelectorEditor selector = (ObjectSelectorEditor)EditorWindow.GetWindow(typeof(ObjectSelectorEditor), true, "Object Selector");
                selector.Init(ObjectSelectorEditor.BrickPrefabPath, index, (valueIndex, value) =>
                {
                    // 현재 목록의 이전 선택 정보를 삭제
                    if (_grid._curSelectIndex == valueIndex)
                        _grid._curSelectIndex = -1;

                    _grid._cachedObjects[valueIndex] = (GameObject)value;

                    EditorUtility.SetDirty(_grid);
                });
            }

            if (_grid._cachedObjects.Count <= index || _grid._cachedObjects[index] == null)
                continue;

            t2D = AssetPreview.GetAssetPreview(_grid._cachedObjects[index]);
            if (t2D != null)
                GUI.DrawTexture(new Rect(40 + 100 * index, 45, 80, 80), t2D);
        }

        // 선택한 캐시 오브젝트 표시
        if (_grid._curSelectIndex != -1 && _grid._cachedObjects[_grid._curSelectIndex] != null )
            GUI.DrawTexture(new Rect(40 + 100 * _grid._curSelectIndex, 40, 5, 5), Texture2D.whiteTexture);
        
        GUI.skin.box.normal.textColor = initBoxTextColor;
        GUI.skin.button.normal.textColor = initBtnTextColor;
        GUI.backgroundColor = initBgColor;

        GUILayout.EndArea();
        UnityEditor.Handles.EndGUI();
    }
    #endregion
}