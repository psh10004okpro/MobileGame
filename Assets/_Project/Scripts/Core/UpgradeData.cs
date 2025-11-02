using UnityEngine;
using BreakInfinity;

namespace PunchKing
{
    [CreateAssetMenu(fileName = "New Upgrade", menuName = "PunchKing/Upgrade Data")]
    public class UpgradeData : ScriptableObject
    {
        [Header("정보")]
        public string upgradeName;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;

        [Header("비용")]
        public BigDouble baseCost = 100;
        public float costMultiplier = 1.15f;  // 펀치킹 표준

        [Header("효과")]
        public UpgradeType upgradeType;
        public BigDouble effectValue;
        public float percentBonus;  // 퍼센트 보너스 (0.5 = 50%)

        [Header("제한")]
        public int maxLevel = 999;
        public bool isUnlocked = true;

        public BigDouble GetCostAtLevel(int level)
        {
            return baseCost * BigDouble.Pow(costMultiplier, level);
        }

        public string GetUpgradeDescription(int currentLevel)
        {
            return $"{description}\n\nLevel: {currentLevel}/{maxLevel}";
        }
    }

    public enum UpgradeType
    {
        ClickDamage,        // 클릭 데미지
        ProductionDPS,      // 초당 생산량
        CriticalChance,     // 크리티컬 확률
        CriticalDamage,     // 크리티컬 데미지
        GoldMultiplier      // 골드 배율
    }
}
