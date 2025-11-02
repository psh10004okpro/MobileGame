using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BreakInfinity;

namespace PunchKing
{
    public class UpgradeButton : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI costText;
        public TextMeshProUGUI descriptionText;
        public Image iconImage;
        public Button button;

        [Header("Data")]
        public UpgradeData upgradeData;

        private int currentLevel = 0;

        void Start()
        {
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClicked);
            }

            UpdateUI();
        }

        void Update()
        {
            // 매 프레임 구매 가능 여부 체크
            if (UpgradeManager.Instance != null && upgradeData != null)
            {
                bool canAfford = UpgradeManager.Instance.CanAffordUpgrade(upgradeData);
                if (button != null)
                {
                    button.interactable = canAfford && currentLevel < upgradeData.maxLevel;
                }
            }
        }

        void OnButtonClicked()
        {
            if (UpgradeManager.Instance != null && upgradeData != null)
            {
                if (UpgradeManager.Instance.TryPurchaseUpgrade(upgradeData))
                {
                    UpdateUI();
                }
            }
        }

        public void UpdateUI()
        {
            if (upgradeData == null)
                return;

            currentLevel = UpgradeManager.Instance != null ? UpgradeManager.Instance.GetUpgradeLevel(upgradeData) : 0;

            // 이름
            if (nameText != null)
            {
                nameText.SetText(upgradeData.upgradeName);
            }

            // 레벨
            if (levelText != null)
            {
                levelText.SetText(string.Format("Lv.{0}/{1}", currentLevel, upgradeData.maxLevel));
            }

            // 비용
            if (costText != null)
            {
                BigDouble cost = upgradeData.GetCostAtLevel(currentLevel);
                costText.SetText(string.Format("Cost: {0}", NumberFormatter.FormatNumber(cost)));
            }

            // 설명
            if (descriptionText != null)
            {
                descriptionText.SetText(upgradeData.description);
            }

            // 아이콘
            if (iconImage != null && upgradeData.icon != null)
            {
                iconImage.sprite = upgradeData.icon;
            }
        }
    }
}
