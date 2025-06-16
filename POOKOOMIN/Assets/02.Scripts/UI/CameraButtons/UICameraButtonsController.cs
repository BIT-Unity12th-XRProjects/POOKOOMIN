using UnityEngine;

namespace Pookoomin.UI
{
    public class UICameraButtonsController : UIController<UICameraButtonsView, object>
    {
        public UICameraButtonsController(UICameraButtonsView view, object model) : base(view, null) { }

        public override void BindInputEvents()
        {
            view.ReloadButton.onClick.RemoveAllListeners();
            view.CameraButton.onClick.RemoveAllListeners();
            view.AlbumButton.onClick.RemoveAllListeners();

            view.ReloadButton.onClick.AddListener(OnClickReloadButton);
            view.CameraButton.onClick.AddListener(OnClickCameraButton);
            view.AlbumButton.onClick.AddListener(OnClickAlbumButton);
        }

        public void OnClickReloadButton()
        {
            //TODO : 오브젝트 정렬
        }

        public void OnClickCameraButton()
        {
            //TODO : 카메라 촬영
        }

        public void OnClickAlbumButton()
        {
            //TODO : 앨범 접근
        }
    }
}

