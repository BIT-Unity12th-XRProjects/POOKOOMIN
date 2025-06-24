using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pookoomin.UI
{
    public class UIUserWorkDataView : UIView
    {
        [SerializeField] private Image stepIcon;

        [SerializeField] private TextMeshProUGUI stepHeaderText;
        [SerializeField] private TextMeshProUGUI stepValueText;
        [SerializeField] private TextMeshProUGUI currentCoinHeaderText;
        [SerializeField] private TextMeshProUGUI currentCoinValueText;

        public void InitUIUserWorkData(UserWorkData userData)
        {
            stepValueText.text = userData.stepCount.Value.ToString();
            currentCoinValueText.text = userData.coin.Value.ToString();
        }

        public void OnStepUpdated(int currentStep)
        {
            stepValueText.text = currentStep.ToString();
        }

        public void OnCoinUpdated(int currentCoin)
        {
            currentCoinValueText.text = currentCoin.ToString();
        }
    }
}
