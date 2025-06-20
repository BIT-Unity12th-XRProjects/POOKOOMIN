using UnityEngine;
using UnityEngine.UI;

namespace Pookoomin.UI
{
    public class UIInGamebuttonsView : UIView
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button sortButton;
        [SerializeField] private Button arCameraButton;

        public Button BackButton => backButton;
        public Button SortButton => sortButton;
        public Button ARCameraButton => arCameraButton;
    }
}

