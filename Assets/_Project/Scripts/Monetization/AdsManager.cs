using UnityEngine;
using System;

namespace PunchKing.Monetization
{
    /// <summary>
    /// 광고 시스템 관리자
    /// Unity Ads, AdMob 등을 통합하여 사용
    /// </summary>
    public class AdsManager : MonoBehaviour
    {
        public static AdsManager Instance { get; private set; }

        [Header("광고 설정")]
        public bool enableAds = true;
        public bool testMode = true;

        [Header("광고 타입별 활성화")]
        public bool enableRewardedAds = true;
        public bool enableInterstitialAds = true;
        public bool enableBannerAds = false;

        [Header("광고 표시 간격 (초)")]
        public float interstitialInterval = 300f; // 5분

        // 광고 준비 상태
        private bool isRewardedAdReady = false;
        private bool isInterstitialAdReady = false;

        // 타이머
        private float interstitialTimer = 0f;

        // 이벤트
        public event Action OnRewardedAdCompleted;
        public event Action OnRewardedAdFailed;
        public event Action OnInterstitialAdCompleted;

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
            InitializeAds();
        }

        void Update()
        {
            // 전면 광고 타이머
            if (enableInterstitialAds)
            {
                interstitialTimer += Time.deltaTime;
            }
        }

        void InitializeAds()
        {
            if (!enableAds)
            {
                Debug.Log("광고 시스템 비활성화됨");
                return;
            }

            // VIP 유저는 광고 제거
            if (VIPManager.Instance != null && VIPManager.Instance.IsVIP() && VIPManager.Instance.HasAdRemoval())
            {
                enableAds = false;
                Debug.Log("VIP 유저 - 광고 제거됨");
                return;
            }

            // TODO: Unity Ads / AdMob 초기화
            // Advertisement.Initialize(gameId, testMode);
            // MobileAds.Initialize(initStatus => { });

            Debug.Log("광고 시스템 초기화 완료");

            // 테스트 모드에서는 광고 준비 완료로 설정
            if (testMode)
            {
                isRewardedAdReady = true;
                isInterstitialAdReady = true;
            }
        }

        #region Rewarded Ads (보상형 광고)

        /// <summary>
        /// 보상형 광고 준비 여부 확인
        /// </summary>
        public bool IsRewardedAdReady()
        {
            if (!enableAds || !enableRewardedAds)
                return false;

            // TODO: Unity Ads 체크
            // return Advertisement.IsReady("Rewarded_Android");

            return isRewardedAdReady; // 테스트용
        }

        /// <summary>
        /// 보상형 광고 표시
        /// </summary>
        public void ShowRewardedAd(Action<bool> callback)
        {
            if (!IsRewardedAdReady())
            {
                Debug.LogWarning("보상형 광고가 준비되지 않았습니다.");
                callback?.Invoke(false);
                return;
            }

            // TODO: Unity Ads 표시
            /*
            Advertisement.Show("Rewarded_Android", new ShowOptions
            {
                resultCallback = result =>
                {
                    if (result == ShowResult.Finished)
                    {
                        OnRewardedAdCompleted?.Invoke();
                        callback?.Invoke(true);
                    }
                    else
                    {
                        OnRewardedAdFailed?.Invoke();
                        callback?.Invoke(false);
                    }
                }
            });
            */

            // 테스트 모드: 즉시 성공
            if (testMode)
            {
                Debug.Log("테스트 모드: 보상형 광고 시청 완료");
                OnRewardedAdCompleted?.Invoke();
                callback?.Invoke(true);
            }
        }

        #endregion

        #region Interstitial Ads (전면 광고)

        /// <summary>
        /// 전면 광고 준비 여부 확인
        /// </summary>
        public bool IsInterstitialAdReady()
        {
            if (!enableAds || !enableInterstitialAds)
                return false;

            // TODO: Unity Ads 체크
            // return Advertisement.IsReady("Interstitial_Android");

            return isInterstitialAdReady; // 테스트용
        }

        /// <summary>
        /// 전면 광고 표시 (자동)
        /// </summary>
        public void ShowInterstitialAdIfReady()
        {
            if (interstitialTimer < interstitialInterval)
                return;

            if (!IsInterstitialAdReady())
                return;

            ShowInterstitialAd();
            interstitialTimer = 0f;
        }

        /// <summary>
        /// 전면 광고 표시 (수동)
        /// </summary>
        public void ShowInterstitialAd()
        {
            if (!IsInterstitialAdReady())
            {
                Debug.LogWarning("전면 광고가 준비되지 않았습니다.");
                return;
            }

            // TODO: Unity Ads 표시
            /*
            Advertisement.Show("Interstitial_Android", new ShowOptions
            {
                resultCallback = result =>
                {
                    OnInterstitialAdCompleted?.Invoke();
                }
            });
            */

            // 테스트 모드
            if (testMode)
            {
                Debug.Log("테스트 모드: 전면 광고 표시");
                OnInterstitialAdCompleted?.Invoke();
            }
        }

        #endregion

        #region Banner Ads (배너 광고)

        /// <summary>
        /// 배너 광고 표시
        /// </summary>
        public void ShowBanner()
        {
            if (!enableAds || !enableBannerAds)
                return;

            // TODO: Unity Ads 배너 표시
            // Advertisement.Banner.Show("Banner_Android");

            Debug.Log("배너 광고 표시");
        }

        /// <summary>
        /// 배너 광고 숨기기
        /// </summary>
        public void HideBanner()
        {
            // TODO: Unity Ads 배너 숨기기
            // Advertisement.Banner.Hide();

            Debug.Log("배너 광고 숨김");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 광고 제거 구매 시 호출
        /// </summary>
        public void RemoveAds()
        {
            enableAds = false;
            HideBanner();
            Debug.Log("광고가 영구적으로 제거되었습니다.");
        }

        /// <summary>
        /// 프레스티지 후 전면 광고 표시
        /// </summary>
        public void ShowAdAfterPrestige()
        {
            if (PrestigeManager.Instance.prestigeLevel % 3 == 0) // 3번째 프레스티지마다
            {
                ShowInterstitialAdIfReady();
            }
        }

        #endregion
    }
}
