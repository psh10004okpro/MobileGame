using UnityEngine;
using BreakInfinity;
using System;
using System.Collections.Generic;

namespace PunchKing.Monetization
{
    /// <summary>
    /// 인앱 구매(IAP) 상품 데이터
    /// </summary>
    [Serializable]
    public class IAPProduct
    {
        public string productId;
        public string productName;
        public ProductType productType;
        public float price; // USD
        public BigDouble goldAmount;
        public BigDouble prestigePointAmount;
        public int vipDays;
    }

    public enum ProductType
    {
        Gold,           // 골드 팩
        PrestigePoints, // PP 팩
        RemoveAds,      // 광고 제거
        VIPPass,        // VIP 패스
        StarterPack     // 스타터 팩
    }

    /// <summary>
    /// 인앱 구매 시스템 관리자
    /// Unity IAP를 통합하여 사용
    /// </summary>
    public class IAPManager : MonoBehaviour
    {
        public static IAPManager Instance { get; private set; }

        [Header("IAP 설정")]
        public bool enableIAP = true;
        public bool testMode = true;

        [Header("상품 목록")]
        public List<IAPProduct> products = new List<IAPProduct>();

        // 구매 완료 이벤트
        public event Action<IAPProduct> OnPurchaseCompleted;
        public event Action<string> OnPurchaseFailed;

        // 구매 데이터 저장
        private bool hasRemovedAds = false;
        private bool hasPurchasedStarterPack = false;

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
            InitializeIAP();
            LoadPurchaseData();
        }

        void InitializeIAP()
        {
            if (!enableIAP)
            {
                Debug.Log("IAP 시스템 비활성화됨");
                return;
            }

            // 기본 상품 추가 (코드로 생성)
            if (products.Count == 0)
            {
                AddDefaultProducts();
            }

            // TODO: Unity IAP 초기화
            /*
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var product in products)
            {
                builder.AddProduct(product.productId, ProductType.Consumable);
            }

            UnityPurchasing.Initialize(this, builder);
            */

            Debug.Log("IAP 시스템 초기화 완료");
        }

        void AddDefaultProducts()
        {
            // 골드 팩
            products.Add(new IAPProduct
            {
                productId = "gold_pack_small",
                productName = "골드 소액 팩",
                productType = ProductType.Gold,
                price = 0.99f,
                goldAmount = 100000
            });

            products.Add(new IAPProduct
            {
                productId = "gold_pack_medium",
                productName = "골드 중간 팩",
                productType = ProductType.Gold,
                price = 4.99f,
                goldAmount = 1000000
            });

            products.Add(new IAPProduct
            {
                productId = "gold_pack_large",
                productName = "골드 대형 팩",
                productType = ProductType.Gold,
                price = 9.99f,
                goldAmount = 5000000
            });

            // PP 팩
            products.Add(new IAPProduct
            {
                productId = "pp_pack_starter",
                productName = "PP 스타터 팩",
                productType = ProductType.PrestigePoints,
                price = 2.99f,
                prestigePointAmount = 100
            });

            products.Add(new IAPProduct
            {
                productId = "pp_pack_mega",
                productName = "PP 메가 팩",
                productType = ProductType.PrestigePoints,
                price = 9.99f,
                prestigePointAmount = 500
            });

            // 광고 제거
            products.Add(new IAPProduct
            {
                productId = "remove_ads",
                productName = "광고 제거",
                productType = ProductType.RemoveAds,
                price = 4.99f
            });

            // VIP 패스
            products.Add(new IAPProduct
            {
                productId = "vip_pass_30days",
                productName = "VIP 패스 (30일)",
                productType = ProductType.VIPPass,
                price = 9.99f,
                vipDays = 30
            });

            // 스타터 팩
            products.Add(new IAPProduct
            {
                productId = "starter_pack",
                productName = "스타터 팩 (한정)",
                productType = ProductType.StarterPack,
                price = 0.99f,
                goldAmount = 500000,
                prestigePointAmount = 50
            });
        }

        #region Purchase Methods

        /// <summary>
        /// 상품 구매
        /// </summary>
        public void PurchaseProduct(string productId)
        {
            if (!enableIAP)
            {
                Debug.LogWarning("IAP가 비활성화되어 있습니다.");
                return;
            }

            IAPProduct product = GetProduct(productId);
            if (product == null)
            {
                Debug.LogError($"상품을 찾을 수 없습니다: {productId}");
                OnPurchaseFailed?.Invoke("상품을 찾을 수 없습니다.");
                return;
            }

            // 스타터 팩은 1회만 구매 가능
            if (product.productType == ProductType.StarterPack && hasPurchasedStarterPack)
            {
                Debug.LogWarning("스타터 팩은 이미 구매했습니다.");
                OnPurchaseFailed?.Invoke("이미 구매한 상품입니다.");
                return;
            }

            // TODO: Unity IAP 구매 시작
            // buyController.InitiatePurchase(productId);

            // 테스트 모드: 즉시 구매 완료
            if (testMode)
            {
                Debug.Log($"테스트 모드: {product.productName} 구매 완료");
                ProcessPurchase(product);
            }
        }

        /// <summary>
        /// 구매 처리 (Unity IAP 콜백)
        /// </summary>
        public void ProcessPurchase(IAPProduct product)
        {
            Debug.Log($"구매 완료: {product.productName}");

            // 상품 타입별 처리
            switch (product.productType)
            {
                case ProductType.Gold:
                    GiveGold(product.goldAmount);
                    break;

                case ProductType.PrestigePoints:
                    GivePrestigePoints(product.prestigePointAmount);
                    break;

                case ProductType.RemoveAds:
                    RemoveAds();
                    break;

                case ProductType.VIPPass:
                    ActivateVIP(product.vipDays);
                    break;

                case ProductType.StarterPack:
                    GiveGold(product.goldAmount);
                    GivePrestigePoints(product.prestigePointAmount);
                    hasPurchasedStarterPack = true;
                    break;
            }

            // 구매 데이터 저장
            SavePurchaseData();

            // 이벤트 발동
            OnPurchaseCompleted?.Invoke(product);
        }

        #endregion

        #region Reward Methods

        void GiveGold(BigDouble amount)
        {
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddGold(amount);
                Debug.Log($"골드 획득: {NumberFormatter.FormatNumber(amount)}");
            }
        }

        void GivePrestigePoints(BigDouble amount)
        {
            if (PrestigeManager.Instance != null)
            {
                PrestigeManager.Instance.prestigePoints += amount;
                Debug.Log($"PP 획득: {NumberFormatter.FormatNumber(amount)}");
            }
        }

        void RemoveAds()
        {
            hasRemovedAds = true;
            if (AdsManager.Instance != null)
            {
                AdsManager.Instance.RemoveAds();
            }
            Debug.Log("광고가 영구 제거되었습니다!");
        }

        void ActivateVIP(int days)
        {
            if (VIPManager.Instance != null)
            {
                VIPManager.Instance.ActivateVIP(days);
            }
            Debug.Log($"VIP 활성화: {days}일");
        }

        #endregion

        #region Helper Methods

        public IAPProduct GetProduct(string productId)
        {
            return products.Find(p => p.productId == productId);
        }

        public bool HasPurchasedStarterPack()
        {
            return hasPurchasedStarterPack;
        }

        public bool HasRemovedAds()
        {
            return hasRemovedAds;
        }

        #endregion

        #region Save/Load

        void SavePurchaseData()
        {
            PlayerPrefs.SetInt("IAP_RemovedAds", hasRemovedAds ? 1 : 0);
            PlayerPrefs.SetInt("IAP_StarterPack", hasPurchasedStarterPack ? 1 : 0);
            PlayerPrefs.Save();
        }

        void LoadPurchaseData()
        {
            hasRemovedAds = PlayerPrefs.GetInt("IAP_RemovedAds", 0) == 1;
            hasPurchasedStarterPack = PlayerPrefs.GetInt("IAP_StarterPack", 0) == 1;

            // 광고 제거 적용
            if (hasRemovedAds && AdsManager.Instance != null)
            {
                AdsManager.Instance.RemoveAds();
            }
        }

        #endregion
    }
}
