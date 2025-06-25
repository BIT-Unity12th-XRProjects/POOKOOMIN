using UnityEngine;

public class GoogleFitUtil
{
    /// <summary>
    /// Request Google Fit OAuth!
    /// </summary>
    public static void RequestGoogleFitOAuth()
    {
#if UNITY_EDITOR
        Debug.Log("에디터/비안드로이드 환경에서는 구글 핏이 지원되지 않습니다.");
        return;

#elif UNITY_ANDROID
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var intent = new AndroidJavaObject("android.content.Intent"))
        {
            intent.Call<AndroidJavaObject>("setClass", activity,
                new AndroidJavaClass("com.example.usergooglefit.googleFitPermissionActivity"));
            activity.Call("startActivity", intent);
        }
#endif
    }
}
