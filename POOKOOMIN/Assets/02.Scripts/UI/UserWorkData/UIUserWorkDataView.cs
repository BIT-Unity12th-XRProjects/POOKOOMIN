using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pookoomin.UI
{
    public class UIUserWorkDataView : UIView
    {
        [SerializeField] private Image stepIcon;
        [SerializeField] private Image coinIcon;

        [SerializeField] private TextMeshProUGUI stepHeaderText;
        [SerializeField] private TextMeshProUGUI stepValueText;
        [SerializeField] private TextMeshProUGUI currentCoinHeaderText;
        [SerializeField] private TextMeshProUGUI nextCoinHeaderText;
        [SerializeField] private TextMeshProUGUI currentCoinValueText;
        [SerializeField] private TextMeshProUGUI nextCoinValueText;

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
