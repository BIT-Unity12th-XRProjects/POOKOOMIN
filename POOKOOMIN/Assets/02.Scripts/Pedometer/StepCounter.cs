using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using FoodyGo.Managers;


namespace Pookoomin.Pedometer
{

    public class StepCounter : MonoBehaviour
    {   
        [Header("StepCounterSO")]
        public StepCounterSO stepCounterSO;

        [Header("Runtime Variables")]
        private Vector3 _acceleration;
        private Vector3 _prevAcceleration;

        UserWorkData userWorkData;

        [Header("Step Detection")]
        private float _lastStepTime = 0f;

        private void Start()
        {
            userWorkData = GameManager.instance.userWorkData;

            if (stepCounterSO == null)
            {
                Debug.LogError("StepCounterSO is not assigned in the inspector.");
                return;
            }

            if (!Accelerometer.current.enabled)
            {
                InputSystem.EnableDevice(Accelerometer.current);
                Debug.Log("Accelerometer enabled manually.");
            }

            _prevAcceleration = Accelerometer.current.acceleration.ReadValue();
        }

        private void Update()
        {
            if (userWorkData == null || userWorkData.stepCount == null)
                return;

            if (Accelerometer.current != null)
            {
                Vector3 acceleration = Accelerometer.current.acceleration.ReadValue();                

                // Y축(또는 Z축) 변화만 사용
                float axisDelta = Mathf.Abs(acceleration.y - _prevAcceleration.y);

                if (axisDelta > 1.0f && (Time.time - _lastStepTime) > stepCounterSO.stepDelay) // 흔들림 임계값 설정
                {
                    userWorkData.stepCount.Value++;
                    _lastStepTime = Time.time;
                }

                _prevAcceleration = acceleration;
            }
        }
    }
}