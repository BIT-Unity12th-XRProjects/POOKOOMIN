using System.Collections.Generic;
using UnityEngine;

public class StepRewardData 
{
    [System.Serializable]
    public class StepRewardEntry
    {
        public int stepThreshold;       // 몇 걸음 이상 걸으면
        public string rewardItemId;     // 어떤 보상을 주는가
    }

    [System.Serializable]
    public class StepRewardConfig
    {
        public List<StepRewardEntry> rewards;
    }
}
