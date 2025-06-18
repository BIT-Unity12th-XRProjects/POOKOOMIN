using FoodyGo.Mapping;
using FoodyGo.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace FoodyGo.Services.GPS
{
    public class GPSLocationService : MonoBehaviour
    {
        [SerializeField] private GPSLocationServiceSO _serviceData;

        public bool isReady { get; private set; }

        [Header("Simulation Settings (Editor Only)")]
        [SerializeField] bool _isSimulation;
        [SerializeField] Transform _simulationTarget;

        public double latitude { get; private set; }
        public double longitude { get; private set; }
        public double altitude { get; private set; }
        public float accuracy { get; private set; }
        public double timeStamp { get; private set; }

        public int ZoomLevel => _serviceData.mapTileZoomLevel;

        public event Action onMapRedraw;

        public MapLocation mapOrigin; 
        public MapLocation mapCenter;
        public MapEnvelope mapEnvelope;
        public Vector2 mapScale;

        private ILocationProvider _locationProvider;

        private void Awake()
        {
#if UNITY_EDITOR
            SimulatedLocationProvider simulatedLocationProvider = gameObject.AddComponent<SimulatedLocationProvider>();
            simulatedLocationProvider.target = _simulationTarget;
            simulatedLocationProvider.zoomLevel = _serviceData.mapTileZoomLevel;
            simulatedLocationProvider.startLocation = _serviceData.simulationStartLocation;
            _locationProvider = simulatedLocationProvider;
            timeStamp = Epoch.Now;
#else
            _locationProvider = gameObject.AddComponent<DeviceLocationProvider>();
#endif
        }

        IEnumerator Start()
        {
            yield return new WaitUntil(() => _locationProvider.isRunning);
            mapOrigin = new MapLocation(_locationProvider.latitude, _locationProvider.longitude);
            isReady = true;
        }

        private void OnEnable()
        {
            _locationProvider.onLocationUpdated += OnLocationUpdated;
            _locationProvider.StartService();
        }

        private void OnDisable()
        {
            _locationProvider.onLocationUpdated -= OnLocationUpdated;
            _locationProvider.StopService();
        }

        private void OnLocationUpdated(double newLatitude, double newLongitude, double newAltitude, float newAccuracy, double newTimeStamp)
        {
            latitude = newLatitude;
            longitude = newLongitude;
            altitude = newAltitude;
            accuracy = newAccuracy;
            timeStamp = newTimeStamp;

            //if (mapEnvelope.Contains(new MapLocation(latitude, longitude)) == false)
            {
                CenterMap();
            }
        }

        private void CenterMap()
        {
            mapCenter.latitude = latitude;
            mapCenter.longitude = longitude;
           
            onMapRedraw?.Invoke();
        }
    }
}
