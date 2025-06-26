using FoodyGo.Managers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum PetMode
{
    Walk,
    ARCamera,
}

/// <summary>
/// (25.06.26)시간없어서 간단하게 만듦 : 추후 수정해야함. (bt 적용)
/// </summary>
public class PetController : MonoBehaviour
{
    public float followSpeed = 5f;
    public float followDistance = 1.5f;
    public List<Renderer> _renderers;

    public PetMode petMode;

    private Transform player;
    Animator _animator;
    

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void InitData(PetMode petMode, Transform player = null)
    {
        if(petMode == PetMode.Walk)
        {
            this.player = player;
            _animator.SetBool("IsMove", true);
            GameManager.instance.onChangeGameState += OnChangeGameState;
        }
        else if(petMode == PetMode.ARCamera)
        {
            _animator.SetBool("IsMove", true);
        }

        this.petMode = petMode;
    }

    void Update()
    {
        if(petMode == PetMode.Walk)
        {
            Vector3 targetPosition = player.position - player.forward * followDistance;
            targetPosition.y = transform.position.y; // Y값 고정

            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            transform.LookAt(player); // 펫이 플레이어를 바라보게 설정
        }
    }

    public void OnChangeGameState(GameState state)
    {
        if (state == GameState.ARCamera)
        {
            SetMeshVisible(false);
        }
        else if (state == GameState.Lobby || state == GameState.Walk)
        {
            SetMeshVisible(true);
        }
    }

    public void SetMeshVisible(bool active)
    {
        foreach (var renderer in _renderers)
        {
            renderer.enabled = active;
        }
    }
}
