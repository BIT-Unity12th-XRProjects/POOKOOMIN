using FoodyGo.Managers;
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

    private void Start()
    {
        arCamera = GetComponent<Camera>();
        arCameraManager = GetComponent<ARCameraManager>();
        trackedPosDriver = GetComponent<TrackedPoseDriver>();
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

        }
        else
        {
            arCamera.enabled = false;
            arCameraManager.enabled = false;
            trackedPosDriver.enabled = false;

            arSession.SetActive(false);
            xrOrigin.SetActive(false);
        }
    }
}
