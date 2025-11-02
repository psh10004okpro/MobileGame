using UnityEngine;
using BreakInfinity;
using System.Collections.Generic;
using System;

namespace PunchKing
{
    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance { get; private set; }

        [Header("업그레이드 데이터")]
        public List<UpgradeData> upgradeDataList = new List<UpgradeData>();

        private Dictionary<UpgradeData, int> upgradeLevels = new Dictionary<UpgradeData, int>();

        // 이벤트
        public event Action<UpgradeData, int> OnUpgradePurchased;

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
                return;
            }

            // 초기화
            foreach (var data in upgradeDataList)
            {
                if (data != null)
                {
                    upgradeLevels[data] = 0;
                }
            }
        }

        public bool TryPurchaseUpgrade(UpgradeData upgrade)
        {
            if (upgrade == null)
                return false;

            int currentLevel = GetUpgradeLevel(upgrade);

            if (currentLevel >= upgrade.maxLevel)
                return false;

            BigDouble cost = upgrade.GetCostAtLevel(currentLevel);

            if (CurrencyManager.Instance.TrySpend(cost))
            {
                upgradeLevels[upgrade]++;
                ApplyUpgrade(upgrade);
                OnUpgradePurchased?.Invoke(upgrade, upgradeLevels[upgrade]);

                // 저장
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.SaveGame();
                }

                return true;
            }

            return false;
        }

        void ApplyUpgrade(UpgradeData upgrade)
        {
            switch (upgrade.upgradeType)
            {
                case UpgradeType.ClickDamage:
                    CurrencyManager.Instance.goldPerClick += upgrade.effectValue;
                    break;

                case UpgradeType.ProductionDPS:
                    CurrencyManager.Instance.goldPerSecond += upgrade.effectValue;
                    break;

                case UpgradeType.CriticalChance:
                    CurrencyManager.Instance.criticalChance += upgrade.percentBonus;
                    // 상한선 35%
                    CurrencyManager.Instance.criticalChance = Mathf.Min(
                        CurrencyManager.Instance.criticalChance, 0.35f
                    );
                    break;

                case UpgradeType.CriticalDamage:
                    CurrencyManager.Instance.criticalDamage *= (1 + upgrade.percentBonus);
                    break;

                case UpgradeType.GoldMultiplier:
                    CurrencyManager.Instance.goldMultiplier *= (1 + upgrade.percentBonus);
                    break;
            }
        }

        public int GetUpgradeLevel(UpgradeData upgrade)
        {
            if (upgrade == null)
                return 0;

            return upgradeLevels.ContainsKey(upgrade) ? upgradeLevels[upgrade] : 0;
        }

        public void SetUpgradeLevel(UpgradeData upgrade, int level)
        {
            if (upgrade != null)
            {
                upgradeLevels[upgrade] = level;
            }
        }

        public BigDouble GetUpgradeCost(UpgradeData upgrade)
        {
            if (upgrade == null)
                return 0;

            int currentLevel = GetUpgradeLevel(upgrade);
            return upgrade.GetCostAtLevel(currentLevel);
        }

        public bool CanAffordUpgrade(UpgradeData upgrade)
        {
            if (upgrade == null)
                return false;

            BigDouble cost = GetUpgradeCost(upgrade);
            return CurrencyManager.Instance.CanAfford(cost);
        }

        // 저장/로드를 위한 메서드
        public Dictionary<string, int> GetUpgradeLevelsDictionary()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach (var kvp in upgradeLevels)
            {
                if (kvp.Key != null)
                {
                    result[kvp.Key.name] = kvp.Value;
                }
            }
            return result;
        }

        public void LoadUpgradeLevels(Dictionary<string, int> levels)
        {
            foreach (var upgrade in upgradeDataList)
            {
                if (upgrade != null && levels.ContainsKey(upgrade.name))
                {
                    upgradeLevels[upgrade] = levels[upgrade.name];
                }
            }

            // 모든 업그레이드 효과 재적용
            ReapplyAllUpgrades();
        }

        void ReapplyAllUpgrades()
        {
            foreach (var kvp in upgradeLevels)
            {
                UpgradeData upgrade = kvp.Key;
                int level = kvp.Value;

                for (int i = 0; i < level; i++)
                {
                    ApplyUpgrade(upgrade);
                }
            }
        }
    }
}
