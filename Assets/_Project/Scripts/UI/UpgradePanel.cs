using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace PunchKing
{
    public class UpgradePanel : MonoBehaviour
    {
        [Header("UI References")]
        public Transform upgradeButtonContainer;
        public GameObject upgradeButtonPrefab;
        public Button closeButton;

        [Header("설정")]
        public bool autoGenerateButtons = true;

        private List<UpgradeButton> upgradeButtons = new List<UpgradeButton>();

        void Start()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseButtonClicked);
            }

            if (autoGenerateButtons)
            {
                GenerateUpgradeButtons();
            }
        }

        public void GenerateUpgradeButtons()
        {
            if (UpgradeManager.Instance == null || upgradeButtonContainer == null || upgradeButtonPrefab == null)
            {
                Debug.LogWarning("UpgradePanel: 필수 참조가 누락되었습니다.");
                return;
            }

            // 기존 버튼 제거
            ClearButtons();

            // 각 업그레이드에 대해 버튼 생성
            foreach (var upgradeData in UpgradeManager.Instance.upgradeDataList)
            {
                if (upgradeData == null)
                    continue;

                GameObject buttonObj = Instantiate(upgradeButtonPrefab, upgradeButtonContainer);
                UpgradeButton upgradeButton = buttonObj.GetComponent<UpgradeButton>();

                if (upgradeButton != null)
                {
                    upgradeButton.upgradeData = upgradeData;
                    upgradeButton.UpdateUI();
                    upgradeButtons.Add(upgradeButton);
                }
            }

            Debug.Log(string.Format("UpgradePanel: {0}개의 업그레이드 버튼 생성됨", upgradeButtons.Count));
        }

        public void ClearButtons()
        {
            foreach (var button in upgradeButtons)
            {
                if (button != null)
                {
                    Destroy(button.gameObject);
                }
            }
            upgradeButtons.Clear();
        }

        public void RefreshButtons()
        {
            foreach (var button in upgradeButtons)
            {
                if (button != null)
                {
                    button.UpdateUI();
                }
            }
        }

        void OnCloseButtonClicked()
        {
            gameObject.SetActive(false);
        }

        void OnEnable()
        {
            RefreshButtons();
        }
    }
}
