using System;
using System.Collections;
using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// ARCamera를 통한 카메라 캡처 기능
/// </summary>
public class ARCameraCapture : MonoBehaviour
{
    public void CaptureImage(Action<Texture2D> onCaptured = null)
    {
        StartCoroutine(CoCaptureImage(onCaptured));
    }
    private IEnumerator CoCaptureImage(Action<Texture2D> onCaptured = null)
    {
        yield return new WaitForEndOfFrame(); 

        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        // 저장 경로 및 파일명 설정
        string fileName = $"ARCapture_{DateTime.Now:yyyyMMdd_HHmmss}.png";
        string filePath = Path.Combine("/storage/emulated/0/DCIM/Camera", fileName);

        File.WriteAllBytes(filePath, texture.EncodeToPNG());
        PictureUtil.SaveImageToGallery(filePath);

        onCaptured?.Invoke(PictureUtil.LoadThumbnailToGallary());

        Destroy(texture);
    }

    #region Legacy
    //XPCpuImage는 유니티 오브젝트 제외하고 정말 카메라 화면만 나옴
    //public void CaptureImage(ARCameraManager arCamera)
    //{
    //    if (!arCamera.TryAcquireLatestCpuImage(out XRCpuImage image))
    //    {
    //        Debug.LogWarning("Failed to acquire CPU image.");
    //        return;
    //    }

    //    StartCoroutine(CoCaptureImage(image));
    //}

    //TODO : 팝업 이미지 띄우기 + 실제 갤러리에 저장하기
    /// <summary>
    /// CPU이미지를 Texture2D로 변환
    /// </summary>
    //private IEnumerator CoCaptureImage(XRCpuImage cpuImage)
    //{
    //    // Texture2D format으로 변경 (conversionParams -> convert): 아래 참고 문서
    //    // @TK : CPUImage -> buffer로 rawData -> Texture2D -> png
    //    // "https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.0/api/UnityEngine.XR.ARSubsystems.XRCpuImage.html#UnityEngine_XR_ARSubsystems_XRCpuImage_Convert_UnityEngine_XR_ARSubsystems_XRCpuImage_ConversionParams_Unity_Collections_NativeSlice_System_Byte__"
    //    var conversionParams = new XRCpuImage.ConversionParams
    //    {
    //        inputRect = new RectInt(0, 0, cpuImage.width, cpuImage.height),
    //        outputDimensions = new Vector2Int(cpuImage.width, cpuImage.height),
    //        outputFormat = TextureFormat.RGBA32,
    //        transformation = XRCpuImage.Transformation.MirrorY // AR camera usually mirrored
    //    };
    //    yield return null;

    //    int size = cpuImage.GetConvertedDataSize(conversionParams);
    //    var buffer = new NativeArray<byte>(size, Allocator.Temp);
    //    cpuImage.Convert(conversionParams, buffer);
    //    cpuImage.Dispose();

    //    Texture2D texture = new Texture2D(conversionParams.outputDimensions.x, conversionParams.outputDimensions.y, conversionParams.outputFormat, false);
    //    texture.LoadRawTextureData(buffer);
    //    texture.Apply();

    //    buffer.Dispose();

    //    //TODO : UI 팝업 띄우기
    //    //TODO : 이거 IOS에도 저장되게 하기
    //    string fileName = $"ARCapture_{DateTime.Now:yyyyMMdd_HHmmss}.png";
    //    string galleryPath = Path.Combine("/storage/emulated/0/DCIM/Camera", fileName);
    //    ScreenCapture.CaptureScreenshot(fileName);
    //    File.WriteAllBytes(galleryPath, texture.EncodeToPNG());
    //    PictureUtil.SaveImageToGallery(galleryPath);
    //}
    #endregion
}
