using UnityEngine;

[CreateAssetMenu(menuName = "Google/GoogleStaticMapService", fileName = "GoogleStaticMapService")]
public class GoogleStaticMapServiceSO : ScriptableObject
{
    public string BASE_URL = "https://maps.googleapis.com/maps/api/staticmap?";
    public string API_KEY = "AIzaSyDqYVb-C0TTNAtDfewlZKnMmqYql9nLDss";
}
