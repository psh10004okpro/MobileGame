# 🎨 Unity UI 설정 가이드 - 펀치킹 프로젝트

Unity Editor에서 UI를 구성하는 완전한 가이드입니다.

---

## 📋 목차

1. [Canvas 기본 설정](#1-canvas-기본-설정)
2. [업그레이드 UI](#2-업그레이드-ui)
3. [스킬 UI](#3-스킬-ui)
4. [프레스티지 패널](#4-프레스티지-패널)
5. [일일 보상 패널](#5-일일-보상-패널)
6. [메인 게임 UI](#6-메인-게임-ui)

---

## 1. Canvas 기본 설정

### 1-1. Canvas 생성
```
Hierarchy 우클릭 > UI > Canvas
이름: "MainCanvas"
```

### 1-2. Canvas 설정
```
Canvas 컴포넌트:
- Render Mode: Screen Space - Overlay
- Pixel Perfect: 체크

Canvas Scaler:
- UI Scale Mode: Scale With Screen Size
- Reference Resolution: 1080 x 1920
- Screen Match Mode: Match Width Or Height
- Match: 0.5
```

### 1-3. UIManager 추가
```
MainCanvas에 UIManager 스크립트 추가
```

---

## 2. 업그레이드 UI

### 2-1. 업그레이드 패널 구조
```
MainCanvas
└── UpgradePanel (Panel)
    ├── Header (Image - 상단 바)
    │   ├── TitleText (TextMeshProUGUI) - "Upgrades"
    │   └── CloseButton (Button)
    │
    ├── ScrollView
    │   └── Viewport
    │       └── Content (Vertical Layout Group)
    │           └── (여기에 UpgradeButton들이 생성됨)
    │
    └── Background (Image - 어두운 배경)
```

### 2-2. UpgradePanel GameObject 생성
```
1. MainCanvas 우클릭 > UI > Panel
2. 이름: "UpgradePanel"
3. RectTransform:
   - Anchor: Stretch-Stretch
   - Left: 0, Right: 0, Top: 0, Bottom: 0
4. UpgradePanel 스크립트 추가
```

### 2-3. ScrollView 추가
```
UpgradePanel 우클릭 > UI > Scroll View
설정:
- Scroll Rect > Vertical: 체크
- Scroll Rect > Horizontal: 체크 해제
- Content Size Fitter 추가 (Content에)
  - Vertical Fit: Preferred Size
```

### 2-4. UpgradeButton 프리팹 제작

**Step 1: Button GameObject 생성**
```
Project 창 > Assets/_Project/Prefabs 폴더
우클릭 > Create > Folder (이름: UI)

Hierarchy에서:
우클릭 > UI > Button
이름: "UpgradeButton"
```

**Step 2: UpgradeButton 구조**
```
UpgradeButton
├── Icon (Image)
├── NameText (TextMeshProUGUI)
├── LevelText (TextMeshProUGUI)
├── CostText (TextMeshProUGUI)
└── DescriptionText (TextMeshProUGUI)
```

**Step 3: 레이아웃 설정**
```
UpgradeButton:
- RectTransform: Width 1000, Height 150
- Layout Element 추가:
  - Preferred Width: 1000
  - Preferred Height: 150

Icon:
- 위치: 왼쪽
- 크기: 100x100

NameText:
- 위치: 아이콘 옆
- Font Size: 32

LevelText:
- 위치: 오른쪽 위
- Font Size: 24

CostText:
- 위치: 오른쪽 아래
- Font Size: 28

DescriptionText:
- 위치: 중간 아래
- Font Size: 20
```

**Step 4: UpgradeButton 스크립트 연결**
```
UpgradeButton에 UpgradeButton.cs 스크립트 추가
Inspector에서 연결:
- Name Text → NameText
- Level Text → LevelText
- Cost Text → CostText
- Description Text → DescriptionText
- Icon Image → Icon
- Button → 자기 자신의 Button 컴포넌트
```

**Step 5: 프리팹 저장**
```
Hierarchy의 UpgradeButton을 드래그하여
Assets/_Project/Prefabs/UI 폴더로 이동
```

### 2-5. UpgradePanel 연결
```
MainCanvas > UpgradePanel 선택
UpgradePanel 컴포넌트:
- Upgrade Button Container: ScrollView/Viewport/Content
- Upgrade Button Prefab: UpgradeButton 프리팹
- Close Button: CloseButton
- Auto Generate Buttons: 체크
```

---

## 3. 스킬 UI

### 3-1. 스킬 패널 구조
```
MainCanvas
└── BottomPanel
    └── SkillContainer (Horizontal Layout Group)
        ├── SkillButton1
        ├── SkillButton2
        ├── SkillButton3
        ├── SkillButton4
        └── SkillButton5
```

### 3-2. SkillButton 프리팹 제작

**Step 1: Button GameObject**
```
Hierarchy 우클릭 > UI > Button
이름: "SkillButton"
크기: 180x180
```

**Step 2: SkillButton 구조**
```
SkillButton
├── Icon (Image)
├── CooldownFill (Image - Fill Amount)
├── SkillNameText (TextMeshProUGUI)
└── CooldownText (TextMeshProUGUI)
```

**Step 3: CooldownFill 설정**
```
CooldownFill:
- Image Type: Filled
- Fill Method: Radial 360
- Fill Origin: Top
- Fill Amount: 0
- Color: Red (255, 0, 0, 128)
```

**Step 4: SkillButton 스크립트 연결**
```
SkillButton.cs 추가
연결:
- Skill Name Text → SkillNameText
- Cooldown Text → CooldownText
- Icon Image → Icon
- Cooldown Fill Image → CooldownFill
- Button → 자기 자신
- Skill Index: 0 (각 버튼마다 0, 1, 2, 3, 4로 설정)
```

**Step 5: 5개 복제**
```
SkillButton을 4번 복제 (Ctrl+D)
각각 Skill Index를 0, 1, 2, 3, 4로 설정
```

### 3-3. GameManagers의 SkillSystem 설정

**Step 1: 스킬 데이터 추가**
```
GameManagers > SkillSystem 선택
Skills 리스트에 5개 추가:

Skill 0 - Click Boost:
- Skill Name: "Click Boost"
- Description: "2x Click Damage for 30s"
- Skill Type: ClickBoost
- Duration: 30
- Cooldown: 60
- Multiplier: 2

Skill 1 - Production Boost:
- Skill Name: "Production Boost"
- Description: "2x DPS for 30s"
- Skill Type: ProductionBoost
- Duration: 30
- Cooldown: 60
- Multiplier: 2

Skill 2 - Gold Boost:
- Skill Name: "Gold Boost"
- Description: "2x Gold for 30s"
- Skill Type: GoldBoost
- Duration: 30
- Cooldown: 60
- Multiplier: 2

Skill 3 - Critical Strike:
- Skill Name: "Critical Strike"
- Description: "100% Critical for 30s"
- Skill Type: CriticalStrike
- Duration: 30
- Cooldown: 90

Skill 4 - Instant Damage:
- Skill Name: "Instant Damage"
- Description: "Instant DPS x1000"
- Skill Type: InstantDamage
- Cooldown: 120
```

---

## 4. 프레스티지 패널

### 4-1. 프레스티지 패널 구조
```
MainCanvas
└── PrestigePanel (Panel)
    ├── Header
    │   ├── TitleText - "Prestige"
    │   └── CloseButton
    │
    ├── Content
    │   ├── CurrentPPText (TextMeshProUGUI)
    │   ├── PrestigeLevelText (TextMeshProUGUI)
    │   ├── NextPPGainText (TextMeshProUGUI)
    │   ├── LifetimeGoldText (TextMeshProUGUI)
    │   ├── WarningText (TextMeshProUGUI)
    │   └── PrestigeButton (Button)
    │
    └── ConfirmPanel (Panel - 비활성화 상태)
        ├── ConfirmText - "Are you sure?"
        ├── YesButton
        └── NoButton
```

### 4-2. PrestigePanel 생성
```
1. MainCanvas 우클릭 > UI > Panel
2. 이름: "PrestigePanel"
3. PrestigePanel 스크립트 추가
4. 비활성화 (초기에는 보이지 않음)
```

### 4-3. 스크립트 연결
```
PrestigePanel 컴포넌트:
- Current PP Text → CurrentPPText
- Prestige Level Text → PrestigeLevelText
- Next PP Gain Text → NextPPGainText
- Lifetime Gold Text → LifetimeGoldText
- Warning Text → WarningText
- Prestige Button → PrestigeButton
- Confirm Panel → ConfirmPanel
- Confirm Yes Button → YesButton
- Confirm No Button → NoButton
```

### 4-4. UIManager 연결
```
MainCanvas > UIManager:
- Prestige Panel → PrestigePanel
```

---

## 5. 일일 보상 패널

### 5-1. 일일 보상 패널 구조
```
MainCanvas
└── DailyRewardPanel (Panel)
    ├── Header
    │   ├── TitleText - "Daily Reward"
    │   └── CloseButton
    │
    ├── Content
    │   ├── RewardAmountText (TextMeshProUGUI)
    │   ├── ConsecutiveDaysText (TextMeshProUGUI)
    │   ├── NextRewardTimeText (TextMeshProUGUI)
    │   ├── DayContainer (Horizontal Layout Group)
    │   │   ├── Day1 (Image)
    │   │   ├── Day2 (Image)
    │   │   ├── Day3 (Image)
    │   │   ├── Day4 (Image)
    │   │   ├── Day5 (Image)
    │   │   ├── Day6 (Image)
    │   │   └── Day7 (Image)
    │   └── ClaimButton (Button)
    │
    └── Background
```

### 5-2. DailyRewardPanel 생성
```
1. MainCanvas 우클릭 > UI > Panel
2. 이름: "DailyRewardPanel"
3. DailyRewardPanel 스크립트 추가
4. 비활성화
```

### 5-3. 7일 아이콘 생성
```
DayContainer:
- Horizontal Layout Group 추가
- Spacing: 20
- Child Force Expand: Width, Height 체크

각 Day 이미지:
- 크기: 100x100
- 색상: Gray (초기)
- 완료 시 Green으로 변경 (스크립트에서 자동)
```

### 5-4. 스크립트 연결
```
DailyRewardPanel 컴포넌트:
- Reward Amount Text → RewardAmountText
- Consecutive Days Text → ConsecutiveDaysText
- Next Reward Time Text → NextRewardTimeText
- Claim Button → ClaimButton
- Close Button → CloseButton
- Day Reward Objects: Day1~Day7 (배열 크기 7)
```

---

## 6. 메인 게임 UI

### 6-1. 전체 UI 구조
```
MainCanvas (UIManager)
├── TopPanel
│   ├── GoldText
│   ├── DPSText
│   ├── ClickDamageText
│   └── LevelText
│
├── CenterPanel
│   └── SandbagImage (ClickHandler)
│
├── BottomPanel
│   ├── UpgradeButton (메인 버튼)
│   ├── PrestigeButton (메인 버튼)
│   ├── DailyRewardButton (메인 버튼)
│   └── SkillContainer
│       └── (5개 스킬 버튼)
│
├── DamageNumberParent (빈 오브젝트)
│
├── UpgradePanel (비활성화)
├── PrestigePanel (비활성화)
└── DailyRewardPanel (비활성화)
```

### 6-2. TopPanel 생성
```
1. MainCanvas 우클릭 > UI > Panel
2. 이름: "TopPanel"
3. Anchor: Top-Stretch
4. Height: 200
```

**TextMeshProUGUI 추가:**
```
GoldText:
- Text: "Gold: 0"
- Font Size: 48
- Alignment: Left
- Position: 왼쪽 위

DPSText:
- Text: "DPS: 0/s"
- Font Size: 36
- Position: GoldText 아래

ClickDamageText:
- Text: "Click: 1"
- Font Size: 36
- Position: DPSText 아래

LevelText:
- Text: "Lv.1"
- Font Size: 40
- Position: 오른쪽 위
```

### 6-3. CenterPanel (클릭 영역)
```
1. MainCanvas 우클릭 > UI > Image
2. 이름: "SandbagImage"
3. Anchor: Center
4. 크기: 400x400
5. 색상: 흰색 (임시)
```

**ClickHandler 추가:**
```
SandbagImage에 ClickHandler 스크립트 추가
설정:
- Target Object: 자기 자신
- Use Raycast: true
- Punch Scale: 0.15
- Punch Duration: 0.3
- Give Exp On Click: true
- Exp Per Click: 1
```

**클릭 감지를 위한 설정:**
```
방법 1: UI Button 사용
- SandbagImage에 Button 컴포넌트 추가
- Transition: None

방법 2: EventTrigger 사용
- SandbagImage에 Event Trigger 추가
- Add New Event Type: PointerClick
```

### 6-4. BottomPanel (버튼들)
```
1. MainCanvas 우클릭 > UI > Panel
2. 이름: "BottomPanel"
3. Anchor: Bottom-Stretch
4. Height: 300
```

**메인 버튼 3개 생성:**
```
UpgradeButton (Button):
- Text: "Upgrades"
- 크기: 300x100
- OnClick: UIManager.ToggleUpgradePanel()

PrestigeButton (Button):
- Text: "Prestige"
- 크기: 300x100
- OnClick: UIManager.TogglePrestigePanel()

DailyRewardButton (Button):
- Text: "Daily Reward"
- 크기: 300x100
- OnClick: DailyRewardPanel.SetActive(true)
```

### 6-5. UIManager 최종 연결
```
MainCanvas > UIManager 선택

메인 UI 텍스트:
- Gold Text → TopPanel/GoldText
- DPS Text → TopPanel/DPSText
- Click Damage Text → TopPanel/ClickDamageText
- Level Text → TopPanel/LevelText

패널:
- Upgrade Panel → UpgradePanel
- Prestige Panel → PrestigePanel
- Skill Panel → (없으면 비워둠)
- Settings Panel → (없으면 비워둠)

데미지 넘버:
- Damage Number Parent → DamageNumberParent
```

---

## 🎯 테스트 체크리스트

### 필수 테스트
- [ ] Play 버튼 눌러서 에러 없음
- [ ] 샌드백 클릭 시 골드 증가
- [ ] TopPanel 텍스트 업데이트 확인
- [ ] Upgrades 버튼 → 패널 열림
- [ ] 업그레이드 구매 가능 (Console 로그)
- [ ] Prestige 버튼 → 패널 열림
- [ ] 스킬 버튼 5개 클릭 가능
- [ ] 스킬 쿨다운 표시 작동
- [ ] 일일 보상 패널 열림
- [ ] 게임 종료 후 재시작 시 저장 데이터 로드

---

## 💡 팁

### UI 디버깅
```
Console 창을 열어두고 (Window > General > Console)
에러 메시지 확인
```

### 빠른 테스트
```
GameManagers > CurrencyManager:
- Gold: 999999999 (시작 골드 설정)
- Gold Per Click: 1000 (빠른 테스트용)
```

### 해상도 테스트
```
Game 창 > Free Aspect > 9:16 (모바일)
여러 해상도에서 UI 확인
```

---

## 📝 다음 단계

UI 구성이 완료되면:
1. **비주얼 개선**: 스프라이트, 아이콘 추가
2. **파티클**: 클릭 이펙트, 레벨업 이펙트
3. **사운드**: 클릭음, BGM
4. **모바일 빌드**: Android/iOS 테스트

---

**작성자**: Claude AI
**버전**: 1.0
**업데이트**: 2025-11-02
