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
            Debug.Log($"Model is null? : {model == null}");
            Debug.Log($"ThumnailImage is null? : {model?.ThumbnailImage == null}");
            
            //model.onChangedThumbnail += view.SetAlbumImage;
            //model.ThumbnailImage = PictureUtil.LoadThumbnailToGallary();
        }

        public void OnClickReloadButton()
        {
            //TODO : 오브젝트 정렬
        }

        public void OnClickCameraButton()
        {
            arCamera?.CaptureImage();

            //arCamera?.CaptureImage((texture) =>
            //{
            //    model.ThumbnailImage = texture;
            //});
        }

        public void OnClickAlbumButton()
        {
            //TODO : 앨범 접근
        }
    }
}

