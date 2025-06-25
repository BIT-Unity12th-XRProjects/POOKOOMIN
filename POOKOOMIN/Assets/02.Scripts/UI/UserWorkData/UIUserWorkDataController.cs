using FoodyGo.Utils.DI;
using UnityEngine;


namespace Pookoomin.UI
{
    public class UIUserWorkDataController : UIController<UIUserWorkDataView, UserWorkData>
    {
        private GoogleFitService _googleFitService;

        public UIUserWorkDataController(UIUserWorkDataView view, UserWorkData model) : base(view, model)
        {
            //TODO : 이거 DI로 주입 
            _googleFitService = Object.FindAnyObjectByType<GoogleFitService>();
        }

        public override void BindModelToView()
        {
            model.stepCount.PropertyChanged += (sender, value) => {
                view.OnStepUpdated(value);
            };

            //model.coin.PropertyChanged += (sender, value) => {
            //    view.OnCoinUpdated(value);
            //};
        }
    }
}
    
