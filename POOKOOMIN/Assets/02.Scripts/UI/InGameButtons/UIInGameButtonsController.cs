using FoodyGo.Managers;
using Pookoomin.Manager;
using UnityEngine;

namespace Pookoomin.UI
{
    public class UIInGameButtonsController : UIController<UIInGamebuttonsView, object>
    {
        public UIInGameButtonsController(UIInGamebuttonsView view, object model) : base(view, null) { }

        ARContentManager _contentManager;

        public override void BindInputEvents()
        {
            GameManager.instance.onChangeGameState += OnChangeGameState;

            view.BackButton.onClick.RemoveAllListeners();
            view.SortButton.onClick.RemoveAllListeners();
            view.ARCameraButton.onClick.RemoveAllListeners();

            view.BackButton.onClick.AddListener(OnClickBackButton);
            view.SortButton.onClick.AddListener(OnClickSortButton);
            view.ARCameraButton.onClick.AddListener(OnClickARCameraButton);
        }

        public void OnClickBackButton()
        {
            SoundManager.instance.PlaySFX(SFX.Click);
            GameManager.instance.ChangeBackState();
        }

        public void OnClickSortButton()
        {
            SoundManager.instance.PlaySFX(SFX.SqueakerToy);
            //TODO : 펫 정렬시키기
            if (_contentManager == null)
                _contentManager = Object.FindFirstObjectByType<ARContentManager>();

            _contentManager.PlaceObjectAtCenter();
        }

        public void OnClickARCameraButton()
        {
            SoundManager.instance.PlaySFX(SFX.Click);
            //현재 GameManager의 namespace가 FoodyGo로 되어있음 추후 수정 필요
            GameManager.instance.ChangeGameState(GameState.ARCamera);
        }

        public void OnChangeGameState(GameState state)
        {
            if (state == GameState.ARCamera)
            {
                view.SortButton.interactable = true;
                view.ARCameraButton.interactable = false;
            }
            else if (state == GameState.Lobby || state == GameState.Walk)
            {
                view.SortButton.interactable = false;
                view.ARCameraButton.interactable = true;
            }
        }
    }
}

