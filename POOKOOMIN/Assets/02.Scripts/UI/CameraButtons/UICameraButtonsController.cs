using Pookoomin.Manager;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Pookoomin.UI
{
    public class UICameraButtonsController : UIController<UICameraButtonsView, GallaryData>
    {
        private ARCameraController arCamera;

        public UICameraButtonsController(UICameraButtonsView view, GallaryData gallaryData) : base(view, gallaryData) 
        {
            arCamera = Object.FindAnyObjectByType<ARCameraController>();
            
        }

        public override void BindInputEvents()
        {
            view.ReloadButton.onClick.RemoveAllListeners();
            view.CameraButton.onClick.RemoveAllListeners();
            view.AlbumButton.onClick.RemoveAllListeners();

            view.ReloadButton.onClick.AddListener(OnClickReloadButton);
            view.CameraButton.onClick.AddListener(OnClickCameraButton);
            view.AlbumButton.onClick.AddListener(OnClickAlbumButton);
        }

        public override void BindModelToView()
        {
            model.onChangedThumbnail += view.SetAlbumImage;
            model.ThumbnailImage = PictureUtil.LoadThumbnailToGallary();
        }

        public void OnClickReloadButton()
        {
            //TODO : object(pet) sort
        }

        public void OnClickCameraButton()
        {
            SoundManager.instance.PlaySFX(SFX.CameraFlash);

            arCamera?.CaptureImage((texture) =>
            {
                model.ThumbnailImage = texture;
            });
        }

        public void OnClickAlbumButton()
        {
            SoundManager.instance.PlaySFX(SFX.Click2);
            PictureUtil.OpenGallery();
        }
    }
}

