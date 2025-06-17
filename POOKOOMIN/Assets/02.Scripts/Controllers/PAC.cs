using FoodyGo.Controllers;
using UnityEngine;

/// <summary>
/// PlayerAnimationController
/// </summary>
[RequireComponent(typeof(Animator))]
public class PAC : MonoBehaviour
{
    private PC _pc;
    private Animator _anim;

    private enum AnimParams
    {
        MoveSpeed,
        Pickup,
        Grounded,
        Wave,
        Win,
    }

    private void Awake()
    {
        _pc = GetComponent<PC>();
        _anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (_pc != null)
        {
            _pc.OnMovement += SetMovementAnimation;
        }
    }

    private void OnDisable()
    {
        if (_pc != null)
        {
            _pc.OnMovement -= SetMovementAnimation;
        }
    }

    public void SetMovementAnimation(float value)
    {
        _anim.SetFloat(AnimParams.MoveSpeed.ToString(), value);
    }
}
