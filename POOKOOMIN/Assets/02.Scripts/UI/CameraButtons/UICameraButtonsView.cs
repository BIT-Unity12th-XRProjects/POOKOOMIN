using FoodyGo.Utils.DI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace Pookoomin.UI
{
    public class UICameraButtonsView : UIView
    {
        [SerializeField] private Button reloadButton;
        [SerializeField] private Button cameraButton;
        [SerializeField] private Button albumButton;
        [SerializeField] private RawImage albumImage; //First gallary in Image

        public Button ReloadButton => reloadButton;
        public Button CameraButton => cameraButton;
        public Button AlbumButton => albumButton;

        public void SetAlbumImage(Texture2D texture)
        {
            if (texture == null)
            {
                albumImage.texture = null;
                return;
            }
            albumImage.texture = texture;
        }
    }
}

