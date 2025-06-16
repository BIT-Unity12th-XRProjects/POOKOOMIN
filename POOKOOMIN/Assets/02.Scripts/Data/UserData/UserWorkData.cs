using UnityEngine;

public class UserWorkData
{
    public ObservableModel<int> stepCount;
    public ObservableModel<int> coin;

    public UserWorkData()
    {
        stepCount = new ObservableModel<int>();
        coin = new ObservableModel<int>();
    }
}
