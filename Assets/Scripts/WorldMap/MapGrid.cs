using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class MapGrid : CustomMonoBehaviour
{
    [Header("Grid Info")]
    [SerializeField] private GameObject _gridAreaObject = null;
    [SerializeField] private int _halfXSize = 10;
    [SerializeField] private int _halfZSize = 10;
    [SerializeField] private Color _color = Color.white;

    [Header("Cell Info")]
    [SerializeField] [Range(1, 10)] private int _cellSize = 1;

    [Header("Object Info")]
    [SerializeField] [Range(1, 8)] private int _objectCacheCount = 5;
    public int ObjectCacheCount => _objectCacheCount;

    [HideInInspector] public int _curSelectIndex = -1;
    [HideInInspector] public List<GameObject> _cachedObjects = null;

    private const string _gridAreaObjectName = "GridAreaObject";

    private Dictionary<Vector3, EditingObject> _objectDic = new Dictionary<Vector3, EditingObject>();

    public bool MapEditorStart { get; private set; } = false;

    /// <summary>
    /// Map Editor 시작
    /// </summary>
    public void StartMapEditor()
    {
        if (MapEditorStart)
            return;

        // Base Touch Area 생성
        if (_gridAreaObject != null)
        {
            GameObject temp = Instantiate(_gridAreaObject, transform);

            temp.transform.position = transform.position + new Vector3(0, -0.1f, 0);
            temp.transform.localScale = new Vector3(_halfXSize * _cellSize * 0.2f, 1, _halfZSize * _cellSize * 0.2f);
            temp.name = _gridAreaObjectName;
        }

        // Guide Object 생성
        if (_curSelectIndex != -1)
            OnChangeGuideObject(_cachedObjects[_curSelectIndex]);

        MapEditorStart = true;
    }

    /// <summary>
    /// Map Editor 종료
    /// </summary>
    public void EndMapEditor()
    {
        if (!MapEditorStart)
            return;

        // Base Touch Area 제거
        if (transform.Find(_gridAreaObjectName) != null)
            DestroyImmediate(transform.Find(_gridAreaObjectName).gameObject);

        MapEditorStart = false;
    }

    /// <summary>
    /// GridAreaObject 재생성
    /// </summary>
    public void RegenerateGridAreaObject()
    {
        // Base Touch Area 제거
        if (transform.Find(_gridAreaObjectName) != null)
            DestroyImmediate(transform.Find(_gridAreaObjectName).gameObject);

        // Base Touch Area 생성
        if (_gridAreaObject != null)
        {
            GameObject temp = Instantiate(_gridAreaObject, transform);

            temp.transform.position = transform.position + new Vector3(0, -0.1f, 0);
            temp.transform.localScale = new Vector3(_halfXSize * _cellSize * 0.2f, 1, _halfZSize * _cellSize * 0.2f);
            temp.name = _gridAreaObjectName;
        }
    }

    public Vector3 CalGridPosition(Vector3 pos)
    {
        Vector3 newPos = new Vector3(Mathf.Floor(pos.x / _cellSize) * _cellSize + _cellSize / 2f,
         Mathf.Floor((pos.y + 0.1f) / _cellSize), Mathf.Floor(pos.z / _cellSize) * _cellSize + _cellSize / 2f);

        return newPos;
    }

    /// <summary>
    /// 클릭 위치에 Object 생성
    /// </summary>
    public void InstantiateObject(Vector3 newPos, GameObject target)
    {
        if (_objectDic.ContainsKey(newPos)
            || target.GetComponent<EditingObject>() == null)
            return;

        // 오브젝트 생성
        GameObject newObject = Instantiate(target, transform);

        // 오브젝트 위치 지정
        float height = (newObject.GetComponent<Collider>()?.bounds.size.y ?? 0) * 0.5f;
        newObject.transform.position = new Vector3(newPos.x, newPos.y + height, newPos.z);
        newObject.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);

        // 오브젝트 캐싱
        EditingObject customObject = newObject.GetComponent<EditingObject>();
        _objectDic.Add(newPos, customObject);
    }

    /// <summary>
    /// 클릭 위치에 Object 제거
    /// </summary>
    public void DestroyObject(GameObject target)
    {
        if (target.name == _gridAreaObjectName)
            return;

        EditingObject customObject = target.GetComponent<EditingObject>();
        Vector3 key = Vector3.zero;
        foreach(var ob in _objectDic)
        {
            if (ob.Value == customObject)
            {
                key = ob.Key;
                break;
            }
        }

        if (key != Vector3.zero)
            _objectDic.Remove(key);
        DestroyImmediate(target);
    }

    private GameObject _curGuideObject = null;
    private float _curGuideObjectHeight = 0;
    public void OnChangeGuideObject(GameObject newGuideObject)
    {
        if (newGuideObject.GetComponent<EditingObject>() == null)
            return;

        // 이전 Guide 오브젝트 제거
        if (_curGuideObject != null)
            DestroyImmediate(_curGuideObject);

        // 새 Guide 오브젝트 생성
        _curGuideObject = Instantiate(newGuideObject, transform);

        // Collider를 제거하므로, 미리 Height 값 계산 및 캐싱
        _curGuideObjectHeight = (_curGuideObject.GetComponent<Collider>()?.bounds.size.y ?? 0) * 0.5f;

        // 새 Guide 초기화
        _curGuideObject.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
        _curGuideObject.GetComponent<Collider>().enabled = false;
        _curGuideObject.SetActive(false);
    }

    public void OnSetGuideObject(Vector3 newPos, bool setActive = true)
    {
        if (_curGuideObject == null)
            return;

        if (setActive == false)
        {
            _curGuideObject.SetActive(false);
            return;
        }

        _curGuideObject.SetActive(setActive);
        _curGuideObject.transform.position = new Vector3(newPos.x, newPos.y + _curGuideObjectHeight, newPos.z);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (MapEditorStart == false)
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
        for (float z = pos.z - _halfZSize * _cellSize; z <= pos.z + _halfZSize * _cellSize; z += _cellSize)
        {
            Gizmos.DrawLine(new Vector3(_halfXSize * _cellSize + pos.x, pos.y, Mathf.Floor(z / _cellSize) * _cellSize),
            new Vector3(-_halfXSize * _cellSize + pos.x, pos.y, Mathf.Floor(z / _cellSize) * _cellSize));
        }

        // Horizontal Grid 표시
        for (float x = pos.x - _halfXSize * _cellSize - _cellSize; x < pos.x + _halfXSize * _cellSize; x += _cellSize)
        {
            Gizmos.DrawLine(new Vector3(Mathf.Floor(x / _cellSize) * _cellSize + _cellSize, pos.y, _halfZSize * _cellSize + pos.z),
            new Vector3(Mathf.Floor(x / _cellSize) * _cellSize + _cellSize, pos.y, -_halfZSize * _cellSize + pos.z));
        }
    }
#endif
}
