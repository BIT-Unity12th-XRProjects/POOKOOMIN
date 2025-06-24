using System;
using UnityEngine;

/// <summary>
/// 어플리케이션 갤러리 폴더의 데이터 관리
/// </summary>
public class GallaryData 
{
    public Texture2D thumbnailImage;

    public Texture2D ThumbnailImage
    {
        get { return thumbnailImage; }
        set
        {
            if (thumbnailImage != null && thumbnailImage.Equals(value))
                return;

            thumbnailImage = value;
            onChangedThumbnail?.Invoke(value);
        }
    }

    public Action<Texture2D> onChangedThumbnail;

    public GallaryData()
    {
        thumbnailImage = null;
    }
}
