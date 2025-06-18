using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 탭 치면 공 발사
/// </summary>
public class BallSpawner : MonoBehaviour
{
    [SerializeField] InputActionReference _tapInputAction;
    [SerializeField] InputActionReference _tapPositionInputAction;
    [SerializeField] GameObject _ballPrefab;
    [SerializeField] Camera _xrCamera;
    [SerializeField] float _ballSpawnOffsetZ = 0.3f;
    [SerializeField] float _fireForce = 10f;
    private GameObject _ball;
    private Vector2 _cachedTapPosition;

    private void Start()
    {
        _tapInputAction.action.started += OnTapStarted;
        _tapInputAction.action.canceled += OnTapCanceled;

        _tapPositionInputAction.action.performed += OnTapPositionPerformed;
    }

    void OnTapStarted(InputAction.CallbackContext context)
    {
        Vector3 spawnPosition = _xrCamera.transform.position + _xrCamera.transform.forward * _ballSpawnOffsetZ;
        Quaternion spawnRot = _xrCamera.transform.rotation;

        _ball = Instantiate(_ballPrefab, spawnPosition, spawnRot);
        Rigidbody rigidbody = _ball.GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        PlaceBall();
    }
    void OnTapCanceled(InputAction.CallbackContext context)
    {
        Rigidbody rigidbody = _ball.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.AddForce(_xrCamera.transform.forward * _fireForce, ForceMode.Impulse);
        _ball = null;
    }

    void OnTapPositionPerformed(InputAction.CallbackContext context)
    {
        _cachedTapPosition = context.ReadValue<Vector2>();
        PlaceBall();
    }

    void PlaceBall()
    {
        if (_ball == null)
            return;

        Ray ray = _xrCamera.ScreenPointToRay(_cachedTapPosition);

        Vector3 planeNormal = _xrCamera.transform.forward;
        Plane plane = new Plane(planeNormal, _ballSpawnOffsetZ);

        // ray.Origin 에서부터 hit 된 point 까지의 거리 = enter
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            _ball.transform.position = hitPoint;
            _ball.transform.rotation = Quaternion.LookRotation(ray.direction);
        }
    }

    // 기즈모 그리기 - Scene 뷰에서 경로 및 위치 확인
    private void OnDrawGizmos()
    {
        if (_xrCamera == null)
            return;

        // 1. XR 카메라 앞에 생성 위치 구체 (녹색)
        Vector3 spawnPos = _xrCamera.transform.position + _xrCamera.transform.forward * _ballSpawnOffsetZ;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(spawnPos, 0.05f);

        // 2. XR 카메라에서 전방으로 발사되는 Ray (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_xrCamera.transform.position, _xrCamera.transform.forward * (_ballSpawnOffsetZ + 2f));

        // 3. 탭한 화면 좌표로부터 생성되는 Ray (파란색)
        Ray tapRay = _xrCamera.ScreenPointToRay(_cachedTapPosition);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(tapRay.origin, tapRay.direction * 10f);

        // 4. Ray와 평면의 교차 지점 (노란색)
        Vector3 planePoint = _xrCamera.transform.position + _xrCamera.transform.forward * _ballSpawnOffsetZ;
        Plane plane = new Plane(_xrCamera.transform.forward, planePoint);
        if (plane.Raycast(tapRay, out float enter))
        {
            Vector3 hitPoint = tapRay.GetPoint(enter);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(hitPoint, 0.05f);
        }
    }
}
