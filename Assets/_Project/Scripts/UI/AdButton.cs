using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using PunchKing.Monetization;

namespace PunchKing
{
    /// <summary>
    /// 광고 보상 타입
    /// </summary>
    public enum AdRewardType
    {
        DoubleGold,         // 골드 2배 (30분)
        DoubleDPS,          // DPS 2배 (1시간)
        TripleGold,         // 골드 3배 (30분)
        InstantGold,        // 즉시 골드 (DPS x 3600)
        ResetSkillCooldown, // 스킬 쿨다운 리셋
        DoubleOfflineReward // 오프라인 보상 2배
    }

    /// <summary>
    /// 광고 시청 보상 버튼
    /// </summary>
    public class AdButton : MonoBehaviour
    {
        [Header("UI References")]
        public Button button;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI timerText;
        public GameObject activeIndicator;

        [Header("광고 보상 설정")]
        public AdRewardType rewardType;
        public float duration = 1800f; // 30분 (초 단위)
        public float multiplier = 2f;

        [Header("쿨다운")]
        public float cooldown = 0f; // 0이면 쿨다운 없음
        private float cooldownTimer = 0f;

        private bool isRewardActive = false;
        private float rewardTimer = 0f;

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
            // 쿨다운 처리
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
            }

            // 보상 지속 시간 처리
            if (isRewardActive)
            {
                rewardTimer -= Time.deltaTime;

                if (rewardTimer <= 0)
                {
                    DeactivateReward();
                }
            }

            UpdateUI();
        }

        void OnButtonClicked()
        {
            if (AdsManager.Instance == null)
            {
                Debug.LogWarning("AdsManager가 없습니다!");
                return;
            }

            // 쿨다운 체크
            if (cooldownTimer > 0)
            {
                Debug.Log($"쿨다운 중: {cooldownTimer:F0}초 남음");
                return;
            }

            // 이미 활성화된 보상 체크
            if (isRewardActive)
            {
                Debug.Log("이미 보상이 활성화되어 있습니다!");
                return;
            }

            // 광고 시청
            AdsManager.Instance.ShowRewardedAd(OnAdWatched);
        }

        void OnAdWatched(bool success)
        {
            if (!success)
            {
                Debug.Log("광고 시청 실패");
                return;
            }

            // 보상 지급
            ActivateReward();

            // 쿨다운 시작
            if (cooldown > 0)
            {
                cooldownTimer = cooldown;
            }
        }

        void ActivateReward()
        {
            Debug.Log($"광고 보상 활성화: {rewardType}");

            switch (rewardType)
            {
                case AdRewardType.DoubleGold:
                    ApplyGoldMultiplier(2f);
                    break;

                case AdRewardType.DoubleDPS:
                    ApplyDPSMultiplier(2f);
                    break;

                case AdRewardType.TripleGold:
                    ApplyGoldMultiplier(3f);
                    break;

                case AdRewardType.InstantGold:
                    GiveInstantGold();
                    break;

                case AdRewardType.ResetSkillCooldown:
                    ResetAllSkillCooldowns();
                    break;

                case AdRewardType.DoubleOfflineReward:
                    // 다음 오프라인 보상 2배 플래그 설정
                    PlayerPrefs.SetInt("AdReward_DoubleOffline", 1);
                    break;
            }

            // 지속 시간이 있는 보상
            if (duration > 0)
            {
                isRewardActive = true;
                rewardTimer = duration;
            }
        }

        void DeactivateReward()
        {
            Debug.Log($"광고 보상 만료: {rewardType}");

            switch (rewardType)
            {
                case AdRewardType.DoubleGold:
                    RemoveGoldMultiplier(2f);
                    break;

                case AdRewardType.DoubleDPS:
                    RemoveDPSMultiplier(2f);
                    break;

                case AdRewardType.TripleGold:
                    RemoveGoldMultiplier(3f);
                    break;
            }

            isRewardActive = false;
        }

        #region Reward Methods

        void ApplyGoldMultiplier(float mult)
        {
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.goldMultiplier *= mult;
            }
        }

        void RemoveGoldMultiplier(float mult)
        {
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.goldMultiplier /= mult;
            }
        }

        void ApplyDPSMultiplier(float mult)
        {
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.productionMultiplier *= mult;
            }
        }

        void RemoveDPSMultiplier(float mult)
        {
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.productionMultiplier /= mult;
            }
        }

        void GiveInstantGold()
        {
            if (CurrencyManager.Instance != null)
            {
                BreakInfinity.BigDouble instantGold = CurrencyManager.Instance.goldPerSecond * 3600; // 1시간치
                CurrencyManager.Instance.AddGold(instantGold);
                Debug.Log($"즉시 골드 획득: {NumberFormatter.FormatNumber(instantGold)}");
            }
        }

        void ResetAllSkillCooldowns()
        {
            if (SkillSystem.Instance != null)
            {
                foreach (var skill in SkillSystem.Instance.skills)
                {
                    skill.cooldownTimer = 0f;
                }
                Debug.Log("모든 스킬 쿨다운 리셋!");
            }
        }

        #endregion

        void UpdateUI()
        {
            bool canWatch = cooldownTimer <= 0 && !isRewardActive;

            // 버튼 활성화
            if (button != null)
            {
                button.interactable = canWatch;
            }

            // 타이머 텍스트
            if (timerText != null)
            {
                if (isRewardActive)
                {
                    timerText.SetText(string.Format("Active: {0}", NumberFormatter.FormatTime(rewardTimer)));
                    timerText.color = Color.green;
                }
                else if (cooldownTimer > 0)
                {
                    timerText.SetText(string.Format("Cooldown: {0}", NumberFormatter.FormatTime(cooldownTimer)));
                    timerText.color = Color.red;
                }
                else
                {
                    timerText.SetText("Watch Ad!");
                    timerText.color = Color.white;
                }
            }

            // 활성화 표시
            if (activeIndicator != null)
            {
                activeIndicator.SetActive(isRewardActive);
            }

            // 제목 및 설명
            UpdateTextUI();
        }

        void UpdateTextUI()
        {
            switch (rewardType)
            {
                case AdRewardType.DoubleGold:
                    if (titleText != null) titleText.SetText("Gold x2");
                    if (descriptionText != null) descriptionText.SetText("30 minutes");
                    break;

                case AdRewardType.DoubleDPS:
                    if (titleText != null) titleText.SetText("DPS x2");
                    if (descriptionText != null) descriptionText.SetText("1 hour");
                    break;

                case AdRewardType.TripleGold:
                    if (titleText != null) titleText.SetText("Gold x3");
                    if (descriptionText != null) descriptionText.SetText("30 minutes");
                    break;

                case AdRewardType.InstantGold:
                    if (titleText != null) titleText.SetText("Instant Gold");
                    if (descriptionText != null) descriptionText.SetText("DPS x 1 hour");
                    break;

                case AdRewardType.ResetSkillCooldown:
                    if (titleText != null) titleText.SetText("Reset Skills");
                    if (descriptionText != null) descriptionText.SetText("All cooldowns");
                    break;
            }
        }
    }
}
