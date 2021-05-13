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
        GUILayout.BeginArea(new Rect(20, 20, 540, 190));

        Color initBoxTextColor = GUI.skin.box.normal.textColor;
        Color initBtnTextColor = GUI.skin.button.normal.textColor;
        Color initBgColor = GUI.backgroundColor;

        GUI.skin.box.normal.textColor = Color.white;
        GUI.backgroundColor = new Color(0.2f, 0, 0, 0.3f);
        GUI.Box(new Rect(20, 20, 520, 170), "Object Menu");

        GUI.skin.button.normal.textColor = Color.white;

        // Object List
        int brickCount = ((_grid._brickList?.Count ?? 0) > 5) ? 5 : (_grid._brickList?.Count ?? 0);
        Texture2D t2D = null;
        for (int index = 0; index < 5; ++index)
        {
            int brickIndex = index % brickCount;

            t2D = AssetPreview.GetAssetPreview(_grid._brickList[brickIndex]);
            GUI.DrawTexture(new Rect(40 + 100 * index, 45, 80, 80), t2D);

            if (GUI.Button(new Rect(40 + 100 * index, 130, 80, 20), "선택"))
            {

            }

            if (GUI.Button(new Rect(40 + 100 * index, 155, 80, 20), "변경"))
            {

            }
        }

        GUI.skin.box.normal.textColor = initBoxTextColor;
        GUI.skin.button.normal.textColor = initBtnTextColor;
        GUI.backgroundColor = initBgColor;

        GUILayout.EndArea();
        UnityEditor.Handles.EndGUI();
    }
    #endregion
}