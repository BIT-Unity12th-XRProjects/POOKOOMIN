using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;

public class ARContentManager : MonoBehaviour
{
    private ARRaycastManager arRaycastManager;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private NavMeshAgent agent;

    public GameObject agentPrefab;
    public GameObject groundPrefab;

    private GameObject _instanceGround;

    private bool isFirst = true;

    private void Start()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
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

                GameObject instanceAgent = Instantiate(agentPrefab, hitPose.position, hitPose.rotation);
                agent = instanceAgent.GetComponent<NavMeshAgent>();
            }
            else
            {
                // 해당 위치로 ground를 옮기고 NavMeshSurface 빌드를 다시 함
                _instanceGround.transform.position = hitPose.position;
                StartCoroutine(BuildNavMeshNextFrame(_instanceGround));

                // 이미 생성된 경우, 해당 위치로 이동 명령
                agent.SetDestination(hitPose.position);
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