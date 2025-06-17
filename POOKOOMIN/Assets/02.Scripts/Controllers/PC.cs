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
        /// 움직임 이벤트 : 파라미터는 움직임 값
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
            transform.position = _mapTileManager.GetCenterTileWorldPosition();
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
                
        private Vector3 _prevPosition;

        [Inject] GPSLocationService _gpsLocationService;

        private void FixedUpdate()
        {
            float x = GoogleMapUtils.LonToUnityX(_gpsLocationService.longitude, _gpsLocationService.mapOrigin.longitude, _gpsLocationService.mapTileZoomLevel);

            float z = GoogleMapUtils.LatToUnityY(_gpsLocationService.latitude, _gpsLocationService.mapOrigin.latitude, _gpsLocationService.mapTileZoomLevel);

            transform.position = new Vector3(x, 0f, z);

            _prevPosition = transform.position;
            Vector3 newPosition = new Vector3(x, 0f, z);

            float moveSpeed = (newPosition - _prevPosition).magnitude / Time.fixedDeltaTime;
            
            //@TK(25.06.17) Check 빌드에서 애니메이션 잘 나오는지
            OnMovement?.Invoke(moveSpeed);

            transform.position = newPosition;
            _prevPosition = newPosition;

            //TODO : 어떻게 LookAt 할 것인가
        }
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