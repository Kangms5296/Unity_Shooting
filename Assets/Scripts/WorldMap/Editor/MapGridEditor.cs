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
        Event e = Event.current;

        // Grid는 마우스 클릭 Event만 처리
        if (e.type != EventType.MouseDown &&
            e.type != EventType.MouseDrag)
            return;

        // 마우스 좌표 계산
        var mousePosition = Event.current.mousePosition * EditorGUIUtility.pixelsPerPoint;
        mousePosition.y = Camera.current.pixelHeight - mousePosition.y;

        var Ray = Camera.current.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(Ray, out RaycastHit hit))
        {
            if (e.control)
            {
                // Brick 생성
                if (e.button == 0)
                {
                    Debug.Log("Brick 생성 : "+ _grid.CalGridPosition(hit.point));


                }
                // Brick 제거
                else if (e.button == 1)
                {
                    Debug.Log("Brick 제거 : " + _grid.CalGridPosition(hit.point));


                }
            }

            // Hierachy Focus를 Grid로 유지
            int crtID = GUIUtility.GetControlID(FocusType.Passive);
            GUIUtility.hotControl = crtID;

            // Event 사용 처리
            Event.current.Use();
        }
    }
}