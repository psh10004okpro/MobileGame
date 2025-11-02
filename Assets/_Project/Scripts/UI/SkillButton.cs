using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PunchKing
{
    public class SkillButton : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI skillNameText;
        public TextMeshProUGUI cooldownText;
        public Image iconImage;
        public Image cooldownFillImage;
        public Button button;

        [Header("Skill Data")]
        public int skillIndex = 0;

        private Skill currentSkill;

        void Start()
        {
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClicked);
            }

            // 스킬 참조 가져오기
            if (SkillSystem.Instance != null && skillIndex < SkillSystem.Instance.skills.Count)
            {
                currentSkill = SkillSystem.Instance.skills[skillIndex];
                InitializeUI();
            }
        }

        void Update()
        {
            if (currentSkill != null)
            {
                UpdateUI();
            }
        }

        void InitializeUI()
        {
            if (currentSkill == null)
                return;

            // 스킬 이름
            if (skillNameText != null)
            {
                skillNameText.SetText(currentSkill.skillName);
            }

            // 아이콘
            if (iconImage != null && currentSkill.icon != null)
            {
                iconImage.sprite = currentSkill.icon;
            }
        }

        void UpdateUI()
        {
            if (SkillSystem.Instance == null || currentSkill == null)
                return;

            bool isReady = SkillSystem.Instance.IsSkillReady(currentSkill);
            bool isActive = SkillSystem.Instance.IsSkillActive(currentSkill);

            // 버튼 활성화 상태
            if (button != null)
            {
                button.interactable = isReady;
            }

            // 쿨다운 텍스트
            if (cooldownText != null)
            {
                if (isActive)
                {
                    // 활성 시간 표시
                    cooldownText.SetText(string.Format("Active: {0:F1}s", currentSkill.activeTimer));
                    cooldownText.color = Color.green;
                }
                else if (currentSkill.cooldownTimer > 0)
                {
                    // 쿨다운 표시
                    cooldownText.SetText(string.Format("Cooldown: {0:F1}s", currentSkill.cooldownTimer));
                    cooldownText.color = Color.red;
                }
                else
                {
                    // 준비 완료
                    cooldownText.SetText("Ready!");
                    cooldownText.color = Color.white;
                }
            }

            // 쿨다운 Fill 이미지
            if (cooldownFillImage != null)
            {
                if (isActive)
                {
                    // 활성 시간 진행도
                    float progress = SkillSystem.Instance.GetSkillActivePercent(currentSkill);
                    cooldownFillImage.fillAmount = progress;
                    cooldownFillImage.color = Color.green;
                }
                else if (currentSkill.cooldownTimer > 0)
                {
                    // 쿨다운 진행도
                    float progress = SkillSystem.Instance.GetSkillCooldownPercent(currentSkill);
                    cooldownFillImage.fillAmount = progress;
                    cooldownFillImage.color = Color.red;
                }
                else
                {
                    cooldownFillImage.fillAmount = 0f;
                }
            }
        }

        void OnButtonClicked()
        {
            if (SkillSystem.Instance != null && currentSkill != null)
            {
                if (SkillSystem.Instance.TryActivateSkill(currentSkill))
                {
                    Debug.Log(string.Format("스킬 활성화: {0}", currentSkill.skillName));
                }
            }
        }
    }
}
