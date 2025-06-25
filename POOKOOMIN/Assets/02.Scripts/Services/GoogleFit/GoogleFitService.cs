using FoodyGo.Mapping;
using FoodyGo.Services.GPS;
using System;
using UnityEngine;

public class GoogleFitService : MonoBehaviour
{
    public Action<int> OnStepCountChanged;

    private int testValue = 0;

    private void Start()
    {
        GoogleFitUtil.GetTodayStepCount();
    }

    /// <summary>
    /// Java -> Unity로 호출 됨 (GoogleFit Sensor 변경될 때마다)
    /// </summary>
    /// <param name="deltaStr"></param>
    public void onStepCountChanged(string deltaStr)
    {
        int delta = int.Parse(deltaStr);
        OnStepCountChanged?.Invoke(delta);
    }

    public void FailDebug(string msg)
    {
        Debug.Log(msg);
    }
}
