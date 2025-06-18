using UnityEngine;

namespace Pookoomin.Pedometer
{
    [CreateAssetMenu(fileName = "StepCounterSO", menuName = "StepCounter/stepCounterSO", order = 1)]
    public class StepCounterSO : ScriptableObject
    {
        [Header("Step Counter Settings")]
        [Tooltip("Average step length in meters.")]
        public float stepLength = 0.75f;

        [Header("Detection Settings")]
        [Tooltip("Acceleration threshold for detecting steps.")]
        public float threshold = 1f;

        [Header("Step Detection")]
        [Tooltip("Minimum time between steps in seconds.")]
        public float stepDelay = 0.4f;
    }
}
