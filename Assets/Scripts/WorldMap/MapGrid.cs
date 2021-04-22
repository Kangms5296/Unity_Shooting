using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapGrid : CustomMonoBehaviour
{
    [Header("Grid Info")]
    [SerializeField] private GameObject _gridAreaObject = null;
    [SerializeField] private int _halfXSize = 10;
    [SerializeField] private int _halfZSize = 10;

    [Header("Cell Info")]
    [SerializeField] [Range(1, 10)] private int _cellSize = 1;
    [SerializeField] private Color _color = Color.white;

    private bool _startMapEditor = false;

    private const string _gridAreaObjectName = "GridAreaObject";

    public int CellSize
    {
        get => _cellSize;
    }

    /// <summary>
    /// Map Editor 시작
    /// </summary>
    public void StartMapEditor()
    {
        if (_startMapEditor)
            return;

        // Base Touch Area 생성
        if (_gridAreaObject != null)
        {
            GameObject temp = Instantiate(_gridAreaObject, transform);

            temp.transform.position = transform.position + new Vector3(0, -0.1f, 0);
            temp.transform.localScale = new Vector3(_halfXSize * 0.2f, 1, _halfZSize * 0.2f);
            temp.name = _gridAreaObjectName;
        }

        _startMapEditor = true;
    }

    /// <summary>
    /// Map Editor 종료
    /// </summary>
    public void EndMapEditor()
    {
        if (!_startMapEditor)
            return;

        // Base Touch Area 제거
        if (transform.Find(_gridAreaObjectName) != null)
            DestroyImmediate(transform.Find(_gridAreaObjectName).gameObject);

        _startMapEditor = false;
    }

    public Vector3 CalGridPosition(Vector3 pos)
    {
        Vector3 newPos = new Vector3(Mathf.Floor(pos.x / _cellSize) * _cellSize + _cellSize / 2f,
            transform.position.y, Mathf.Floor(pos.z / _cellSize) * _cellSize + _cellSize / 2f);

        return newPos;
    }

    /// <summary>
    /// 클릭 위치에 Brick 유무 확인 및 생성 혹은 제거
    /// </summary>
    /// <param name="newPos"></param>
    /// <param name="destoryObject"></param>
    public void CheckCompareObject(Vector3 newPos, bool destoryObject)
    {

    }

#if UNITY_EDITOR

    private void OnGUI()
    {
        DrawBrickMenu(null);
    }

    private void OnEnable()
    {
        UnityEditor.SceneView.onSceneGUIDelegate -= DrawBrickMenu;
        UnityEditor.SceneView.onSceneGUIDelegate += DrawBrickMenu;
    }

    private void OnDisable()
    {
        UnityEditor.SceneView.onSceneGUIDelegate -= DrawBrickMenu;
    }

    private void DrawBrickMenu(UnityEditor.SceneView view)
    {
        new UnityEditor.SerializedObject(this);

        UnityEditor.Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(20, 20, 500, 80));

        // 배경 GUI
        GUI.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.5f);
        GUI.skin.box.normal.textColor = Color.white;
        GUI.Box(new Rect(20, 20, 500, 80), "Blick Menu");

        GUILayout.EndArea();
        UnityEditor.Handles.EndGUI();
    }

    void OnDrawGizmosSelected()
    {
        if (_startMapEditor == false)
            return;

        if (_cellSize <= 0
            || _halfXSize < 0
            || _halfZSize < 0)
            return;

        Gizmos.color = _color;

        // 중앙 위치
        Vector3 pos = new Vector3(Mathf.Floor(transform.position.x / _cellSize) * _cellSize
            , Mathf.Floor(transform.position.y / _cellSize) * _cellSize
            , Mathf.Floor(transform.position.z / _cellSize) * _cellSize);

        // Vertical Grid 표시
        for (float z = pos.z - _halfZSize; z <= pos.z + _halfZSize; z += _cellSize)
        {
            Gizmos.DrawLine(new Vector3(_halfXSize + pos.x, pos.y, Mathf.Floor(z / _cellSize) * _cellSize),
            new Vector3(-_halfXSize + pos.x, pos.y, Mathf.Floor(z / _cellSize) * _cellSize));
        }

        // Horizontal Grid 표시
        for (float x = pos.x - _halfXSize - 1; x < pos.x + _halfXSize; x += _cellSize)
        {
            Gizmos.DrawLine(new Vector3(Mathf.Floor(x / _cellSize) * _cellSize + _cellSize, pos.y, _halfZSize + pos.z),
            new Vector3(Mathf.Floor(x / _cellSize) * _cellSize + _cellSize, pos.y, -_halfZSize + pos.z));
        }
    }
#endif
}
