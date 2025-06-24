using UnityEngine;
using System.IO;
using System.Linq;

public static class PictureUtil
{
    /// <summary>
    /// 모바일 사진 저장 유틸 메소드
    /// </summary>
    public static void SaveImageToGallery(string filePath)
    {
#if UNITY_EDITOR
        return;
#elif UNITY_ANDROID
        using (AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject mediaScanIntent = new AndroidJavaObject("android.content.Intent", "android.intent.action.MEDIA_SCANNER_SCAN_FILE"))
        using (AndroidJavaObject fileObj = new AndroidJavaObject("java.io.File", filePath))
        using (AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri"))
        using (AndroidJavaObject uri = uriClass.CallStatic<AndroidJavaObject>("fromFile", fileObj))
        {
            mediaScanIntent.Call<AndroidJavaObject>("setData", uri);
            activity.Call("sendBroadcast", mediaScanIntent);
        }
#endif

    }

    /// <summary>
    /// 갤러리에서 첫번째 사진, 썸네일 형태로 로드
    /// </summary>
    public static Texture2D LoadThumbnailToGallary()
    {
#if UNITY_EDITOR
        Debug.Log("에디터/비안드로이드 환경에서는 갤러리 접근이 지원되지 않습니다.");
        return null;
#elif UNITY_ANDROID
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass galleryClass = new AndroidJavaClass("com.example.usergallery.gallery");
            string base64 = galleryClass.CallStatic<string>("getFirstImage", currentActivity);
            Debug.Log($"@@@base64 is null : {base64 == null}");
            Texture2D tex = UIUtil.Base64ToTexture2D(base64);
            Debug.Log($"@@@tex is null : {tex == null}");
            return tex;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error calling Java: " + e.Message);
        }
        return null;
#endif
    }

    /// <summary>
    /// 갤러리 앱 열기
    /// </summary>
    public static void OpenGallery()
    {
#if UNITY_EDITOR
        Debug.Log("에디터/비안드로이드 환경에서는 갤러리 접근이 지원되지 않습니다.");
        return;
#elif UNITY_ANDROID
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass galleryClass = new AndroidJavaClass("com.example.usergallery.gallery");
            galleryClass.CallStatic("openGallery", currentActivity);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error calling Java: " + e.Message);
        }
#endif
    }
}
