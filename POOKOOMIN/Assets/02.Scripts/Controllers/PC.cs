using FoodyGo.Mapping;
using FoodyGo.Services.GPS;
using FoodyGo.UI;
using FoodyGo.Utils.DI;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace FoodyGo.Controllers
{
    /// <summary>
    /// PlayerController
    /// </summary>
    public class PC : MonoBehaviour
    {
        [SerializeField] LayerMask _battleMask;

        /// <summary>
        /// ������ �̺�Ʈ : �Ķ���ʹ� ������ ��
        /// </summary>
        public event Action<float> OnMovement; 

#if UNITY_EDITOR
        public Vector3 velocity;
        public Vector3 direction;
        public float speed = 5f;

        [SerializeField] CinemachineCamera _cam;
        [SerializeField] GoogleMapTileManager _mapTileManager;
        [SerializeField] InputActionReference _moveInputAction;


        IEnumerator Start()
        {
            yield return new WaitUntil(() => _mapTileManager.isInitialized);
            //transform.position = _mapTileManager.GetCenterTileWorldPosition();
        }

        private void OnEnable()
        {
            _moveInputAction.action.performed += OnMovePerformed;
            _moveInputAction.action.canceled += OnMoveCanceled;
            _moveInputAction.action.Enable();
        }

        private void OnDisable()
        {
            _moveInputAction.action.performed -= OnMovePerformed;
            _moveInputAction.action.canceled -= OnMoveCanceled;
            _moveInputAction.action.Disable();
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            Vector2 input2D = context.ReadValue<Vector2>();
            direction = new Vector3(input2D.x, 0f, input2D.y);
            OnMovement?.Invoke(input2D.sqrMagnitude);
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            direction = Vector3.zero;
            OnMovement?.Invoke(0f);
        }

        private void FixedUpdate()
        {
            Vector3 camForward = _cam.transform.forward;
            Vector3 camRight = _cam.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            Vector3 moveDir = (camForward * direction.z + camRight * direction.x);

            if (moveDir.sqrMagnitude > 0)
            {
                velocity = moveDir.normalized * speed;
                transform.Translate(velocity * Time.fixedDeltaTime, Space.World);

                Vector3 lookDir = new Vector3(moveDir.x, 0f, moveDir.z);
                Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
                transform.rotation = targetRot;
            }
            else
            {
                velocity = Vector3.zero;
            }
        }
#elif UNITY_ANDROID
        
        [Inject] GPSLocationService _gpsLocationService;

        //�ӷ� üũ
        private double _prevLatitude;
        private double _prevLongitude;
        private double _prevTime;
        private const double SpeedUpdateInterval = 1.0; // 1�� �������� �ӵ� üũ
        private double _currentSpeed; // ���� ���� �ӵ�(m/s)
        private float _animSpeed;     // �ִϸ����Ϳ� ���޵� �ӵ�

        private void OnEnable()
        {
            _prevLatitude = _gpsLocationService.latitude;
            _prevLongitude = _gpsLocationService.longitude;
            _prevTime = _gpsLocationService.timeStamp;

            _currentSpeed = 0;
            _animSpeed = 0;
        }

        private void FixedUpdate()
        {
            Movement();
        }

        private void Movement()
        {
            float x = GoogleMapUtils.LonToUnityX(_gpsLocationService.longitude, _gpsLocationService.mapOrigin.longitude, _gpsLocationService.ZoomLevel);
            float z = GoogleMapUtils.LatToUnityY(_gpsLocationService.latitude, _gpsLocationService.mapOrigin.latitude, _gpsLocationService.ZoomLevel);

            transform.position = new Vector3(x, 0f, z);

            double currTime = _gpsLocationService.timeStamp;
            double timeDiff = currTime - _prevTime;

            if (timeDiff >= SpeedUpdateInterval)
            {
                _currentSpeed = GoogleMapUtils.GetSpeed(
                _prevLatitude, _prevLongitude, _prevTime,
                _gpsLocationService.latitude, _gpsLocationService.longitude, currTime
                );

                _animSpeed = (float)((_currentSpeed < 0.2) ? 0.0 : _currentSpeed);

                OnMovement?.Invoke(_animSpeed);

                _prevLatitude = _gpsLocationService.latitude;
                _prevLongitude = _gpsLocationService.longitude;
                _prevTime = currTime;
            }
        }

        //void OnGUI()
        //{
        //    string text = $"Speed: {_currentSpeed:F2} m/s\n
        //    AnimSpeed: {_animSpeed:F2}\n
        //    Latitude: {_prevLatitude:F6}\n
        //    Longitude: {_prevLongitude:F6}";
        //    GUIStyle style = new GUIStyle(GUI.skin.label)
        //    {
        //        fontSize = 32,
        //        alignment = TextAnchor.UpperCenter,
        //        normal = { textColor = Color.white }
        //    };

        //    float width = 600;
        //    float height = 160;
        //    float x = (Screen.width - width) / 2f;
        //    float y = 10;

        //    GUI.Label(new Rect(x, y, width, height), text, style);
        //}
#endif

        private void OnTriggerEnter(Collider other)
        {
            if ((1 << other.gameObject.layer & _battleMask) > 0)
            {
                UI_BattleConfirmWindow window = UIManager.instance.Resolve<UI_BattleConfirmWindow>();
                window.Show();
            }
        }
    }
}