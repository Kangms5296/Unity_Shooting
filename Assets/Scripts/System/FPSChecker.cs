using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSChecker : MonoBehaviour
{
    [Header("Frame Check")]
    [Range(1, 100)] public int _fontSize = 50;
    public Color _fontColor = Color.black;

    private Rect _rect;
    private GUIStyle _style;

    [Header("Frame Limit")]
    public bool _isLimit = false;
    public int _frameLimit = 120;

    private void Start()
    {
        // 프레임 표시
        int width = Screen.width;
        int height = Screen.height;
        _rect = new Rect(10, 10, width, height * 4 / 100);

        _style = new GUIStyle();
        _style.alignment = TextAnchor.UpperLeft;
        _style.fontSize = _fontSize;
        _style.normal.textColor = _fontColor;

        // 최대 프레임 지정
        if (_isLimit)
            Application.targetFrameRate = _frameLimit;
    }

    private float _delta = 0.0f;
    private void Update()
    {
        _delta += (Time.unscaledDeltaTime - _delta) * 0.1f;

        _style.fontSize = _fontSize;
        _style.normal.textColor = _fontColor;
    }

    float _msec;
    float _fps;
    string _result;
    private void OnGUI()
    {
        _msec = _delta * 1000.0f;
        _fps = 1.0f / _delta;

        _result = _msec.ToString("F0") + "ms (" + _fps.ToString("F0") + ")";
        GUI.Label(_rect, _result, _style);
    }
}
