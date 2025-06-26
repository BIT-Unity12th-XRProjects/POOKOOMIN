using FoodyGo.Managers;
using FoodyGo.Mapping;
using FoodyGo.UI;
using FoodyGo.Utils.DI;
using FoodyGo.Services;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using FoodyGo.Services.GPS;

namespace Pookoomin.Controller
{
    public class PlayerController : MonoBehaviour
    {

        [SerializeField] LayerMask _battleMask;
        [SerializeField] PlayerRenderer _renderer;

        //@TK 임시
        private PetController _pet;

        /// <summary>
        /// 움직임 이벤트 : 파라미터는 움직임 값
        /// </summary>
        public event Action<float> OnMovement;

        public void OnChangeGameState(GameState state)
        {
            if(state == GameState.ARCamera)
            {
                _renderer.SetMeshVisible(false);
            }
            else if(state == GameState.Lobby || state == GameState.Walk)
            {
                _renderer.SetMeshVisible(true);
            }
        }

        private void Awake()
        {
            GameManager.instance.onChangeGameState += OnChangeGameState;
        }

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

            GameObject petObj = Resources.Load<GameObject>("Entity/Pet/LittleSquirrel");
            _pet = Instantiate(petObj, transform.position + new Vector3(3f, 0f, 3f), Quaternion.identity).GetComponent<PetController>();
            _pet.InitData(PetMode.Walk, transform);
        }

        private void OnDisable()
        {
            _moveInputAction.action.performed -= OnMovePerformed;
            _moveInputAction.action.canceled -= OnMoveCanceled;
            _moveInputAction.action.Disable();

            Destroy(_pet.gameObject);
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

        //속력 계산
        private double _prevLatitude;
        private double _prevLongitude;
        private double _prevTime;
        private const double SpeedUpdateInterval = 1.0; // 1초마다 갱신
        private double _currentSpeed; //현재 스피드
        private float _animSpeed;     //애니메이션 파라미터

        private void OnEnable()
        {
            _prevLatitude = _gpsLocationService.latitude;
            _prevLongitude = _gpsLocationService.longitude;
            _prevTime = _gpsLocationService.timeStamp;

            _currentSpeed = 0;
            _animSpeed = 0;

            GameObject petObj = Resources.Load<GameObject>("Entity/Pet/LittleSquirrel");
            _pet = Instantiate(petObj, transform.position + new Vector3(3f, 0f, 3f), Quaternion.identity).GetComponent<PetController>();
            _pet.InitData(PetMode.Walk, transform);
        }

        private void OnDisable()
        {
            Destroy(_pet.gameObject);
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

