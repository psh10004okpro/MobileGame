using UnityEngine;
using BreakInfinity;
using System;

namespace PunchKing
{
    public class CurrencyManager : MonoBehaviour
    {
        public static CurrencyManager Instance { get; private set; }

        [Header("Currency")]
        public BigDouble gold = 0;
        public BigDouble goldPerClick = 1;
        public BigDouble goldPerSecond = 0;

        [Header("Multipliers")]
        public BigDouble clickMultiplier = 1;
        public BigDouble productionMultiplier = 1;
        public BigDouble goldMultiplier = 1;

        [Header("Critical")]
        public float criticalChance = 0.05f;  // 5%
        public BigDouble criticalDamage = 1.5;

        // 이벤트
        public event Action<BigDouble, bool> OnGoldEarned;

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
            // 수동적 골드 획득
            if (goldPerSecond > 0)
            {
                BigDouble passiveGain = goldPerSecond * productionMultiplier * goldMultiplier * Time.deltaTime;
                gold += passiveGain;
            }
        }

        public bool OnClick()
        {
            // 크리티컬 체크
            bool isCrit = UnityEngine.Random.value < criticalChance;
            BigDouble clickGain = goldPerClick * clickMultiplier * goldMultiplier;

            if (isCrit)
            {
                clickGain *= criticalDamage;
            }

            gold += clickGain;

            // 이벤트 발동
            OnGoldEarned?.Invoke(clickGain, isCrit);

            return isCrit;
        }

        public bool CanAfford(BigDouble cost)
        {
            return gold >= cost;
        }

        public bool TrySpend(BigDouble cost)
        {
            if (CanAfford(cost))
            {
                gold -= cost;
                return true;
            }
            return false;
        }

        public void AddGold(BigDouble amount)
        {
            gold += amount;
        }

        public BigDouble GetTotalDPS()
        {
            return goldPerSecond * productionMultiplier * goldMultiplier;
        }

        public BigDouble GetTotalClickDamage()
        {
            return goldPerClick * clickMultiplier * goldMultiplier;
        }
    }
}
