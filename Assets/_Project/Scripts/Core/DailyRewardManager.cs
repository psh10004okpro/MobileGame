using UnityEngine;
using BreakInfinity;
using System;

namespace PunchKing
{
    public class DailyRewardManager : MonoBehaviour
    {
        public static DailyRewardManager Instance { get; private set; }

        [Header("일일 보상 설정")]
        public BigDouble baseReward = 1000;
        public float rewardMultiplier = 2f;  // 연속 일수마다 2배
        public int maxConsecutiveDays = 7;   // 최대 7일

        private int consecutiveDays = 0;
        private DateTime lastClaimDate;
        private bool canClaim = false;

        // 이벤트
        public event Action<BigDouble, int> OnDailyRewardClaimed;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            LoadDailyRewardData();
            CheckDailyReward();
        }

        public void CheckDailyReward()
        {
            DateTime today = DateTime.Today;
            TimeSpan timeSinceLastClaim = today - lastClaimDate;

            if (timeSinceLastClaim.Days == 1)
            {
                // 연속 출석
                consecutiveDays++;
                if (consecutiveDays > maxConsecutiveDays)
                {
                    consecutiveDays = maxConsecutiveDays;
                }
                canClaim = true;
            }
            else if (timeSinceLastClaim.Days > 1)
            {
                // 연속 출석 끊김
                consecutiveDays = 1;
                canClaim = true;
            }
            else if (timeSinceLastClaim.Days == 0)
            {
                // 오늘 이미 받음
                canClaim = false;
            }
            else
            {
                // 첫 방문
                consecutiveDays = 1;
                canClaim = true;
            }

            Debug.Log($"Daily Reward - Consecutive Days: {consecutiveDays}, Can Claim: {canClaim}");
        }

        public bool CanClaimReward()
        {
            return canClaim;
        }

        public void ClaimDailyReward()
        {
            if (!canClaim)
            {
                Debug.Log("오늘 이미 일일 보상을 받았습니다!");
                return;
            }

            BigDouble reward = CalculateReward();

            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddGold(reward);
            }

            lastClaimDate = DateTime.Today;
            canClaim = false;

            SaveDailyRewardData();

            // 이벤트 발동
            OnDailyRewardClaimed?.Invoke(reward, consecutiveDays);

            Debug.Log($"일일 보상 획득! {reward} Gold (연속 {consecutiveDays}일)");
        }

        BigDouble CalculateReward()
        {
            return baseReward * BigDouble.Pow(rewardMultiplier, consecutiveDays - 1);
        }

        public BigDouble GetNextReward()
        {
            return CalculateReward();
        }

        public int GetConsecutiveDays()
        {
            return consecutiveDays;
        }

        public TimeSpan GetTimeUntilNextReward()
        {
            DateTime tomorrow = DateTime.Today.AddDays(1);
            return tomorrow - DateTime.Now;
        }

        void SaveDailyRewardData()
        {
            PlayerPrefs.SetInt("DailyReward_ConsecutiveDays", consecutiveDays);
            PlayerPrefs.SetString("DailyReward_LastClaimDate", lastClaimDate.ToString("o"));
            PlayerPrefs.Save();
        }

        void LoadDailyRewardData()
        {
            consecutiveDays = PlayerPrefs.GetInt("DailyReward_ConsecutiveDays", 0);
            string dateString = PlayerPrefs.GetString("DailyReward_LastClaimDate", "");

            if (!string.IsNullOrEmpty(dateString))
            {
                try
                {
                    lastClaimDate = DateTime.Parse(dateString);
                }
                catch
                {
                    lastClaimDate = DateTime.MinValue;
                }
            }
            else
            {
                lastClaimDate = DateTime.MinValue;
            }
        }
    }
}
