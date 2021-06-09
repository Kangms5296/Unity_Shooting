using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMonoBehaviour : MonoBehaviour
{
    GameObject _gameObject = null;
    public new GameObject gameObject
    {
        get
        {
            if (_gameObject == null)
                _gameObject = base.gameObject;

            return _gameObject;
        }
    }

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
        get => transform.position;
    }

    /// <summary>
    /// x, z Position 반환
    /// </summary>
    public Vector2 position2
    {
        get => new Vector2(position3.x, position3.z);
    }
}
