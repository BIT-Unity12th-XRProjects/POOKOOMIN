using FoodyGo.Managers;
using FoodyGo.Utils.DI;
using Unity.Cinemachine;
using UnityEngine;


public class MainCameraController : MonoBehaviour
{
    private Camera camera;
    [Inject("FreeLock Camera")]
    private CinemachineCamera freeLockCam;
    [Inject("TopView Camera")]
    private CinemachineCamera topViewCam;

    private void Start()
    {
        camera = GetComponent<Camera>();
        GameManager.instance.onChangeGameState += OnChangeGameState;
    }

    public void OnChangeGameState(GameState currentState)
    {
        if(currentState == GameState.ARCamera)
        {
            camera.enabled = false;
        }
        else
        {
            camera.enabled = true;
            if(currentState == GameState.Lobby)
            {
                topViewCam?.gameObject.SetActive(false);
            }
            else if(currentState == GameState.Walk)
            {
                topViewCam?.gameObject.SetActive(true);
            }
        }
    }

}
