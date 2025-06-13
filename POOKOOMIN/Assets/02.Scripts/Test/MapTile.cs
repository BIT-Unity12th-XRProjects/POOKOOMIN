using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Pookoomin.Map
{
    public class MapTile : MonoBehaviour
    {

        [Header("Map Settings")]
        [Tooltip("¡‹ ∑π∫ß")]
        [Range(1, 20)]
        public int zoomLevel = 19;

        [Tooltip("∏  ≈ÿΩ∫√ƒ ªÁ¿Ã¡Ó")]
        [Range(64, 1024)]
        public int size = 640;

        public RawImage mapRawImage;

        [Header("∏  ¡§∫∏ º≥¡§")]
        public string strBaseURL = "";
        public double latitude = 35.000;
        public double longitude = 35.000;
        public string strAPIKey = "";

        private void Start()
        {
            mapRawImage = GetComponent<RawImage>();
            StartCoroutine(LoadMap());
        }

        IEnumerator LoadMap()
        {
            string url = strBaseURL + "center=" + latitude + "," + longitude +
                "&zoom=" + zoomLevel.ToString() + "&size=" + size.ToString() + "x" +
                size.ToString()
                + "&key=" + strAPIKey;

            Debug.Log("URL : " + url);

            url = UnityWebRequest.UnEscapeURL(url);
            UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);

            yield return req.SendWebRequest();

            mapRawImage.texture = DownloadHandlerTexture.GetContent(req);


        }
    }
}

