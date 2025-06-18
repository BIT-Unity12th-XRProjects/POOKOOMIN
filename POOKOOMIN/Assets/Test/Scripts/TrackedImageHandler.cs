using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImageHandler : MonoBehaviour
{
    [SerializeField] ARTrackedImageManager _arTrackedImageManager;
    [SerializeField] GameObject _placePrefab;
    private Dictionary<TrackableId, GameObject> _placedMarkers = new Dictionary<TrackableId, GameObject>();

    private void Start()
    {
        _arTrackedImageManager.trackablesChanged.AddListener(OnTrackablesChanged);
    }

    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach(ARTrackedImage image in args.added)
        {
            if(_placedMarkers.TryGetValue(image.trackableId, out GameObject marker) == false)
            {
                _placedMarkers.Add(image.trackableId,Instantiate(_placePrefab));
            }

            _placedMarkers[image.trackableId].transform.position = image.transform.position;
            _placedMarkers[(image.trackableId)].transform.rotation = image.transform.rotation;
        }

        foreach (ARTrackedImage image in args.updated)
        {
            if (_placedMarkers.TryGetValue(image.trackableId, out GameObject marker) == false)
            {
                _placedMarkers.Add(image.trackableId, Instantiate(_placePrefab));
            }

            _placedMarkers[image.trackableId].transform.position = image.transform.position;
            _placedMarkers[(image.trackableId)].transform.rotation = image.transform.rotation;
        }

        foreach (KeyValuePair<TrackableId, ARTrackedImage> image in args.removed)
        {
            //µñ¼Å³Ê¸®¿¡¼­ »èÁ¦
        }
    }
}
