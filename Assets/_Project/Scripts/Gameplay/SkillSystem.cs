using UnityEngine;
using BreakInfinity;
using System.Collections.Generic;
using System;

namespace PunchKing
{
    [Serializable]
    public class Skill
    {
        public string skillName;
        [TextArea(2, 3)]
        public string description;
        public Sprite icon;

        public SkillType skillType;
        public float duration = 30f;      // 지속 시간
        public float cooldown = 60f;      // 쿨다운

        public BigDouble multiplier = 2;  // 배율

        [HideInInspector]
        public float cooldownTimer = 0f;
        [HideInInspector]
        public float activeTimer = 0f;
        [HideInInspector]
        public bool isActive = false;
    }

    public enum SkillType
    {
        ClickBoost,      // 클릭 데미지 부스트
        ProductionBoost, // 생산량 부스트
        GoldBoost,       // 골드 배율 부스트
        CriticalStrike,  // 100% 크리티컬
        InstantDamage    // 즉시 데미지
    }

    public class SkillSystem : MonoBehaviour
    {
        public static SkillSystem Instance { get; private set; }

        [Header("스킬 목록")]
        public List<Skill> skills = new List<Skill>();

        // 이벤트
        public event Action<Skill> OnSkillActivated;
        public event Action<Skill> OnSkillDeactivated;

        // 임시 저장용 (크리티컬 확률)
        private float originalCritChance = 0f;

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
            foreach (var skill in skills)
            {
                // 쿨다운 감소
                if (skill.cooldownTimer > 0)
                {
                    skill.cooldownTimer -= Time.deltaTime;
                }

                // 활성 시간 감소
                if (skill.isActive)
                {
                    skill.activeTimer -= Time.deltaTime;

                    if (skill.activeTimer <= 0)
                    {
                        DeactivateSkill(skill);
                    }
                }
            }
        }

        public bool TryActivateSkill(Skill skill)
        {
            if (skill.cooldownTimer > 0 || skill.isActive)
                return false;

            skill.isActive = true;
            skill.activeTimer = skill.duration;
            skill.cooldownTimer = skill.cooldown;

            ApplySkillEffect(skill, true);

            OnSkillActivated?.Invoke(skill);

            Debug.Log($"스킬 활성화: {skill.skillName}");

            return true;
        }

        public bool TryActivateSkillByIndex(int index)
        {
            if (index >= 0 && index < skills.Count)
            {
                return TryActivateSkill(skills[index]);
            }
            return false;
        }

        void ApplySkillEffect(Skill skill, bool activate)
        {
            if (CurrencyManager.Instance == null)
                return;

            switch (skill.skillType)
            {
                case SkillType.ClickBoost:
                    if (activate)
                        CurrencyManager.Instance.clickMultiplier *= skill.multiplier;
                    else
                        CurrencyManager.Instance.clickMultiplier /= skill.multiplier;
                    break;

                case SkillType.ProductionBoost:
                    if (activate)
                        CurrencyManager.Instance.productionMultiplier *= skill.multiplier;
                    else
                        CurrencyManager.Instance.productionMultiplier /= skill.multiplier;
                    break;

                case SkillType.GoldBoost:
                    if (activate)
                        CurrencyManager.Instance.goldMultiplier *= skill.multiplier;
                    else
                        CurrencyManager.Instance.goldMultiplier /= skill.multiplier;
                    break;

                case SkillType.CriticalStrike:
                    // 지속시간 동안 100% 크리티컬
                    if (activate)
                    {
                        originalCritChance = CurrencyManager.Instance.criticalChance;
                        CurrencyManager.Instance.criticalChance = 1.0f;
                    }
                    else
                    {
                        CurrencyManager.Instance.criticalChance = originalCritChance;
                    }
                    break;

                case SkillType.InstantDamage:
                    // 현재 DPS의 1000배 즉시 데미지
                    if (activate)
                    {
                        BigDouble damage = CurrencyManager.Instance.goldPerSecond * 1000;
                        CurrencyManager.Instance.gold += damage;
                        Debug.Log($"즉시 데미지: {damage}");
                    }
                    break;
            }
        }

        void DeactivateSkill(Skill skill)
        {
            skill.isActive = false;
            ApplySkillEffect(skill, false);

            OnSkillDeactivated?.Invoke(skill);

            Debug.Log($"스킬 비활성화: {skill.skillName}");
        }

        public bool IsSkillReady(Skill skill)
        {
            return skill.cooldownTimer <= 0 && !skill.isActive;
        }

        public bool IsSkillActive(Skill skill)
        {
            return skill.isActive;
        }

        public float GetSkillCooldownPercent(Skill skill)
        {
            if (skill.cooldownTimer <= 0)
                return 0f;

            return skill.cooldownTimer / skill.cooldown;
        }

        public float GetSkillActivePercent(Skill skill)
        {
            if (!skill.isActive)
                return 0f;

            return skill.activeTimer / skill.duration;
        }
    }
}
