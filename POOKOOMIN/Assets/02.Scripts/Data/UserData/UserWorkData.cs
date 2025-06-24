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
    //todo : 코인 -> 리워드(5개의 악세서리...)
    //총 걸음 수, 현재 얻은 악세서리 정보를 로컬에 저장 로드 해야해요.
    //TableManager의 로직을 따라가시면 됩니다. persiteneDataPath -> json 넣어서 로컬 Load, Write 넣으셔야함
}
