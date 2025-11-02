using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BreakInfinity;
using System;

namespace PunchKing
{
    public class DailyRewardPanel : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI rewardAmountText;
        public TextMeshProUGUI consecutiveDaysText;
        public TextMeshProUGUI nextRewardTimeText;
        public Button claimButton;
        public Button closeButton;

        [Header("일일 보상 표시 (7일)")]
        public GameObject[] dayRewardObjects = new GameObject[7];

        void Start()
        {
            if (claimButton != null)
            {
                claimButton.onClick.AddListener(OnClaimButtonClicked);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseButtonClicked);
            }

            UpdateUI();
        }

        void Update()
        {
            UpdateUI();
        }

        void UpdateUI()
        {
            if (DailyRewardManager.Instance == null)
                return;

            bool canClaim = DailyRewardManager.Instance.CanClaimReward();

            // 보상 금액
            if (rewardAmountText != null)
            {
                BigDouble reward = DailyRewardManager.Instance.GetNextReward();
                rewardAmountText.SetText(string.Format("Reward: {0} Gold",
                    NumberFormatter.FormatNumber(reward)));
            }

            // 연속 일수
            if (consecutiveDaysText != null)
            {
                int days = DailyRewardManager.Instance.GetConsecutiveDays();
                consecutiveDaysText.SetText(string.Format("Day {0}/7", days));
            }

            // 다음 보상까지 남은 시간
            if (nextRewardTimeText != null)
            {
                if (canClaim)
                {
                    nextRewardTimeText.SetText("Available Now!");
                    nextRewardTimeText.color = Color.green;
                }
                else
                {
                    TimeSpan timeUntil = DailyRewardManager.Instance.GetTimeUntilNextReward();
                    nextRewardTimeText.SetText(string.Format("Next in: {0:D2}:{1:D2}:{2:D2}",
                        timeUntil.Hours, timeUntil.Minutes, timeUntil.Seconds));
                    nextRewardTimeText.color = Color.gray;
                }
            }

            // 버튼 활성화
            if (claimButton != null)
            {
                claimButton.interactable = canClaim;
            }

            // 7일 보상 표시 업데이트
            UpdateDayRewardDisplay();
        }

        void UpdateDayRewardDisplay()
        {
            int currentDay = DailyRewardManager.Instance.GetConsecutiveDays();

            for (int i = 0; i < dayRewardObjects.Length; i++)
            {
                if (dayRewardObjects[i] != null)
                {
                    // i+1일차가 현재 일수보다 작거나 같으면 활성화
                    bool isCompleted = (i + 1) <= currentDay;

                    // 색상이나 체크마크 표시 (Image 컴포넌트 사용)
                    Image image = dayRewardObjects[i].GetComponent<Image>();
                    if (image != null)
                    {
                        image.color = isCompleted ? Color.green : Color.gray;
                    }
                }
            }
        }

        void OnClaimButtonClicked()
        {
            if (DailyRewardManager.Instance != null && DailyRewardManager.Instance.CanClaimReward())
            {
                DailyRewardManager.Instance.ClaimDailyReward();
                UpdateUI();

                // 보상 획득 효과 (선택사항)
                Debug.Log("일일 보상 획득!");
            }
        }

        void OnCloseButtonClicked()
        {
            gameObject.SetActive(false);
        }

        void OnEnable()
        {
            UpdateUI();

            // 패널이 열릴 때 자동으로 받을 수 있으면 표시
            if (DailyRewardManager.Instance != null && DailyRewardManager.Instance.CanClaimReward())
            {
                Debug.Log("일일 보상을 받을 수 있습니다!");
            }
        }
    }
}
