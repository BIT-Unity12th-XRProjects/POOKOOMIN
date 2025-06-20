using FoodyGo.Controllers;
using UnityEngine;

namespace Pookoomin.Controller
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private PlayerController _player;
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
            _player = GetComponent<PlayerController>();
            _anim = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            if (_player != null)
            {
                _player.OnMovement += SetMovementAnimation;
            }
        }

        private void OnDisable()
        {
            if (_player != null)
            {
                _player.OnMovement -= SetMovementAnimation;
            }
        }

        public void SetMovementAnimation(float value)
        {
            _anim.SetFloat(AnimParams.MoveSpeed.ToString(), value);
        }
    }
}

