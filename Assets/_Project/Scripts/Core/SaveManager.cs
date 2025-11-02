using UnityEngine;
using BreakInfinity;
using System.IO;
using System.Collections.Generic;

namespace PunchKing
{
    [System.Serializable]
    public class GameSaveData
    {
        public int version = 1;
        public long saveTimestamp;

        // 화폐
        public string gold;
        public string lifetimeGold;
        public string goldPerClick;
        public string goldPerSecond;

        // 곱셈 보너스
        public string clickMultiplier;
        public string productionMultiplier;
        public string goldMultiplier;

        // 크리티컬
        public float criticalChance;
        public string criticalDamage;

        // 프레스티지
        public string prestigePoints;
        public int prestigeLevel;

        // 캐릭터
        public int characterLevel;
        public int evolutionStage;
        public string experiencePoints;

        // 업그레이드 레벨
        public List<string> upgradeNames = new List<string>();
        public List<int> upgradeLevels = new List<int>();

        // 영구 업그레이드
        public List<string> permanentUpgradeNames = new List<string>();
        public List<int> permanentUpgradeLevels = new List<int>();
    }

    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private string SavePath => Application.persistentDataPath + "/punchking_save.json";
        private float autoSaveInterval = 300f;  // 5분
        private float autoSaveTimer = 0f;

        private GameSaveData loadedData;

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
        }

        void Update()
        {
            autoSaveTimer += Time.deltaTime;
            if (autoSaveTimer >= autoSaveInterval)
            {
                SaveGame();
                autoSaveTimer = 0f;
            }
        }

        public void SaveGame()
        {
            if (CurrencyManager.Instance == null)
            {
                Debug.LogWarning("CurrencyManager가 초기화되지 않았습니다.");
                return;
            }

            GameSaveData data = new GameSaveData
            {
                version = 1,
                saveTimestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(),

                // 화폐
                gold = CurrencyManager.Instance.gold.ToString(),
                goldPerClick = CurrencyManager.Instance.goldPerClick.ToString(),
                goldPerSecond = CurrencyManager.Instance.goldPerSecond.ToString(),

                // 곱셈 보너스
                clickMultiplier = CurrencyManager.Instance.clickMultiplier.ToString(),
                productionMultiplier = CurrencyManager.Instance.productionMultiplier.ToString(),
                goldMultiplier = CurrencyManager.Instance.goldMultiplier.ToString(),

                // 크리티컬
                criticalChance = CurrencyManager.Instance.criticalChance,
                criticalDamage = CurrencyManager.Instance.criticalDamage.ToString(),
            };

            // 프레스티지
            if (PrestigeManager.Instance != null)
            {
                data.lifetimeGold = PrestigeManager.Instance.lifetimeGold.ToString();
                data.prestigePoints = PrestigeManager.Instance.prestigePoints.ToString();
                data.prestigeLevel = PrestigeManager.Instance.prestigeLevel;

                // 영구 업그레이드
                foreach (var kvp in PrestigeManager.Instance.permanentUpgrades)
                {
                    data.permanentUpgradeNames.Add(kvp.Key);
                    data.permanentUpgradeLevels.Add(kvp.Value);
                }
            }

            // 캐릭터
            if (CharacterEvolution.Instance != null)
            {
                data.characterLevel = CharacterEvolution.Instance.characterLevel;
                data.evolutionStage = CharacterEvolution.Instance.currentStage;
                data.experiencePoints = CharacterEvolution.Instance.experiencePoints.ToString();
            }

            // 업그레이드
            if (UpgradeManager.Instance != null)
            {
                var upgradeLevelsDict = UpgradeManager.Instance.GetUpgradeLevelsDictionary();
                foreach (var kvp in upgradeLevelsDict)
                {
                    data.upgradeNames.Add(kvp.Key);
                    data.upgradeLevels.Add(kvp.Value);
                }
            }

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);

            Debug.Log($"게임 저장 완료: {SavePath}");
        }

        public void LoadGame()
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("저장 파일이 없습니다. 새 게임을 시작합니다.");
                return;
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                loadedData = JsonUtility.FromJson<GameSaveData>(json);

                // 데이터 적용
                ApplyLoadedData(loadedData);

                // 오프라인 진행 계산
                CalculateOfflineProgress();

                Debug.Log("게임 로드 완료");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"게임 로드 실패: {e.Message}");
            }
        }

        void ApplyLoadedData(GameSaveData data)
        {
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.gold = BigDouble.Parse(data.gold);
                CurrencyManager.Instance.goldPerClick = BigDouble.Parse(data.goldPerClick);
                CurrencyManager.Instance.goldPerSecond = BigDouble.Parse(data.goldPerSecond);
                CurrencyManager.Instance.clickMultiplier = BigDouble.Parse(data.clickMultiplier);
                CurrencyManager.Instance.productionMultiplier = BigDouble.Parse(data.productionMultiplier);
                CurrencyManager.Instance.goldMultiplier = BigDouble.Parse(data.goldMultiplier);
                CurrencyManager.Instance.criticalChance = data.criticalChance;
                CurrencyManager.Instance.criticalDamage = BigDouble.Parse(data.criticalDamage);
            }

            // 프레스티지
            if (PrestigeManager.Instance != null)
            {
                PrestigeManager.Instance.lifetimeGold = BigDouble.Parse(data.lifetimeGold);
                PrestigeManager.Instance.prestigePoints = BigDouble.Parse(data.prestigePoints);
                PrestigeManager.Instance.prestigeLevel = data.prestigeLevel;

                // 영구 업그레이드
                PrestigeManager.Instance.permanentUpgrades.Clear();
                for (int i = 0; i < data.permanentUpgradeNames.Count; i++)
                {
                    PrestigeManager.Instance.permanentUpgrades[data.permanentUpgradeNames[i]] = data.permanentUpgradeLevels[i];
                }
            }

            // 캐릭터
            if (CharacterEvolution.Instance != null)
            {
                CharacterEvolution.Instance.characterLevel = data.characterLevel;
                CharacterEvolution.Instance.currentStage = data.evolutionStage;
                CharacterEvolution.Instance.experiencePoints = BigDouble.Parse(data.experiencePoints);
            }

            // 업그레이드
            if (UpgradeManager.Instance != null)
            {
                Dictionary<string, int> upgradeLevels = new Dictionary<string, int>();
                for (int i = 0; i < data.upgradeNames.Count; i++)
                {
                    upgradeLevels[data.upgradeNames[i]] = data.upgradeLevels[i];
                }
                UpgradeManager.Instance.LoadUpgradeLevels(upgradeLevels);
            }
        }

        void CalculateOfflineProgress()
        {
            if (loadedData == null || CurrencyManager.Instance == null)
                return;

            long currentTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long lastSaveTime = loadedData.saveTimestamp;
            long offlineSeconds = currentTime - lastSaveTime;

            // 최대 4시간
            offlineSeconds = System.Math.Min(offlineSeconds, 14400);

            if (offlineSeconds > 60)  // 1분 이상
            {
                BigDouble offlineGold = CurrencyManager.Instance.goldPerSecond *
                                        offlineSeconds * 0.9;  // 90% 효율

                CurrencyManager.Instance.gold += offlineGold;

                Debug.Log($"오프라인 보상: {offlineGold} ({offlineSeconds}초)");

                // UI 팝업 표시 (추후 구현)
                // ShowOfflineRewardPopup(offlineGold, offlineSeconds);
            }
        }

        void OnApplicationQuit()
        {
            SaveGame();
        }

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveGame();
            }
        }

        public void DeleteSave()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("저장 데이터 삭제됨");
            }
        }

        public bool HasSaveData()
        {
            return File.Exists(SavePath);
        }
    }
}
