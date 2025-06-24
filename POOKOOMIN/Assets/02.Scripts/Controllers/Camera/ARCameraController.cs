using FoodyGo.Managers;
using Pookoomin.UI;
using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.ARFoundation;

public class ARCameraController : MonoBehaviour
{
    [SerializeField] private GameObject arSession;
    [SerializeField] private GameObject xrOrigin;

    private Camera arCamera;
    private ARCameraManager arCameraManager;
    private TrackedPoseDriver trackedPosDriver;
    private ARCameraCapture arCapture;

    private void Start()
    {
        arCamera = GetComponent<Camera>();
        arCameraManager = GetComponent<ARCameraManager>();
        trackedPosDriver = GetComponent<TrackedPoseDriver>();
        arCapture = GetComponent<ARCameraCapture>();
        GameManager.instance.onChangeGameState += OnChangeGameState;
    }

    public void OnChangeGameState(GameState currentState)
    {
        if (currentState == GameState.ARCamera)
        {
            arSession.SetActive(true);
            xrOrigin.SetActive(true);

            arCamera.enabled = true;
            arCameraManager.enabled = true;
            trackedPosDriver.enabled = true;
            arCapture.enabled = true;

            //@tk : 이거 관리하는 클래스 필요. (매번 로드하는 것 이상함)
            GallaryData data = new GallaryData();
            Pookoomin.UI.UIManager.instance.OpenUI<UICameraButtonsController, UICameraButtonsView, GallaryData>(data);
        }
        else
        {

            arCamera.enabled = false;
            arCameraManager.enabled = false;
            trackedPosDriver.enabled = false;
            arCapture.enabled = false;

            arSession.SetActive(false);
            xrOrigin.SetActive(false);

            if (Pookoomin.UI.UIManager.instance.IsOpenedUI<UICameraButtonsView>())
            {
                var buttonUI = Pookoomin.UI.UIManager.instance.GetOpenedUI<UICameraButtonsView>();
                Pookoomin.UI.UIManager.instance.CloseUI(buttonUI);
            }
        }
    }

    public void CaptureImage(Action<Texture2D> onCaptured = null)
    {
        arCapture.CaptureImage(onCaptured);
    }
}
