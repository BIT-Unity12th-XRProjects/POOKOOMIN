using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject _ar;
    [SerializeField] private GameObject _root;

    public void OnSwitchArCamera()
    {
        _ar.SetActive(true);
        _root.SetActive(false);
    }

    public void OnSwitchGameCamera()
    {
        _ar.SetActive(false);
        _root.SetActive(true);
    }
}
