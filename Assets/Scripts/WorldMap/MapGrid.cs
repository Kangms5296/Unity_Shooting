using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : CustomMonoBehaviour
{
    [Header("Grid Info")]
    [SerializeField] private GameObject _gridAreaObject;
    [SerializeField] private int _halfXSize = 10;
    [SerializeField] private int _halfZSize = 10;

    [Header("Cell Info")]
    [SerializeField] private int _cellSize = 1;
    [SerializeField] private Color _color = Color.white;

    private bool _startMapEditor = false;

    private const string _gridAreaObjectName = "GridAreaObject";

    public int CellSize
    {
        get => _cellSize;
    }

    public void StartMapEditor()
    {
        if (_startMapEditor)
            return;

        // Base Touch Area 생성
        if (_gridAreaObject != null)
        {
            GameObject temp = Instantiate(_gridAreaObject, transform);

            temp.transform.position = new Vector3(0, -0.1f, 0);
            temp.transform.localScale = new Vector3(_halfXSize * 0.2f, 1, _halfZSize * 0.2f);
            temp.name = _gridAreaObjectName;
        }

        _startMapEditor = true;
    }

    public void EndMapEditor()
    {
        if (!_startMapEditor)
            return;

        // Base Touch Area 제거
        if (transform.Find(_gridAreaObjectName) != null)
            DestroyImmediate(transform.Find(_gridAreaObjectName).gameObject);

        _startMapEditor = false;
    }

#if UNITY_EDITOR
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
