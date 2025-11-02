using UnityEngine;
using System;

namespace PunchKing.Monetization
{
    /// <summary>
    /// VIP 시스템 관리자
    /// VIP 혜택, 기간 관리
    /// </summary>
    public class VIPManager : MonoBehaviour
    {
        public static VIPManager Instance { get; private set; }

        [Header("VIP 혜택")]
        public float goldMultiplierBonus = 0.5f;  // +50% 골드
        public float dpsMultiplierBonus = 0.5f;   // +50% DPS
        public float dailyRewardMultiplier = 2f;  // 일일 보상 2배
        public bool removeAds = true;             // 광고 제거

        [Header("VIP 상태")]
        public bool isVIP = false;
        public DateTime vipExpireDate;
        public int vipDaysRemaining = 0;

        // 이벤트
        public event Action OnVIPActivated;
        public event Action OnVIPExpired;

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
            LoadVIPData();
            CheckVIPStatus();
            ApplyVIPBonuses();
        }

        void Update()
        {
            // 매 프레임마다 VIP 만료 체크 (1시간마다 체크하는 것이 더 효율적)
            if (isVIP && Time.frameCount % (60 * 60 * 30) == 0) // 30 FPS 기준 1시간
            {
                CheckVIPStatus();
            }
        }

        /// <summary>
        /// VIP 활성화
        /// </summary>
        public void ActivateVIP(int days)
        {
            if (isVIP)
            {
                // 이미 VIP면 기간 연장
                vipExpireDate = vipExpireDate.AddDays(days);
            }
            else
            {
                // 새로 VIP 활성화
                isVIP = true;
                vipExpireDate = DateTime.Now.AddDays(days);
                OnVIPActivated?.Invoke();
            }

            UpdateVIPDaysRemaining();
            ApplyVIPBonuses();
            SaveVIPData();

            Debug.Log($"VIP 활성화! 만료일: {vipExpireDate:yyyy-MM-dd}");
        }

        /// <summary>
        /// VIP 상태 확인
        /// </summary>
        void CheckVIPStatus()
        {
            if (isVIP)
            {
                UpdateVIPDaysRemaining();

                // VIP 만료 확인
                if (DateTime.Now >= vipExpireDate)
                {
                    ExpireVIP();
                }
            }
        }

        /// <summary>
        /// VIP 만료
        /// </summary>
        void ExpireVIP()
        {
            isVIP = false;
            vipDaysRemaining = 0;
            RemoveVIPBonuses();
            OnVIPExpired?.Invoke();
            SaveVIPData();

            Debug.Log("VIP가 만료되었습니다.");
        }

        /// <summary>
        /// 남은 일수 업데이트
        /// </summary>
        void UpdateVIPDaysRemaining()
        {
            if (isVIP)
            {
                TimeSpan timeRemaining = vipExpireDate - DateTime.Now;
                vipDaysRemaining = (int)Math.Ceiling(timeRemaining.TotalDays);
            }
        }

        /// <summary>
        /// VIP 혜택 적용
        /// </summary>
        void ApplyVIPBonuses()
        {
            if (!isVIP || CurrencyManager.Instance == null)
                return;

            // 골드 배율 증가
            CurrencyManager.Instance.goldMultiplier *= (1 + goldMultiplierBonus);

            // DPS 배율 증가
            CurrencyManager.Instance.productionMultiplier *= (1 + dpsMultiplierBonus);

            Debug.Log($"VIP 혜택 적용: 골드 +{goldMultiplierBonus * 100}%, DPS +{dpsMultiplierBonus * 100}%");
        }

        /// <summary>
        /// VIP 혜택 제거
        /// </summary>
        void RemoveVIPBonuses()
        {
            if (CurrencyManager.Instance == null)
                return;

            // 골드 배율 원복
            CurrencyManager.Instance.goldMultiplier /= (1 + goldMultiplierBonus);

            // DPS 배율 원복
            CurrencyManager.Instance.productionMultiplier /= (1 + dpsMultiplierBonus);

            Debug.Log("VIP 혜택 제거됨");
        }

        #region Query Methods

        public bool IsVIP()
        {
            return isVIP;
        }

        public bool HasAdRemoval()
        {
            return isVIP && removeAds;
        }

        public int GetDaysRemaining()
        {
            return vipDaysRemaining;
        }

        public string GetVIPExpireDateString()
        {
            if (!isVIP)
                return "비활성";

            return vipExpireDate.ToString("yyyy-MM-dd");
        }

        public float GetDailyRewardMultiplier()
        {
            return isVIP ? dailyRewardMultiplier : 1f;
        }

        #endregion

        #region Save/Load

        void SaveVIPData()
        {
            PlayerPrefs.SetInt("VIP_IsActive", isVIP ? 1 : 0);
            PlayerPrefs.SetString("VIP_ExpireDate", vipExpireDate.ToString("o")); // ISO 8601 형식
            PlayerPrefs.Save();
        }

        void LoadVIPData()
        {
            isVIP = PlayerPrefs.GetInt("VIP_IsActive", 0) == 1;

            string expireDateString = PlayerPrefs.GetString("VIP_ExpireDate", "");
            if (!string.IsNullOrEmpty(expireDateString))
            {
                try
                {
                    vipExpireDate = DateTime.Parse(expireDateString);
                }
                catch
                {
                    vipExpireDate = DateTime.Now;
                    isVIP = false;
                }
            }
        }

        #endregion

        #region Debug Methods

        [ContextMenu("Debug: VIP 30일 활성화")]
        void DebugActivateVIP30Days()
        {
            ActivateVIP(30);
        }

        [ContextMenu("Debug: VIP 즉시 만료")]
        void DebugExpireVIP()
        {
            ExpireVIP();
        }

        #endregion
    }
}
