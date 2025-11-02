using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BreakInfinity;

namespace PunchKing
{
    public class PrestigePanel : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI currentPPText;
        public TextMeshProUGUI prestigeLevelText;
        public TextMeshProUGUI nextPPGainText;
        public TextMeshProUGUI lifetimeGoldText;
        public TextMeshProUGUI warningText;
        public Button prestigeButton;
        public GameObject confirmPanel;
        public Button confirmYesButton;
        public Button confirmNoButton;

        [Header("설정")]
        public float updateInterval = 0.2f;

        private float updateTimer = 0f;

        void Start()
        {
            if (prestigeButton != null)
            {
                prestigeButton.onClick.AddListener(OnPrestigeButtonClicked);
            }

            if (confirmYesButton != null)
            {
                confirmYesButton.onClick.AddListener(OnConfirmYes);
            }

            if (confirmNoButton != null)
            {
                confirmNoButton.onClick.AddListener(OnConfirmNo);
            }

            if (confirmPanel != null)
            {
                confirmPanel.SetActive(false);
            }

            UpdateUI();
        }

        void Update()
        {
            updateTimer += Time.deltaTime;

            if (updateTimer >= updateInterval)
            {
                UpdateUI();
                updateTimer = 0f;
            }
        }

        void UpdateUI()
        {
            if (PrestigeManager.Instance == null)
                return;

            // 현재 PP
            if (currentPPText != null)
            {
                currentPPText.SetText(string.Format("Current PP: {0}",
                    NumberFormatter.FormatNumber(PrestigeManager.Instance.prestigePoints)));
            }

            // 프레스티지 레벨
            if (prestigeLevelText != null)
            {
                prestigeLevelText.SetText(string.Format("Prestige Level: {0}",
                    PrestigeManager.Instance.prestigeLevel));
            }

            // 다음 PP 획득량
            BigDouble nextGain = PrestigeManager.Instance.GetPrestigeGain();
            if (nextPPGainText != null)
            {
                if (nextGain > 0)
                {
                    nextPPGainText.SetText(string.Format("Next Gain: +{0} PP",
                        NumberFormatter.FormatNumber(nextGain)));
                    nextPPGainText.color = Color.green;
                }
                else
                {
                    nextPPGainText.SetText("Not enough progress");
                    nextPPGainText.color = Color.gray;
                }
            }

            // 라이프타임 골드
            if (lifetimeGoldText != null)
            {
                lifetimeGoldText.SetText(string.Format("Lifetime Gold: {0}",
                    NumberFormatter.FormatNumber(PrestigeManager.Instance.lifetimeGold)));
            }

            // 경고 텍스트
            if (warningText != null)
            {
                warningText.SetText("Warning: All progress will be reset!\nYou will keep Prestige Points.");
            }

            // 버튼 활성화
            bool canPrestige = PrestigeManager.Instance.CanPrestige();
            if (prestigeButton != null)
            {
                prestigeButton.interactable = canPrestige;
            }
        }

        void OnPrestigeButtonClicked()
        {
            if (confirmPanel != null)
            {
                confirmPanel.SetActive(true);
            }
            else
            {
                // 확인 패널이 없으면 바로 프레스티지
                DoPrestige();
            }
        }

        void OnConfirmYes()
        {
            DoPrestige();

            if (confirmPanel != null)
            {
                confirmPanel.SetActive(false);
            }
        }

        void OnConfirmNo()
        {
            if (confirmPanel != null)
            {
                confirmPanel.SetActive(false);
            }
        }

        void DoPrestige()
        {
            if (PrestigeManager.Instance != null)
            {
                PrestigeManager.Instance.DoPrestige();
                UpdateUI();

                // 패널 닫기 (선택사항)
                // gameObject.SetActive(false);
            }
        }

        void OnEnable()
        {
            UpdateUI();
        }
    }
}
