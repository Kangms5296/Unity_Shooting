using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : CustomMonoBehaviour
{
    [Header("Cell Info")]
    [SerializeField] private int _halfVertical = 10;
    [SerializeField] private int _halfHorizontal = 10;

    [Header("Cell Info")]
    [SerializeField] private int _cellSize = 1;
    [SerializeField] private Color _color = Color.white;

    void OnDrawGizmos()
    {  
        if (_cellSize <= 0
            || _halfVertical < 0
            || _halfHorizontal < 0)
            return;
        Gizmos.color = _color;

        // 중앙 위치
        Vector3 pos = new Vector3(Mathf.Floor(transform.position.x / _cellSize) * _cellSize
            , Mathf.Floor(transform.position.y / _cellSize) * _cellSize
            , Mathf.Floor(transform.position.z / _cellSize) * _cellSize);

        // Vertical Grid 표시
        for (float z = pos.z - _halfVertical; z <= pos.z + _halfVertical; z += _cellSize)
        {
            Gizmos.DrawLine(new Vector3(_halfHorizontal + pos.x, pos.y, Mathf.Floor(z / _cellSize) * _cellSize),
            new Vector3(-_halfHorizontal + pos.x, pos.y, Mathf.Floor(z / _cellSize) * _cellSize));
        }

        // Horizontal Grid 표시
        for (float x = pos.x - _halfHorizontal - 1; x < pos.x + _halfHorizontal; x += _cellSize)
        {
            Gizmos.DrawLine(new Vector3(Mathf.Floor(x / _cellSize) * _cellSize + _cellSize, pos.y, _halfVertical + pos.z),
            new Vector3(Mathf.Floor(x / _cellSize) * _cellSize + _cellSize, pos.y, -_halfVertical + pos.z));
        }
    }
}
