using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMonoBehaviour : MonoBehaviour
{
    Transform _transform = null;
    public new Transform transform
    {
        get
        {
            if (_transform == null)
                _transform = base.transform;

            return _transform;
        }
    }

    /// <summary>
    /// x, y, z Position 반환
    /// </summary>
    public Vector3 position3
    {
        get
        {
            if (_transform == null)
                _transform = transform;

            return _transform.position;
        }
    }

    /// <summary>
    /// x, z Position 반환
    /// </summary>
    public Vector2 position2
    {
        get => new Vector2(position3.x, position3.z);
    }
}
