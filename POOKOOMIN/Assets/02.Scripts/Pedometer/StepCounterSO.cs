using UnityEngine;

namespace Pookoomin.Pedometer
{
    [CreateAssetMenu(fileName = "StepCounterSO", menuName = "StepCounter/stepCounterSO", order = 1)]
    public class StepCounterSO : ScriptableObject
    {
        [Header("Step Detection")]
        [Tooltip("Minimum time between steps in seconds.")]
        public float stepDelay = 0.4f;
    }
}
