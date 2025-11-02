using UnityEngine;
using BreakInfinity;
using System.Collections.Generic;
using System;

namespace PunchKing
{
    public class PrestigeManager : MonoBehaviour
    {
        public static PrestigeManager Instance { get; private set; }

        [Header("프레스티지")]
        public BigDouble lifetimeGold = 0;
        public BigDouble prestigePoints = 0;
        public int prestigeLevel = 0;

        [Header("영구 업그레이드")]
        public Dictionary<string, int> permanentUpgrades = new Dictionary<string, int>();

        // 이벤트
        public event Action OnPrestigeCompleted;

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

        void Update()
        {
            // 라이프타임 골드 추적
            if (CurrencyManager.Instance != null && CurrencyManager.Instance.gold > lifetimeGold)
            {
                lifetimeGold = CurrencyManager.Instance.gold;
            }
        }

        // 프레스티지 포인트 계산 (제곱근 공식)
        public BigDouble CalculatePrestigePoints()
        {
            // PP = 150 × √(LifetimeGold / 10^15)
            BigDouble divisor = BigDouble.Pow(10, 15);
            if (lifetimeGold < divisor)
                return 0;

            BigDouble result = 150 * BigDouble.Sqrt(lifetimeGold / divisor);
            return result;
        }

        public BigDouble GetPrestigeGain()
        {
            BigDouble newPoints = CalculatePrestigePoints();
            BigDouble gain = newPoints - prestigePoints;
            return BigDouble.Max(0, gain);
        }

        public bool CanPrestige()
        {
            return GetPrestigeGain() > 0;
        }

        public void DoPrestige()
        {
            if (!CanPrestige())
            {
                Debug.Log("프레스티지 이득이 없습니다!");
                return;
            }

            // 프레스티지 포인트 획득
            BigDouble gainedPoints = GetPrestigeGain();
            prestigePoints += gainedPoints;
            prestigeLevel++;

            Debug.Log($"프레스티지 완료! +{gainedPoints} PP");

            // 게임 리셋
            ResetGameState();

            // 영구 보너스 적용
            ApplyPermanentBonuses();

            // 이벤트 발동
            OnPrestigeCompleted?.Invoke();

            // 저장
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
            }
        }

        void ResetGameState()
        {
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.gold = 0;
                CurrencyManager.Instance.goldPerClick = 1;
                CurrencyManager.Instance.goldPerSecond = 0;
                CurrencyManager.Instance.clickMultiplier = 1;
                CurrencyManager.Instance.productionMultiplier = 1;
                CurrencyManager.Instance.goldMultiplier = 1;
                CurrencyManager.Instance.criticalChance = 0.05f;
                CurrencyManager.Instance.criticalDamage = 1.5;
            }

            // 업그레이드 리셋
            if (UpgradeManager.Instance != null)
            {
                foreach (var upgrade in UpgradeManager.Instance.upgradeDataList)
                {
                    if (upgrade != null)
                    {
                        UpgradeManager.Instance.SetUpgradeLevel(upgrade, 0);
                    }
                }
            }

            // 캐릭터 진화 리셋
            if (CharacterEvolution.Instance != null)
            {
                CharacterEvolution.Instance.currentStage = 0;
                CharacterEvolution.Instance.characterLevel = 1;
                CharacterEvolution.Instance.experiencePoints = 0;
            }
        }

        void ApplyPermanentBonuses()
        {
            // 기본 프레스티지 보너스: +2% DPS per prestige point
            if (CurrencyManager.Instance != null && prestigePoints > 0)
            {
                BigDouble dpsBonus = 1 + (prestigePoints * 0.02);
                CurrencyManager.Instance.productionMultiplier *= dpsBonus;

                BigDouble clickBonus = 1 + (prestigePoints * 0.01);
                CurrencyManager.Instance.clickMultiplier *= clickBonus;
            }

            // 영구 업그레이드 보너스 적용
            ApplyPermanentUpgradeBonuses();
        }

        public bool TryPurchasePermanentUpgrade(string upgradeId, BigDouble cost)
        {
            if (prestigePoints >= cost)
            {
                prestigePoints -= cost;

                if (!permanentUpgrades.ContainsKey(upgradeId))
                    permanentUpgrades[upgradeId] = 0;

                permanentUpgrades[upgradeId]++;
                ApplyPermanentBonuses();

                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.SaveGame();
                }

                return true;
            }

            return false;
        }

        void ApplyPermanentUpgradeBonuses()
        {
            if (CurrencyManager.Instance == null)
                return;

            // 예시: 영구 업그레이드 효과들
            if (permanentUpgrades.ContainsKey("ClickDamageBonus"))
            {
                int level = permanentUpgrades["ClickDamageBonus"];
                CurrencyManager.Instance.clickMultiplier *= BigDouble.Pow(1.1, level);
            }

            if (permanentUpgrades.ContainsKey("ProductionBonus"))
            {
                int level = permanentUpgrades["ProductionBonus"];
                CurrencyManager.Instance.productionMultiplier *= BigDouble.Pow(1.15, level);
            }

            if (permanentUpgrades.ContainsKey("CriticalBonus"))
            {
                int level = permanentUpgrades["CriticalBonus"];
                CurrencyManager.Instance.criticalChance += level * 0.01f;
                CurrencyManager.Instance.criticalChance = Mathf.Min(CurrencyManager.Instance.criticalChance, 0.5f);
            }
        }

        public int GetPermanentUpgradeLevel(string upgradeId)
        {
            return permanentUpgrades.ContainsKey(upgradeId) ? permanentUpgrades[upgradeId] : 0;
        }
    }
}
