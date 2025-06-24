using System;
using UnityEngine;

public interface IDataConvertible<TRaw>
{
    TRaw ToRaw();
    void FromRaw(TRaw raw);
}

public class UserWorkData : IDataConvertible<UserWorkDataRaw>
{
    public ObservableModel<int> stepCount;
    public ObservableModel<int> coin;

    public UserWorkData()
    {
        stepCount = new ObservableModel<int>();
        coin = new ObservableModel<int>();
    }

    public void FromRaw(UserWorkDataRaw raw)
    {
        stepCount.Value = raw.stepCount; 
        coin.Value = raw.stepsToReward;
    }

    public UserWorkDataRaw ToRaw()
    {
        return new UserWorkDataRaw
        {
            stepsToReward = coin.Value,
            stepCount = 0 // 시작할 때 항상 0 으로 초기화(GameManager 에서 Load로 한번에 처리되게 하려고 임의로 넣음, 필요한 데이터는 아니라고 생각됨)
        };
    }
}

[Serializable]
public class UserWorkDataRaw
{
    public int stepsToReward;
    public int stepCount;
}
