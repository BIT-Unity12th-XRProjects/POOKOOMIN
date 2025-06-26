using FoodyGo.Managers;
using UnityEngine;


public class MainCameraController : MonoBehaviour
{
    private Camera camera;

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
        }
    }

}
