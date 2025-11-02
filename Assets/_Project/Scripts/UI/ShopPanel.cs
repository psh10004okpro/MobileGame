using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PunchKing.Monetization;
using System.Collections.Generic;

namespace PunchKing
{
    /// <summary>
    /// 상점 패널 (IAP 상품 표시)
    /// </summary>
    public class ShopPanel : MonoBehaviour
    {
        [Header("UI References")]
        public Transform productContainer;
        public GameObject productButtonPrefab;
        public Button closeButton;

        [Header("VIP 표시")]
        public GameObject vipPanel;
        public TextMeshProUGUI vipStatusText;
        public TextMeshProUGUI vipDaysText;

        private List<GameObject> productButtons = new List<GameObject>();

        void Start()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(() => gameObject.SetActive(false));
            }

            GenerateProductButtons();
            UpdateVIPDisplay();
        }

        void OnEnable()
        {
            GenerateProductButtons();
            UpdateVIPDisplay();
        }

        void GenerateProductButtons()
        {
            if (IAPManager.Instance == null || productContainer == null)
                return;

            // 기존 버튼 제거
            ClearButtons();

            // 각 상품에 대해 버튼 생성
            foreach (var product in IAPManager.Instance.products)
            {
                // 스타터 팩은 이미 구매했으면 표시 안 함
                if (product.productType == ProductType.StarterPack &&
                    IAPManager.Instance.HasPurchasedStarterPack())
                {
                    continue;
                }

                // 광고 제거는 이미 구매했으면 표시 안 함
                if (product.productType == ProductType.RemoveAds &&
                    IAPManager.Instance.HasRemovedAds())
                {
                    continue;
                }

                CreateProductButton(product);
            }
        }

        void CreateProductButton(IAPProduct product)
        {
            if (productButtonPrefab == null)
                return;

            GameObject buttonObj = Instantiate(productButtonPrefab, productContainer);

            // 버튼 설정
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnProductButtonClicked(product));
            }

            // 텍스트 설정
            TextMeshProUGUI[] texts = buttonObj.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 3)
            {
                texts[0].SetText(product.productName); // 이름
                texts[1].SetText(GetProductDescription(product)); // 설명
                texts[2].SetText(string.Format("${0:F2}", product.price)); // 가격
            }

            productButtons.Add(buttonObj);
        }

        string GetProductDescription(IAPProduct product)
        {
            switch (product.productType)
            {
                case ProductType.Gold:
                    return NumberFormatter.FormatNumber(product.goldAmount) + " Gold";

                case ProductType.PrestigePoints:
                    return NumberFormatter.FormatNumber(product.prestigePointAmount) + " PP";

                case ProductType.RemoveAds:
                    return "Remove all ads forever";

                case ProductType.VIPPass:
                    return string.Format("{0} days of VIP benefits", product.vipDays);

                case ProductType.StarterPack:
                    return string.Format("{0} Gold + {1} PP",
                        NumberFormatter.FormatNumber(product.goldAmount),
                        NumberFormatter.FormatNumber(product.prestigePointAmount));

                default:
                    return "";
            }
        }

        void OnProductButtonClicked(IAPProduct product)
        {
            if (IAPManager.Instance != null)
            {
                IAPManager.Instance.PurchaseProduct(product.productId);
            }
        }

        void ClearButtons()
        {
            foreach (var btn in productButtons)
            {
                if (btn != null)
                {
                    Destroy(btn);
                }
            }
            productButtons.Clear();
        }

        void UpdateVIPDisplay()
        {
            if (VIPManager.Instance == null || vipPanel == null)
                return;

            bool isVIP = VIPManager.Instance.IsVIP();
            vipPanel.SetActive(isVIP);

            if (isVIP)
            {
                if (vipStatusText != null)
                {
                    vipStatusText.SetText("VIP Active!");
                    vipStatusText.color = Color.yellow;
                }

                if (vipDaysText != null)
                {
                    int days = VIPManager.Instance.GetDaysRemaining();
                    vipDaysText.SetText(string.Format("{0} days remaining", days));
                }
            }
        }
    }
}
