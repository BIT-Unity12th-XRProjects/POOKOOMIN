using FoodyGo.Services.GoogleMaps;
using FoodyGo.Services.GPS;
using UnityEngine;

namespace FoodyGo.Mapping
{
    public class GoogleMapTile : MonoBehaviour
    {
        [Header("Map Settings")]
        [Tooltip("줌 레벨")]
        [Range(1, 20)]
        public int zoomLevel = 15;

        [Tooltip("맵 텍스쳐 사이즈")]
        [Range(64, 1024)]
        public int size = 640;

        [Tooltip("월드 맵 원점")]
        public MapLocation worldCenterLocation;

        [Header("Tile Settings")]
        [Tooltip("타일링을 위한 오프셋")]
        public Vector2Int tileOffset;

        [Tooltip("오프셋 적용한 맵의 중심 위치")]
        public MapLocation tileCenterLocation;

        [Header("Map Services")]
        public GoogleStaticMapService googleStaticMapService;

        [Header("GPS Services")]
        public GPSLocationService gpsLocationService;

        private Renderer _renderer;


        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        public void RefreshMapTile()
        {
            double lon = GoogleMapUtils.UnityXToLon(transform.position.x, worldCenterLocation.longitude, zoomLevel);
            double lat = GoogleMapUtils.UnityYToLat(transform.position.z, worldCenterLocation.latitude, zoomLevel);
            tileCenterLocation = new MapLocation(lat, lon);

            // 맵 텍스쳐 요청
            googleStaticMapService.LoadMap(tileCenterLocation.latitude,
                                           tileCenterLocation.longitude,
                                           zoomLevel,
                                           new Vector2(size, size),
                                           OnMapLoaded);
        }

        private void OnMapLoaded(Texture2D texture)
        {
            if (_renderer.material.mainTexture != null)
                Destroy(_renderer.material.mainTexture);

            _renderer.material.mainTexture = texture;
        }

        private readonly int TILE_WORLD_SIZE = 100;
        /// <summary>
        /// Tile 영역 선으로 그리기
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, new Vector3(TILE_WORLD_SIZE, 1f, TILE_WORLD_SIZE));
        }
    }
}
