# 💰 펀치킹 프로젝트 - 수익화 시스템 가이드

Unity에서 광고와 IAP를 통합하는 완전한 가이드입니다.

---

## 📋 구현된 수익화 시스템

### ✅ 광고 시스템 (AdsManager)
- 보상형 광고 (Rewarded Video)
- 전면 광고 (Interstitial)
- 배너 광고 (Banner)

### ✅ IAP 시스템 (IAPManager)
- 골드 팩 (소/중/대)
- PP 팩 (스타터/메가)
- 광고 제거 ($4.99)
- VIP 패스 (30일)
- 스타터 팩 (한정)

### ✅ VIP 시스템 (VIPManager)
- 골드 +50%
- DPS +50%
- 일일 보상 2배
- 광고 제거

---

## 🚀 Unity 설정 (단계별)

### Step 1: Unity Ads 설정

#### 1-1. Unity Ads 설치
```
Window > Package Manager
Unity Registry > Advertisements
Install
```

#### 1-2. Unity Dashboard 설정
```
1. Unity Dashboard (https://dashboard.unity3d.com) 접속
2. 프로젝트 선택
3. Monetization > Ads 활성화
4. Game ID 복사:
   - Android: ca-app-pub-xxxxxx
   - iOS: ca-app-pub-xxxxxx
```

#### 1-3. Unity Editor 설정
```
Window > General > Services
Unity Ads 활성화
Game ID 입력
Test Mode: 체크 (개발 중)
```

#### 1-4. 광고 단위 ID 설정
```
Android:
- Rewarded: "Rewarded_Android"
- Interstitial: "Interstitial_Android"
- Banner: "Banner_Android"

iOS:
- Rewarded: "Rewarded_iOS"
- Interstitial: "Interstitial_iOS"
- Banner: "Banner_iOS"
```

---

### Step 2: Unity IAP 설정

#### 2-1. Unity IAP 설치
```
Window > Package Manager
Unity Registry > In-App Purchasing
Install
```

#### 2-2. Google Play Console 설정 (Android)
```
1. Google Play Console 접속
2. 앱 선택 > 수익 창출 > 인앱 상품
3. 상품 추가:
   - gold_pack_small ($0.99)
   - gold_pack_medium ($4.99)
   - gold_pack_large ($9.99)
   - pp_pack_starter ($2.99)
   - pp_pack_mega ($9.99)
   - remove_ads ($4.99) - 비소모성
   - vip_pass_30days ($9.99)
   - starter_pack ($0.99)
```

#### 2-3. App Store Connect 설정 (iOS)
```
1. App Store Connect 접속
2. 앱 선택 > In-App Purchases
3. 동일한 Product ID로 상품 생성
```

---

## 🎮 Unity Scene 설정

### GameManagers에 추가
```
1. GameManagers GameObject 선택
2. 다음 스크립트 추가:
   - AdsManager
   - IAPManager
   - VIPManager
```

### AdsManager 설정
```
Enable Ads: 체크
Test Mode: 체크 (개발 중)
Enable Rewarded Ads: 체크
Enable Interstitial Ads: 체크
Enable Banner Ads: 선택
Interstitial Interval: 300 (5분)
```

### IAPManager 설정
```
Enable IAP: 체크
Test Mode: 체크 (개발 중)
Products: 자동 생성됨 (Start 시)
```

### VIPManager 설정
```
Gold Multiplier Bonus: 0.5 (+50%)
DPS Multiplier Bonus: 0.5 (+50%)
Daily Reward Multiplier: 2 (2배)
Remove Ads: 체크
```

---

## 📱 UI 구성

### AdButton 버튼 생성

```
TopPanel에 추가:

1. 골드 2배 버튼
   - Button 생성
   - AdButton 스크립트 추가
   - Reward Type: DoubleGold
   - Duration: 1800 (30분)
   - Multiplier: 2

2. DPS 2배 버튼
   - Button 생성
   - AdButton 스크립트 추가
   - Reward Type: DoubleDPS
   - Duration: 3600 (1시간)
   - Multiplier: 2

3. 즉시 골드 버튼
   - Reward Type: InstantGold
```

### Shop Panel 생성

```
1. MainCanvas > UI > Panel
2. 이름: "ShopPanel"
3. ShopPanel 스크립트 추가
4. ScrollView 추가
5. ProductButtonPrefab 연결
```

### Shop Button (메인)
```
BottomPanel에 "Shop" 버튼 추가
OnClick: ShopPanel.SetActive(true)
```

---

## 🧪 테스트

### 광고 테스트
```
1. Test Mode 활성화
2. Play 모드 진입
3. AdButton 클릭
4. Console에 "테스트 모드: 보상형 광고 시청 완료" 확인
5. 골드/DPS 2배 확인
```

### IAP 테스트
```
1. Test Mode 활성화
2. Shop 패널 열기
3. 상품 버튼 클릭
4. Console에 "구매 완료: ..." 확인
5. 골드/PP 증가 확인
```

### VIP 테스트
```
1. VIP 패스 구매
2. TopPanel에 VIP 아이콘 표시
3. 골드 +50%, DPS +50% 확인
4. 광고 버튼 비활성화 확인
```

---

## 💡 코드 통합 예시

### AdsManager 사용

```csharp
// 보상형 광고 시청
AdsManager.Instance.ShowRewardedAd(success =>
{
    if (success)
    {
        // 보상 지급
        CurrencyManager.Instance.goldMultiplier *= 2;
    }
});

// 전면 광고 표시
AdsManager.Instance.ShowInterstitialAd();

// 프레스티지 후 광고
AdsManager.Instance.ShowAdAfterPrestige();
```

### IAPManager 사용

```csharp
// 골드 팩 구매
IAPManager.Instance.PurchaseProduct("gold_pack_small");

// 구매 완료 이벤트
IAPManager.Instance.OnPurchaseCompleted += (product) =>
{
    Debug.Log($"구매 완료: {product.productName}");
};
```

### VIPManager 사용

```csharp
// VIP 확인
if (VIPManager.Instance.IsVIP())
{
    // VIP 전용 기능
}

// VIP 남은 일수
int days = VIPManager.Instance.GetDaysRemaining();
```

---

## 📊 수익 모델 권장 설정

### 무료 유저 경험
```
- 광고 시청으로 부스트 가능
- 모든 콘텐츠 접근 가능
- 일일 광고 시청 제한: 없음
```

### 광고 빈도
```
- 보상형 광고: 무제한 (자발적)
- 전면 광고: 5분마다 1회
- 배너 광고: 상시 (선택)
```

### 가격 전략
```
스타터 팩: $0.99 (전환율 높임)
골드 소액: $0.99
골드 중간: $4.99 (Best Value)
골드 대형: $9.99
광고 제거: $4.99
VIP 패스: $9.99
```

---

## 🚨 주의사항

### 개발 중
- Test Mode 항상 활성화
- 실제 결제 발생하지 않음
- 테스트 광고 표시

### 배포 전
- Test Mode 비활성화
- Google Play / App Store 상품 활성화
- 광고 단위 ID 확인
- 정책 준수 확인

### 정책
- 광고 표시 전 로딩 확인
- IAP 영수증 검증
- 아동 대상 앱: COPPA 준수
- GDPR: 유럽 사용자 동의

---

## 📈 분석 및 최적화

### Unity Analytics 연동
```
Window > General > Services
Analytics 활성화
```

### 추적할 지표
- 광고 시청률
- IAP 전환율
- ARPU (Average Revenue Per User)
- ARPPU (Average Revenue Per Paying User)
- LTV (Lifetime Value)

---

## 🔧 문제 해결

### 광고가 표시되지 않음
```
1. Test Mode 확인
2. Game ID 확인
3. 인터넷 연결 확인
4. 광고 준비 상태 확인: IsRewardedAdReady()
```

### IAP 구매 실패
```
1. Product ID 일치 확인
2. Google Play Console 활성화 확인
3. 테스트 계정 설정
4. 빌드 서명 확인
```

### VIP 만료 안 됨
```
1. DateTime 동기화 확인
2. PlayerPrefs 저장 확인
3. 기기 시간 변경 감지
```

---

## 📝 다음 단계

수익화 시스템 구현 후:
1. ✅ Unity Ads 통합
2. ✅ Unity IAP 통합
3. ✅ UI 구성
4. 📊 Analytics 연동
5. 🧪 베타 테스트
6. 🚀 배포
7. 📈 성과 분석 및 최적화

---

**작성자**: Claude AI
**버전**: 1.0
**업데이트**: 2025-11-02
