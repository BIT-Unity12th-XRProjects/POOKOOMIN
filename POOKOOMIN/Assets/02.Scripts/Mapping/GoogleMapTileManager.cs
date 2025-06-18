using FoodyGo.Services.GoogleMaps;
using FoodyGo.Services.GPS;
using System;
using System.Collections;
using UnityEngine;

namespace FoodyGo.Mapping
{
    /// <summary>
    /// Maptile 생성, 갱신, 제거 등의 관리
    /// GPS 데이터가 범위를 벗어날때 타일맵 확장 및 반대방향 타일맵 삭제
    /// </summary>
    public class GoogleMapTileManager : MonoBehaviour
    {
        public bool isInitialized { get; private set; }

        [Header("Configuration")]
        [SerializeField] GoogleStaticMapService _googleStaticMapService;
        [SerializeField] GPSLocationService _gpsLocationService;
        [SerializeField] GoogleMapTile _mapTilePrefab;
        [SerializeField] Transform _mapTilesParent;

        [Header("Managed mapTiles")]
        GoogleMapTile[,] _mapTiles = new GoogleMapTile[GRID_SIZE, GRID_SIZE];
        readonly int[] TILE_OFFSETS = { -1, 0, 1 };
        const int GRID_SIZE = 3;
        const float PLANE_SIZE = 100f;

        MapLocation _mapOrigin;
        Vector2Int _currentCenterTileIndex;

        IEnumerator Start()
        {
            yield return new WaitUntil(() => _gpsLocationService.isReady);
            _mapOrigin = _gpsLocationService.mapCenter;
            InitializeTiles();
            isInitialized = true;
        }

        /// <summary>
        /// 현재 GPS 기반으로 중심 타일 인덱스 계산
        /// 3x3 배열로 MapTile 들 생성
        /// </summary>
        void InitializeTiles()
        {
            var centerLoc = _gpsLocationService.mapCenter;
            _currentCenterTileIndex = CalcTileIndex(centerLoc);
            CreateTiles(centerLoc);
        }



        // x가 -50 ~ +49.999  → 0
        // x가 +50 ~ +149.999 → +1 …  (음수 대칭)
        int HalfFloor(float v) => Mathf.FloorToInt((v + PLANE_SIZE * 0.5f) / PLANE_SIZE);

        Vector2Int CalcTileIndex(MapLocation loc)
        {
            float x = (float)GoogleMapUtils.LonToUnityX(
                loc.longitude, _gpsLocationService.mapOrigin.longitude, _gpsLocationService.ZoomLevel);

            float z = (float)GoogleMapUtils.LatToUnityY(
                loc.latitude, _gpsLocationService.mapOrigin.latitude, _gpsLocationService.ZoomLevel);

            return new Vector2Int(HalfFloor(x), HalfFloor(z));
        }


        Vector3 CalcTilePosition(Vector2Int index)
        {
            return new Vector3(index.x * PLANE_SIZE, 0f, index.y * PLANE_SIZE);
        }

        Vector3 CalcTilePosition(MapLocation location)
        {
            Vector2Int index = CalcTileIndex(location);
            Vector3 position = CalcTilePosition(index);

            return position;
        }

        void CreateTiles(MapLocation location)
        {
            // 중심 인덱스 기준으로 모든 방향 타일들 인덱스 계산
            for (int i = 0; i < TILE_OFFSETS.Length; i++)
            {
                for (int j = 0; j < TILE_OFFSETS.Length; j++)
                {
                    GoogleMapTile tile = Instantiate(_mapTilePrefab, _mapTilesParent);
                    tile.tileOffset = new Vector2Int(i - 1, j - 1);
                    tile.googleStaticMapService = _googleStaticMapService;
                    tile.worldCenterLocation = _gpsLocationService.mapOrigin;
                    tile.zoomLevel = _gpsLocationService.ZoomLevel;
                    tile.gpsLocationService = _gpsLocationService;
                    tile.transform.position = CalcTilePosition(location)
                        + new Vector3(i - 1, 0, j - 1) * PLANE_SIZE;
                    tile.name = CalcTileIndex(location).ToString();
                    tile.RefreshMapTile();
                    _mapTiles[i, j] = tile;
                }
            }
        }

        void Update()
        {
            if (!isInitialized) return;

            // 현재 GPS
            MapLocation curLoc = new MapLocation(
                _gpsLocationService.latitude, _gpsLocationService.longitude);

            float dx = (float)GoogleMapUtils.LonToUnityX(
                curLoc.longitude, _gpsLocationService.mapOrigin.longitude, _gpsLocationService.ZoomLevel);
            float dz = (float)GoogleMapUtils.LatToUnityY(
                curLoc.latitude, _gpsLocationService.mapOrigin.latitude, _gpsLocationService.ZoomLevel);

            Vector2Int targetIndex = CalcTileIndex(curLoc);
            Vector2Int deltaIndex = targetIndex - _currentCenterTileIndex;

            // ────────── X 방향(경도) ──────────
            if (Mathf.Abs(deltaIndex.x) == 1)
            {
                ShiftColumn(deltaIndex.x, curLoc);
                _currentCenterTileIndex.x += deltaIndex.x;
            }

            if (Mathf.Abs(deltaIndex.y) == 1)
            {
                ShiftRow(deltaIndex.y, curLoc);
                _currentCenterTileIndex.y += deltaIndex.y;
            }

        }

        // dirX : +1 → 플레이어가 동쪽( +X )으로 1칸(100u) 넘어감
        //        -1 → 플레이어가 서쪽( -X )으로 1칸 넘어감
        void ShiftColumn(int dirX, MapLocation centerLoc)
        {
            // ① 어느 칼럼을 재활용(빼내고) 어느 칼럼 자리에 꽂을지 정리
            int recycleIdx = dirX == +1 ? 0 : GRID_SIZE - 1; // 서쪽 or 동쪽 칼럼
            int insertIdx = dirX == +1 ? GRID_SIZE - 1 : 0;             // 동쪽 or 서쪽 칼럼

            for (int j = 0; j < GRID_SIZE; ++j)
            {
                GoogleMapTile temp = _mapTiles[recycleIdx, j];

                // ② 배열 레퍼런스 한 칸씩 ‘미는’ 방향도 반대가 되어야 함
                if (dirX == +1)  // 동쪽으로 1칸 이동 → 배열을 ← 방향으로 땡김
                {
                    for (int x = 0; x < GRID_SIZE - 1; ++x)
                        _mapTiles[x, j] = _mapTiles[x + 1, j];
                }
                else            // 서쪽으로 1칸 이동 → 배열을 → 방향으로 땡김
                {
                    for (int x = GRID_SIZE - 1; x > 0; --x)
                        _mapTiles[x, j] = _mapTiles[x - 1, j];
                }

                // ③ 재활용 칼럼을 새 자리(insertIdx)로 옮기기
                _mapTiles[insertIdx, j] = temp;

                // ─── 타일 정보 갱신 ───
                temp.tileOffset.x += dirX * GRID_SIZE;      // 논리적 오프셋 보정 (+3 or -3)
                temp.transform.position = CalcTilePosition(centerLoc)
                                        + new Vector3(insertIdx - 1, 0, j - 1) * PLANE_SIZE;
                temp.RefreshMapTile();
            }
        }


        // dirY : +1 → 북쪽으로 새 행 필요(플레이어가 남쪽 → 북쪽), -1 → 남쪽
        void ShiftRow(int dirY, MapLocation centerLoc)
        {
            int recycleIdx = dirY == +1 ? 0 : GRID_SIZE - 1; // 재활용할 행
            int insertIdx = dirY == +1 ? GRID_SIZE - 1 : 0;            // 새로 들어올 행

            for (int i = 0; i < GRID_SIZE; ++i)
            {
                GoogleMapTile recycled = _mapTiles[i, recycleIdx];

                if (dirY == +1)                      // 남→북으로 한 칸씩 이동
                    for (int j = recycleIdx; j < GRID_SIZE - 1; ++j) _mapTiles[i, j] = _mapTiles[i, j + 1];
                else                                // 북→남
                    for (int j = recycleIdx; j > 0; --j) _mapTiles[i, j] = _mapTiles[i, j - 1];

                _mapTiles[i, insertIdx] = recycled;
                _mapTiles[i, insertIdx].name = CalcTileIndex(centerLoc).ToString();

                recycled.tileOffset.y += -dirY * GRID_SIZE;
                // ── Z축 재배열 ──
                recycled.transform.position = CalcTilePosition(centerLoc)
                                             + new Vector3(i - 1, 0, insertIdx - 1) * PLANE_SIZE;

                recycled.RefreshMapTile();
            }
        }


        void OnGUI()
        {
            // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
            if (GUI.Button(new Rect(20, 40, 80, 20), "Level 1"))
            {
                CreateTiles(new MapLocation(_gpsLocationService.latitude, _gpsLocationService.longitude));
            }
        }
    }
}