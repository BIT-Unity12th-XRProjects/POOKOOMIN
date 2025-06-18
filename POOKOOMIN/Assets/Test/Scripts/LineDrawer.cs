using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LineDrawer : MonoBehaviour
{
    [SerializeField] InputActionReference _tapInputAction;
    [SerializeField] InputActionReference _tapPositionInputAction;
    [SerializeField] Camera _xrCamera;
    [SerializeField] float _drawOffsetZ = 0.5f;
    [SerializeField] float _drawMinDistance = 0.01f;
    [SerializeField] float _lineWidth = 0.01f;
    private LineRenderer _lineRenderer;
    private bool _isDrawing;
    private Vector2 _cachedTouchPosition;
    private List<Vector3> _positions;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startWidth = _lineWidth;
        _lineRenderer.endWidth = _lineWidth;
        _lineRenderer.positionCount = 0;
        _positions = new List<Vector3>(512);
    }

    private void Start()
    {
        _tapInputAction.action.started += OnTapStarted;
        _tapInputAction.action.canceled += OnTapCanceled;
        _tapPositionInputAction.action.performed += OnTapPositionPerformed;
    }

    void OnTapStarted(InputAction.CallbackContext context)
    {
        _isDrawing = true;
        AddPoint();
    }

    void OnTapCanceled(InputAction.CallbackContext context)
    {
        _isDrawing= false;
    }

    void OnTapPositionPerformed(InputAction.CallbackContext context)
    {
        if (_isDrawing)
        {
            Vector2 touchPosition = context.ReadValue<Vector2>();

            if (Vector3.Distance(touchPosition, _cachedTouchPosition) >= _drawMinDistance)
            {
                _cachedTouchPosition = touchPosition;
                AddPoint();
            }
        }
        else
        {
            _cachedTouchPosition = context.ReadValue<Vector2>();
        }
    }

    void AddPoint()
    {
        Ray ray = _xrCamera.ScreenPointToRay(_cachedTouchPosition);

        Vector3 planeNormal = _xrCamera.transform.forward;
        Plane plane = new Plane(planeNormal, _drawOffsetZ);

        // ray.Origin 에서부터 hit 된 point 까지의 거리 = enter
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            _positions.Add(hitPoint);
            _lineRenderer.positionCount = _positions.Count;
            int index = _lineRenderer.positionCount - 1; // 새로 추가된 인덱스
            _lineRenderer.SetPosition(index, hitPoint);
        }
    }
}
