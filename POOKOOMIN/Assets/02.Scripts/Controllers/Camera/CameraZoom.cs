using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 카메라 컴포넌트 객체에 부착 : 모바일, 에디터 줌 기능 제공
/// </summary>
[RequireComponent(typeof(CinemachineCamera))]
public class CameraZoom : MonoBehaviour
{
    private CinemachineCamera cam;
    [SerializeField] InputActionReference _pinchInputAction;
    [SerializeField] InputActionReference _scrollInputAction;
    public float zoomSpeed = 5f;
    public float minFOV = 30f;
    public float maxFOV = 60f;

    private float prevTouchDistance;

    private void OnEnable()
    {
        cam = GetComponent<CinemachineCamera>();
#if UNITY_EDITOR
        if (_scrollInputAction != null)
        {
            _scrollInputAction.action.performed += OnScroll;
        }
        _scrollInputAction?.action.Enable();
#elif UNITY_ANDROID
        if (_pinchInputAction != null)
        {
            _pinchInputAction.action.performed += OnPinch;
        }
        _pinchInputAction?.action.Enable();
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        if (_scrollInputAction != null)
        {
            _scrollInputAction.action.performed -= OnScroll;
        }
        _scrollInputAction?.action.Disable();
#elif UNITY_ANDROID
        if (_pinchInputAction != null)
        {
            _pinchInputAction.action.performed -= OnPinch;
        }
        _pinchInputAction?.action.Disable();
#endif
    }

    private void OnScroll(InputAction.CallbackContext ctx)
    {
        Vector2 scrollValue = ctx.ReadValue<Vector2>();
        if (Mathf.Abs(scrollValue.y) > 0.01f)
        {
            Zoom(-scrollValue.y);
        }
    }

    private void OnPinch(InputAction.CallbackContext ctx)
    {
        // 두 손가락 터치가 있을 때만 처리
        if (Touchscreen.current != null && Touchscreen.current.touches.Count >= 2)
        {
            var touch0 = Touchscreen.current.touches[0];
            var touch1 = Touchscreen.current.touches[1];

            if (touch0.press.isPressed && touch1.press.isPressed)
            {
                Vector2 pos0 = touch0.position.ReadValue();
                Vector2 pos1 = touch1.position.ReadValue();

                float currDistance = Vector2.Distance(pos0, pos1);

                if (prevTouchDistance != 0)
                {
                    float diff = currDistance - prevTouchDistance;
                    Zoom(-diff * Time.deltaTime);
                }
                prevTouchDistance = currDistance;
            }
        }
        else
        {
            prevTouchDistance = 0;
        }
    }

    /// <summary>
    /// 시네머신 카메라의 렌즈의 FOV 조절
    /// </summary>
    /// <param name="delta"></param>
    private void Zoom(float delta)
    {
        float newFOV = cam.Lens.FieldOfView + delta * zoomSpeed;
        cam.Lens.FieldOfView = Mathf.Clamp(newFOV, minFOV, maxFOV);
    }
}
