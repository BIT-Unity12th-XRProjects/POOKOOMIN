using FoodyGo.Managers;
using System;
using UnityEngine;

public class GoogleFitService : MonoBehaviour
{
    public Action<int> OnStepCountChanged;

    UserWorkData userWorkData;

    private int _lastStep = -1;
    private int _step = 0;
    private bool isInitialized = false;

    private void Start()
    {
        userWorkData = GameManager.instance.userWorkData;
    }

    /// <summary>
    /// Java -> Unity로 호출 됨 (GoogleFit Sensor 변경될 때마다)
    /// </summary>
    /// <param name="valueStr"></param>
    public void onStepCountChanged(string valueStr)
    {
        Debug.Log("onStepCountChanged called with value: " + valueStr);
        int currentSteps = int.Parse(valueStr);

        if (!isInitialized)
        {
            _lastStep = currentSteps;
            isInitialized = true;
            Debug.Log("set init: " + _lastStep);
            return;
        }

        int delta = currentSteps - _lastStep;
        _lastStep = currentSteps;

        if (delta > 0)
        {
            _step += delta;
            Debug.Log("Total step count" + _step);
            userWorkData.stepCount.Value = _step;
            //OnStepCountChanged?.Invoke(_step);
        }
        else
        {
            Debug.Log("No Change, No Event");
        }
    }

    public void FailDebug(string msg)
    {
        Debug.Log(msg);
    }
}
