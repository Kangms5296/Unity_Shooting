using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : CustomMonoBehaviour
{
    [Header("Grid Info")]
    [SerializeField] private float _width = 1f;
    [SerializeField] private float _height = 1f;
    [SerializeField] private Color _color = Color.white;

    void OnDrawGizmos()
    {  
        if (_width <= 0 || _height <= 0)
            return;

        Gizmos.color = _color;

        // 현재 scene view 에서 보고 있는 카메라 좌표를 가져옵니다.
        Vector3 pos = transform.position;
        for (float z = pos.z - 100f; z < pos.z + 100f; z += _height)
        {
            Gizmos.DrawLine(new Vector3(100f + pos.x, pos.y, Mathf.Floor(z / _height) * _height),
            new Vector3(-100f + pos.x, pos.y, Mathf.Floor(z / _height) * _height));
        }

        for (float x = pos.x - 100f; x < pos.x + 100f; x += _width)
        {
            Gizmos.DrawLine(new Vector3(Mathf.Floor(x / _width) * _width, pos.y, 100 + pos.z),
            new Vector3(Mathf.Floor(x / _width) * _width, pos.y, -100 + pos.z));
        }
    }
}
