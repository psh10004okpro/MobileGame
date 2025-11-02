using UnityEngine;
using BreakInfinity;
using System;

namespace PunchKing
{
    [Serializable]
    public class EvolutionStage
    {
        public string stageName;
        public int requiredLevel;
        public BigDouble requiredGold;

        public float statBonus;  // 모든 스탯 보너스 (0.5 = +50%)
        public Sprite characterSprite;
    }

    public class CharacterEvolution : MonoBehaviour
    {
        public static CharacterEvolution Instance { get; private set; }

        [Header("진화 단계")]
        public EvolutionStage[] evolutionStages;

        [Header("현재 상태")]
        public int currentStage = 0;
        public int characterLevel = 1;
        public BigDouble experiencePoints = 0;

        // 이벤트
        public event Action<int> OnLevelUp;
        public event Action<int> OnEvolution;

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
            // 기본 진화 단계 설정 (필요시)
            if (evolutionStages == null || evolutionStages.Length == 0)
            {
                InitializeDefaultEvolutionStages();
            }
        }

        void InitializeDefaultEvolutionStages()
        {
            evolutionStages = new EvolutionStage[]
            {
                new EvolutionStage
                {
                    stageName = "초보 복서",
                    requiredLevel = 1,
                    requiredGold = 0,
                    statBonus = 0f
                },
                new EvolutionStage
                {
                    stageName = "아마추어 복서",
                    requiredLevel = 10,
                    requiredGold = 100000,
                    statBonus = 0.5f
                },
                new EvolutionStage
                {
                    stageName = "프로 복서",
                    requiredLevel = 25,
                    requiredGold = BigDouble.Parse("1e9"),
                    statBonus = 1.0f
                },
                new EvolutionStage
                {
                    stageName = "챔피언",
                    requiredLevel = 50,
                    requiredGold = BigDouble.Parse("1e15"),
                    statBonus = 2.0f
                },
                new EvolutionStage
                {
                    stageName = "전설의 복서",
                    requiredLevel = 100,
                    requiredGold = BigDouble.Parse("1e25"),
                    statBonus = 5.0f
                }
            };
        }

        public bool CanEvolve()
        {
            if (currentStage >= evolutionStages.Length - 1)
                return false;

            EvolutionStage nextStage = evolutionStages[currentStage + 1];

            return characterLevel >= nextStage.requiredLevel &&
                   CurrencyManager.Instance.gold >= nextStage.requiredGold;
        }

        public void Evolve()
        {
            if (!CanEvolve())
            {
                Debug.Log("진화 조건을 만족하지 못했습니다!");
                return;
            }

            EvolutionStage nextStage = evolutionStages[currentStage + 1];

            // 골드 소비
            CurrencyManager.Instance.TrySpend(nextStage.requiredGold);

            // 진화
            currentStage++;

            // 스탯 보너스 적용
            ApplyEvolutionBonus(nextStage);

            // 비주얼 변경
            UpdateCharacterVisual();

            // 이벤트 발동
            OnEvolution?.Invoke(currentStage);

            Debug.Log($"진화 완료! {nextStage.stageName}");

            // 저장
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
            }
        }

        void ApplyEvolutionBonus(EvolutionStage stage)
        {
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.goldPerClick *= (1 + stage.statBonus);
                CurrencyManager.Instance.goldPerSecond *= (1 + stage.statBonus);
            }
        }

        void UpdateCharacterVisual()
        {
            // UI 업데이트 로직
            // 실제 구현은 UIManager에서 처리
            if (currentStage < evolutionStages.Length)
            {
                Debug.Log($"캐릭터 비주얼 변경: {evolutionStages[currentStage].stageName}");
            }
        }

        public void GainExperience(BigDouble exp)
        {
            experiencePoints += exp;

            // 레벨 업 체크
            BigDouble requiredExp = CalculateRequiredExp();
            while (experiencePoints >= requiredExp)
            {
                LevelUp();
                experiencePoints -= requiredExp;
                requiredExp = CalculateRequiredExp();
            }
        }

        BigDouble CalculateRequiredExp()
        {
            // 레벨 * 100 * 1.1^레벨
            return characterLevel * 100 * BigDouble.Pow(1.1, characterLevel);
        }

        void LevelUp()
        {
            characterLevel++;

            // 레벨업 보너스 (1% 모든 스탯)
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.goldPerClick *= 1.01;
                CurrencyManager.Instance.goldPerSecond *= 1.01;
            }

            // 이벤트 발동
            OnLevelUp?.Invoke(characterLevel);

            Debug.Log($"레벨 업! Lv.{characterLevel}");

            // 자동 진화 체크
            if (CanEvolve())
            {
                Debug.Log("진화 가능!");
            }
        }

        public EvolutionStage GetCurrentStage()
        {
            if (currentStage >= 0 && currentStage < evolutionStages.Length)
            {
                return evolutionStages[currentStage];
            }
            return null;
        }

        public EvolutionStage GetNextStage()
        {
            if (currentStage + 1 < evolutionStages.Length)
            {
                return evolutionStages[currentStage + 1];
            }
            return null;
        }

        public float GetExpProgress()
        {
            BigDouble required = CalculateRequiredExp();
            if (required <= 0)
                return 0f;

            BigDouble progressRatio = experiencePoints / required;
            return (float)progressRatio.ToDouble();
        }
    }
}
