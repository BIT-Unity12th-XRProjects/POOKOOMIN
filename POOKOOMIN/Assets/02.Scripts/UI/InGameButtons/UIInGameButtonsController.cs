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
            //TODO : 이전 상태로 돌아가기
            //일단은 ar카메라모드에서 인게임모드로 돌아가는걸로.
            //추후에 이전상태값을 저장해두고 해당 모드에 맞게 오브젝트들 활성화/비활성화 해야 할 것 같음
           
        }

        public void OnClickSortButton()
        {
            //TODO : 펫 정렬시키기
        }

        public void OnClickARCameraButton()
        {
            //TODO : AR카메라로 전환
            //상태를 AR카메라 모드로 바꾸고
            //상태가 전환되면 아래 활동을 수행 해야할것같음
            //일단 필요한 것 : AR카메라모드에 필요한 오브젝트들(활성화), 기본인게임모드에 필요한 오브젝트들(비활성화)
            GameObject ar = GameObject.Find("AR_Objects").transform.GetChild(2).gameObject;

            ar.GetComponent<Camera>().enabled = true;
            Camera.main.enabled = false;
        }
    }
}

