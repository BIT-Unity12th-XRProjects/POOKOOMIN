using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;

public class ARContentManager : MonoBehaviour
{
    private ARRaycastManager arRaycastManager;
    public Transform arCameraTransform;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private NavMeshAgent agent;

    public GameObject groundPrefab;

    private GameObject _instancePet;
    private GameObject _instanceGround;

    private GameObject agentPrefab;
    private bool isFirst = true;
    private bool isMovingToTarget = false;
    private Vector3 lastTargetPosition;

    private void Start()
    {
        agentPrefab = Resources.Load<GameObject>("Entity/Pet/LittleSquirrel");
        //@tk : ar용에 맞게 스케일 조정
        agentPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        Destroy(_instancePet);
        Destroy(_instanceGround);
    }


    public void PlaceObjectAtCenter()
    {
        // 화면 정중앙 좌표 (픽셀 단위)
        Vector2 centerScreen = new Vector2(Screen.width / 2f, Screen.height / 2f);

        
        if (arRaycastManager.Raycast(centerScreen, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose; // 평면과 만난 위치와 방향 정보

            if (isFirst)
            {
                isFirst = false;

                _instanceGround = Instantiate(groundPrefab, hitPose.position, hitPose.rotation);
                // NavMeshSurface 빌드를 한 프레임 뒤에 수행 (Mesh 초기화 문제 방지)
                StartCoroutine(BuildNavMeshNextFrame(_instanceGround));

                _instancePet = Instantiate(agentPrefab, hitPose.position, hitPose.rotation);
                _instancePet.GetComponent<PetController>().InitData(PetMode.ARCamera);
                Vector3 lookTarget = new Vector3(
                    arCameraTransform.position.x,
                    _instancePet.transform.position.y, // 펫의 y값으로 고정
                    arCameraTransform.position.z
                );
                _instancePet.transform.LookAt(lookTarget);
                agent = _instancePet.AddComponent<NavMeshAgent>();
            }
            else
            {
                // 해당 위치로 ground를 옮기고 NavMeshSurface 빌드를 다시 함
                _instanceGround.transform.position = hitPose.position;
                StartCoroutine(BuildNavMeshNextFrame(_instanceGround));

                // 이미 생성된 경우, 해당 위치로 이동 명령
                agent.SetDestination(hitPose.position);
                agent.SetDestination(hitPose.position);
                isMovingToTarget = true;
                lastTargetPosition = hitPose.position;
            }
        }
    }

    private void Update()
    {
        if (isMovingToTarget && agent != null && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    Vector3 lookTarget = new Vector3(
                        arCameraTransform.position.x,
                        _instancePet.transform.position.y, // 펫의 y값으로 고정
                        arCameraTransform.position.z
                    );
                    _instancePet.transform.LookAt(lookTarget);
                    isMovingToTarget = false;
                }
            }
        }
    }

    private IEnumerator BuildNavMeshNextFrame(GameObject ground)
    {
        yield return null; // 1프레임 대기

        // NavMeshSurface 컴포넌트를 찾아 NavMesh를 생성
        ground.GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}