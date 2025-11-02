using UnityEngine;
using TMPro;
using BreakInfinity;
using System.Collections;

namespace PunchKing
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("메인 UI 텍스트")]
        public TextMeshProUGUI goldText;
        public TextMeshProUGUI dpsText;
        public TextMeshProUGUI clickDamageText;
        public TextMeshProUGUI levelText;

        [Header("프레스티지 UI")]
        public TextMeshProUGUI prestigePointsText;
        public TextMeshProUGUI prestigeLevelText;
        public TextMeshProUGUI prestigeGainText;

        [Header("패널")]
        public GameObject upgradePanel;
        public GameObject prestigePanel;
        public GameObject skillPanel;
        public GameObject settingsPanel;

        [Header("데미지 넘버")]
        public GameObject damageNumberPrefab;
        public Transform damageNumberParent;
        public Canvas mainCanvas;

        [Header("업데이트 설정")]
        public float updateInterval = 0.1f;

        private BigDouble cachedGold;
        private float updateTimer = 0f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            // 이벤트 구독
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnGoldEarned += OnGoldEarned;
            }

            // 초기 UI 업데이트
            UpdateUI();
        }

        void Update()
        {
            updateTimer += Time.deltaTime;

            if (updateTimer >= updateInterval)
            {
                UpdateUI();
                updateTimer = 0f;
            }
        }

        void UpdateUI()
        {
            if (CurrencyManager.Instance == null)
                return;

            // 골드 표시 (GC 최적화)
            BigDouble currentGold = CurrencyManager.Instance.gold;
            if (currentGold != cachedGold || goldText.text == "")
            {
                if (goldText != null)
                    goldText.SetText("Gold: {0}", NumberFormatter.FormatNumber(currentGold));
                cachedGold = currentGold;
            }

            // DPS 표시
            if (dpsText != null)
            {
                BigDouble dps = CurrencyManager.Instance.GetTotalDPS();
                dpsText.SetText("DPS: {0}/s", NumberFormatter.FormatNumber(dps));
            }

            // 클릭 데미지 표시
            if (clickDamageText != null)
            {
                BigDouble clickDmg = CurrencyManager.Instance.GetTotalClickDamage();
                clickDamageText.SetText("Click: {0}", NumberFormatter.FormatNumber(clickDmg));
            }

            // 레벨 표시
            if (levelText != null && CharacterEvolution.Instance != null)
            {
                levelText.SetText("Lv.{0}", CharacterEvolution.Instance.characterLevel);
            }

            // 프레스티지 UI
            UpdatePrestigeUI();
        }

        void UpdatePrestigeUI()
        {
            if (PrestigeManager.Instance == null)
                return;

            if (prestigePointsText != null)
            {
                prestigePointsText.SetText("PP: {0}", NumberFormatter.FormatNumber(PrestigeManager.Instance.prestigePoints));
            }

            if (prestigeLevelText != null)
            {
                prestigeLevelText.SetText("Prestige Lv.{0}", PrestigeManager.Instance.prestigeLevel);
            }

            if (prestigeGainText != null)
            {
                BigDouble gain = PrestigeManager.Instance.GetPrestigeGain();
                if (gain > 0)
                {
                    prestigeGainText.SetText("Next: +{0} PP", NumberFormatter.FormatNumber(gain));
                    prestigeGainText.color = Color.green;
                }
                else
                {
                    prestigeGainText.SetText("Not enough progress");
                    prestigeGainText.color = Color.gray;
                }
            }
        }

        void OnGoldEarned(BigDouble amount, bool isCrit)
        {
            ShowDamageNumber(amount, isCrit);
        }

        public void ShowDamageNumber(BigDouble damage, bool isCrit)
        {
            if (damageNumberPrefab == null || damageNumberParent == null)
                return;

            GameObject dmgObj = Instantiate(damageNumberPrefab, damageNumberParent);
            TextMeshProUGUI dmgText = dmgObj.GetComponent<TextMeshProUGUI>();

            if (dmgText != null)
            {
                // 텍스트 설정
                dmgText.SetText("{0}", NumberFormatter.FormatNumber(damage));
                dmgText.fontSize = isCrit ? 48 : 32;
                dmgText.color = isCrit ? Color.yellow : Color.white;

                // 랜덤 위치 (화면 중앙 근처)
                RectTransform rectTransform = dmgObj.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    float randomX = Random.Range(-100f, 100f);
                    rectTransform.anchoredPosition = new Vector2(randomX, 0);
                }

                // 애니메이션 코루틴
                StartCoroutine(AnimateDamageNumber(dmgObj, dmgText));
            }
        }

        IEnumerator AnimateDamageNumber(GameObject dmgObj, TextMeshProUGUI dmgText)
        {
            RectTransform rectTransform = dmgObj.GetComponent<RectTransform>();
            Vector2 startPos = rectTransform.anchoredPosition;
            Vector2 endPos = startPos + Vector2.up * 150f;

            float duration = 1f;
            float elapsed = 0f;

            // Scale up
            dmgObj.transform.localScale = Vector3.zero;
            while (elapsed < 0.2f)
            {
                dmgObj.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, elapsed / 0.2f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            dmgObj.transform.localScale = Vector3.one;

            // Move up and fade
            elapsed = 0f;
            Color startColor = dmgText.color;
            Color endColor = startColor;
            endColor.a = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

                if (elapsed > 0.2f)
                {
                    dmgText.color = Color.Lerp(startColor, endColor, (elapsed - 0.2f) / (duration - 0.2f));
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(dmgObj);
        }

        // 패널 토글 메서드들
        public void ToggleUpgradePanel()
        {
            if (upgradePanel != null)
                upgradePanel.SetActive(!upgradePanel.activeSelf);
        }

        public void TogglePrestigePanel()
        {
            if (prestigePanel != null)
                prestigePanel.SetActive(!prestigePanel.activeSelf);
        }

        public void ToggleSkillPanel()
        {
            if (skillPanel != null)
                skillPanel.SetActive(!skillPanel.activeSelf);
        }

        public void ToggleSettingsPanel()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(!settingsPanel.activeSelf);
        }

        // 프레스티지 버튼
        public void OnPrestigeButtonClicked()
        {
            if (PrestigeManager.Instance != null && PrestigeManager.Instance.CanPrestige())
            {
                PrestigeManager.Instance.DoPrestige();
                UpdateUI();
            }
            else
            {
                Debug.Log("프레스티지할 수 없습니다!");
            }
        }

        void OnDestroy()
        {
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnGoldEarned -= OnGoldEarned;
            }
        }
    }
}
