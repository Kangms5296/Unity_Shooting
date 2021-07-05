using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class MapGrid : CustomMonoBehaviour
{
    [Header("Grid Info")]
    [SerializeField] private GameObject _gridAreaObjectPrefab = null;
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
    private GameObject _gridAreaObject = null;

    private const string _objectContainerName = "ObjectContainer";
    private GameObject _objectContainer = null;

    private const string _guideObjectParentName = "GuideObjectParent";
    private GameObject _guideObjectParent = null;
    private GameObject _guideObject = null;

    private Dictionary<Vector3, EditingObject> _objectDic = new Dictionary<Vector3, EditingObject>();

    public bool MapEditorStart { get; private set; } = false;

    /// <summary>
    /// Map Editor 시작
    /// </summary>
    public void StartMapEditor()
    {
        if (_gridAreaObjectPrefab == null)
            return;

        if (MapEditorStart)
            return;

        _objectDic.Clear();

        // Base Touch Area 생성
        if (_gridAreaObject == null)
        {
            GameObject newObj = Instantiate(_gridAreaObjectPrefab, transform);
            newObj.transform.position = transform.position + new Vector3(0, -0.1f, 0);
            newObj.transform.localScale = new Vector3(_halfXSize * _cellSize * 0.2f, 1, _halfZSize * _cellSize * 0.2f);
            newObj.name = _gridAreaObjectName;

            _gridAreaObject = newObj;
        }

        // Object Container 생성
        if (_objectContainer == null)
        {
            var newObj = new GameObject();
            newObj.transform.SetParent(transform);
            newObj.name = _objectContainerName;
            newObj.layer = LayerMask.NameToLayer("Ground");

            _objectContainer = newObj;
        }

        // Guide Object 생성 -1
        if (_guideObjectParent == null)
        {
            var newObj = new GameObject();
            newObj.transform.SetParent(transform);
            newObj.name = _guideObjectParentName;
            newObj.layer = LayerMask.NameToLayer("Ground");

            _guideObjectParent = newObj;
        }

        // Guide Object 생성 -2
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

        _objectDic.Clear();

        // Base Touch Area 제거
        if (_gridAreaObject != null)
            DestroyImmediate(_gridAreaObject);
        _gridAreaObject = null;

        // Object Container 제거
        if (_objectContainer != null)
            DestroyImmediate(_objectContainer);
        _objectContainer = null;

        // Guide Object 제거
        if (_guideObjectParent != null)
            DestroyImmediate(_guideObjectParent);
        _guideObjectParent = null;
        _guideObject = null;

        MapEditorStart = false;
    }

    /// <summary>
    /// GridAreaObject 이어서 진행
    /// </summary>
    public void ContinueMapEditor()
    {
        // Base Touch Area
        if (transform.Find(_gridAreaObjectName) != null)
            _gridAreaObject = transform.Find(_gridAreaObjectName).gameObject;
        else
        {
            GameObject temp = Instantiate(_gridAreaObjectPrefab, transform);

            temp.transform.position = transform.position + new Vector3(0, -0.1f, 0);
            temp.transform.localScale = new Vector3(_halfXSize * _cellSize * 0.2f, 1, _halfZSize * _cellSize * 0.2f);
            temp.name = _gridAreaObjectName;

            _gridAreaObject = temp;
        }

        // Object Container
        if (transform.Find(_objectContainerName) != null)
            _objectContainer = transform.Find(_objectContainerName).gameObject;
        else
        {
            var newObj = new GameObject();
            newObj.transform.SetParent(transform);
            newObj.name = _objectContainerName;
            newObj.layer = LayerMask.NameToLayer("Ground");

            _objectContainer = newObj;
        }

        // Guide Object
        if (transform.Find(_guideObjectParentName) != null)
            DestroyImmediate(transform.Find(_guideObjectParentName).gameObject);
        _guideObjectParent = null;
        _guideObject = null;

        if (_guideObjectParent == null)
        {
            var newObj = new GameObject();
            newObj.transform.SetParent(transform);
            newObj.name = _guideObjectParentName;
            newObj.layer = LayerMask.NameToLayer("Ground");

            _guideObjectParent = newObj;
        }

        if (_curSelectIndex != -1)
            OnChangeGuideObject(_cachedObjects[_curSelectIndex]);

        // Cache Objects
        _objectDic.Clear();
        Transform perentTrans = _objectContainer.transform;
        for (int i = 0; i < perentTrans.childCount; i++)
        {
            Transform childTrans = perentTrans.GetChild(i);
            Vector3 heightToVector = new Vector3(0.0f, (childTrans.GetComponent<Collider>()?.bounds.size.y ?? 0) * 0.5f, 0.0f);

            _objectDic.Add(childTrans.position - heightToVector, childTrans.GetComponent<EditingObject>());
        }

        MapEditorStart = true;
    }

    public void SaveMap()
    {
        
    }

    public void LoadMap()
    {

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
        GameObject newObject = Instantiate(target, _objectContainer.transform);

        // 오브젝트 위치 지정
        float height = (newObject.GetComponent<Collider>()?.bounds.size.y ?? 0) * 0.5f;
        newObject.transform.position = new Vector3(newPos.x, newPos.y + height, newPos.z);
        newObject.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);

        // 오브젝트 캐싱
        EditingObject customObject = newObject.GetComponent<EditingObject>();
        _objectDic.Add(newPos, customObject);

        EditorUtility.SetDirty(this);
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

        EditorUtility.SetDirty(this);
    }

    private float _curGuideObjectHeight = 0;
    public void OnChangeGuideObject(GameObject newGuideObject)
    {
        if (newGuideObject.GetComponent<EditingObject>() == null)
            return;

        // 이전 Guide 오브젝트 제거
        if (_guideObject != null)
            DestroyImmediate(_guideObject);

        // 새 Guide 오브젝트 생성
        _guideObject = Instantiate(newGuideObject, _guideObjectParent.transform);

        // Collider를 제거하므로, 미리 Height 값 계산 및 캐싱
        _curGuideObjectHeight = (_guideObject.GetComponent<Collider>()?.bounds.size.y ?? 0) * 0.5f;

        // 새 Guide 초기화
        _guideObject.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
        _guideObject.GetComponent<Collider>().enabled = false;
        _guideObject.SetActive(false);
    }

    public void OnSetGuideObject(Vector3 newPos, bool setActive = true)
    {
        if (_guideObject == null)
            return;

        if (setActive == false)
        {
            _guideObject.SetActive(false);
            return;
        }

        _guideObject.SetActive(setActive);
        _guideObject.transform.position = new Vector3(newPos.x, newPos.y + _curGuideObjectHeight, newPos.z);
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
