using FoodyGo.Mapping;
using UnityEngine;

[CreateAssetMenu(menuName ="Google/GPSLocationService", fileName = "GPSLocationService")]
public class GPSLocationServiceSO : ScriptableObject
{
    [Header("Map Tile Data")]
    [Tooltip("맵 타일 스케일")]
    [field: SerializeField]
    public int mapTileScale { get; private set; } = 1;

    [Tooltip("맵 타일 크기 (픽셀)")]
    [field: SerializeField]
    public int mapTileSizePixels { get; private set; } = 640;
    
    [Tooltip("맵 타일 Zoom 레벨 (1 ~ 20)")]
    [Range(1, 20)]
    [field: SerializeField]
    public int mapTileZoomLevel { get; private set; } = 15;

    [Space(20)]
#if UNITY_EDITOR
    [Header("Simulated")]
    [Tooltip("시뮬레이션 시 시작 위치")] //@tk : Default : 강남 비트 건물 좌표
    public MapLocation simulationStartLocation = new MapLocation(37.4946, 127.0276056);

#endif
}
