using UnityEngine;

public class PlayerRenderer : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;

    public void SetMeshVisible(bool active)
    {
        _renderer.enabled = active;
    }
}
