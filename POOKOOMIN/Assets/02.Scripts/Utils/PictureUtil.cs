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
        // 대표적인 갤러리 경로
        string galleryDir = "/storage/emulated/0/DCIM/Camera";
        string FilePath = string.Empty;
        if (Directory.Exists(galleryDir))
        {
            Debug.Log("@@@@@@@@@@@@@@@갤러리 있음");
            // jpg, png 등 이미지 파일만 필터링
            string[] files = Directory.GetFiles(galleryDir, "*.*")
                .Where(f => f.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".jpeg", System.StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => f)
                .ToArray();

            for (int i = 0; i < files.Length; i++)
            {
                Debug.Log(files[i]);
            }

            if (files.Length > 0)
            {
                FilePath = files[0];

                // 썸네일 생성 (원본을 바로 로드하면 메모리 이슈, 썸네일만 생성)
                byte[] imgBytes = File.ReadAllBytes(FilePath);
                Texture2D tex = new Texture2D(2, 2);
                if (tex.LoadImage(imgBytes))
                {
                    Debug.Log($"texture : {tex}");
                    return tex;
                }

                Debug.Log($"텍스처 못가져옴");

            }
        }
        else
        {
            Debug.LogWarning("갤러리 폴더를 찾을 수 없습니다.");
        }

        return null;
#endif
    }
}
