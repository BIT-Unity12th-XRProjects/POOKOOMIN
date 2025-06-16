using UnityEngine;


namespace Pookoomin.UI
{
    public class UIUserWorkDataController : UIController<UIUserWorkDataView, UserWorkData>
    {
        public UIUserWorkDataController(UIUserWorkDataView view, UserWorkData model) : base(view, model)
        {
        }

        public override void BindModelToView()
        {
            model.stepCount.PropertyChanged += (sender, value) => {
                view.OnStepUpdated(value);
            };

            model.coin.PropertyChanged += (sender, value) => {
                view.OnCoinUpdated(value);
            };
        }
    }
}
    
