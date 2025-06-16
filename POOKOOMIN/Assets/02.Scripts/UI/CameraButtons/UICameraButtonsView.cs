using UnityEngine;
using UnityEngine.UI;

namespace Pookoomin.UI
{
    public class UICameraButtonsView : UIView
    {
        [SerializeField] private Button reloadButton;
        [SerializeField] private Button cameraButton;
        [SerializeField] private Button albumButton;

        public Button ReloadButton => reloadButton;
        public Button CameraButton => cameraButton;
        public Button AlbumButton => albumButton;

    }
}

