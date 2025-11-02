using UnityEngine;

namespace PunchKing
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        public bool isGameInitialized = false;
        public float gameTime = 0f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            // 저장 데이터 로드
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.LoadGame();
            }

            // 성능 설정
            ApplyPerformanceSettings();

            isGameInitialized = true;
        }

        void Update()
        {
            if (isGameInitialized)
            {
                gameTime += Time.deltaTime;
            }
        }

        void InitializeGame()
        {
            Debug.Log("Punch King Game Initialized");
        }

        void ApplyPerformanceSettings()
        {
            // 프레임레이트 제한
            Application.targetFrameRate = 30;

            // VSync 비활성화
            QualitySettings.vSyncCount = 0;

            // 화면 꺼짐 방지 (모바일)
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        void OnApplicationQuit()
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
            }
        }

        void OnApplicationPause(bool pause)
        {
            if (pause && SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
            }
        }

        void OnApplicationFocus(bool focus)
        {
            if (!focus && SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
            }
        }
    }
}
