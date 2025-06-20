using FoodyGo.Managers;
using UnityEngine;

namespace Pookoomin.UI
{
    public class UIInGameButtonsController : UIController<UIInGamebuttonsView, object>
    {
        public UIInGameButtonsController(UIInGamebuttonsView view, object model) : base(view, null) { }

        public override void BindInputEvents()
        {
            view.BackButton.onClick.RemoveAllListeners();
            view.SortButton.onClick.RemoveAllListeners();
            view.ARCameraButton.onClick.RemoveAllListeners();

            view.BackButton.onClick.AddListener(OnClickBackButton);
            view.SortButton.onClick.AddListener(OnClickSortButton);
            view.ARCameraButton.onClick.AddListener(OnClickARCameraButton);
        }

        public void OnClickBackButton()
        {
            //TODO : 이전 상태(모드)로 돌아가기
            GameManager.instance.ChangeBackState();
        }

        public void OnClickSortButton()
        {
            //TODO : 펫 정렬시키기
        }

        public void OnClickARCameraButton()
        {
            //TODO : AR카메라로 전환
            //현재모드를 AR카메라 모드로 바꾸기
            //현재 GameManager의 namespace가 FoodyGo로 되어있음 추후 수정 필요
            GameManager.instance.ChangeGameState(GameState.ARCamera);
        }
    }
}

