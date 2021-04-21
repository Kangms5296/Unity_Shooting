using System.Collections;
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

    private void OnDisable()
    {
        _grid.EndMapEditor();
    }

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

    void OnSceneGUI()
    {
        // Grid는 마우스 클릭 Event만 처리
        if (Event.current.type != EventType.MouseDown || Event.current.button != 0)
            return;

        /*
        if (Event.current.modifiers != EventModifiers.Control)
            return;
        */
        var mousePosition = Event.current.mousePosition * EditorGUIUtility.pixelsPerPoint;
        mousePosition.y = Camera.current.pixelHeight - mousePosition.y;

        var Ray = Camera.current.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(Ray, out RaycastHit hit))
        {
            Debug.Log(hit.point);



            // Hierachy Focus를 Grid로 유지
            int crtID = GUIUtility.GetControlID(FocusType.Passive);
            GUIUtility.hotControl = crtID;

            // Event 사용 처리
            Event.current.Use();
        }
    }
}