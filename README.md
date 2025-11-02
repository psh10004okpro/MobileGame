# 🥊 펀치킹 프로젝트 (Punch King)

Unity 6 기반 모바일 클릭커 게임 - 권투 트레이닝 테마

## 📋 프로젝트 개요

**펀치킹 프로젝트**는 Unity 6 엔진을 사용하여 제작된 모바일 최적화 클릭커 게임입니다.
초보 복서에서 시작하여 전설의 챔피언으로 성장하는 여정을 경험할 수 있습니다.

### 주요 특징
- ✅ **큰 숫자 시스템**: BreakInfinity.cs를 활용한 무한대 숫자 처리
- ✅ **프레스티지 시스템**: 게임을 리셋하고 영구 보너스 획득
- ✅ **캐릭터 진화**: 5단계 진화 시스템
- ✅ **스킬 시스템**: 쿨다운 기반 부스트 스킬
- ✅ **저장/로드**: JSON 기반 자동 저장
- ✅ **오프라인 진행**: 최대 4시간 오프라인 보상
- ✅ **일일 보상**: 연속 출석 보상 시스템

## 🎮 게임 메커니즘

### 1. 클릭 시스템
- 화면을 터치하여 골드 획득
- 크리티컬 히트 (기본 5% 확률)
- 클릭당 경험치 획득

### 2. 업그레이드 시스템
- **클릭 데미지**: 클릭당 골드 증가
- **DPS**: 초당 골드 자동 생산
- **크리티컬**: 확률 및 데미지 증가
- **골드 배율**: 전체 골드 획득량 증가

### 3. 프레스티지
- **조건**: 라이프타임 골드 1e15 이상
- **공식**: PP = 150 × √(LifetimeGold / 10^15)
- **효과**: 모든 스탯 리셋 + 영구 보너스

### 4. 캐릭터 진화
- 초보 복서 → 아마추어 → 프로 → 챔피언 → 전설
- 각 단계마다 스탯 대폭 증가

### 5. 스킬
- **클릭 부스트**: 클릭 데미지 2배 (30초)
- **생산 부스트**: DPS 2배 (30초)
- **골드 부스트**: 모든 골드 획득 2배 (30초)
- **크리티컬 스트라이크**: 100% 크리티컬 (30초)
- **즉시 데미지**: DPS × 1000 즉시 획득

## 🛠️ 기술 스택

### Unity 설정
- **Unity 버전**: Unity 6 LTS
- **렌더 파이프라인**: Universal Render Pipeline (URP)
- **타겟 플랫폼**: Android (API 24+), iOS
- **해상도**: 1080x1920 (세로 모드)
- **타겟 FPS**: 30

### 외부 라이브러리
- **BreakInfinity.cs**: 큰 숫자 처리 (BigDouble)
- **TextMeshPro**: UI 텍스트 렌더링 (Unity 내장)
- **DOTween**: 애니메이션 (선택사항)

## 📁 프로젝트 구조

```
Assets/
├── _Project/
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GameManager.cs
│   │   │   ├── CurrencyManager.cs
│   │   │   ├── UpgradeManager.cs
│   │   │   ├── UpgradeData.cs
│   │   │   ├── PrestigeManager.cs
│   │   │   ├── SaveManager.cs
│   │   │   └── DailyRewardManager.cs
│   │   ├── UI/
│   │   │   ├── UIManager.cs
│   │   │   └── UpgradeButton.cs
│   │   ├── Gameplay/
│   │   │   ├── ClickHandler.cs
│   │   │   ├── SkillSystem.cs
│   │   │   └── CharacterEvolution.cs
│   │   └── Utilities/
│   │       └── NumberFormatter.cs
│   ├── Data/
│   ├── Prefabs/
│   └── Scenes/
│       └── MainGame.unity
└── Plugins/
    └── BreakInfinity/
        └── BigDouble.cs
```

## 🚀 시작하기

### 1. Unity에서 프로젝트 열기
```bash
Unity Hub에서 프로젝트 추가
Unity 6 이상 버전 선택
```

### 2. 씬 설정
1. `Assets/_Project/Scenes/` 폴더에 메인 씬 생성
2. 빈 게임오브젝트 생성 및 매니저 컴포넌트 추가:
   - GameManager
   - CurrencyManager
   - UpgradeManager
   - PrestigeManager
   - CharacterEvolution
   - SkillSystem
   - SaveManager
   - DailyRewardManager
   - UIManager

3. UI Canvas 설정:
   - Canvas > Render Mode: Screen Space - Overlay
   - Canvas Scaler > UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1080x1920

### 3. 스크립트 설정
1. UIManager에 텍스트 필드 연결 (TextMeshPro)
2. UpgradeManager에 UpgradeData ScriptableObject 추가
3. ClickHandler에 클릭 대상 오브젝트 할당

### 4. ScriptableObject 생성
```
우클릭 > Create > PunchKing > Upgrade Data
필수 업그레이드 생성:
- Click Damage Upgrade
- Production DPS Upgrade
- Critical Chance Upgrade
- Critical Damage Upgrade
```

## ⚙️ 밸런싱 수치

### 업그레이드 비용
- 기본 비용: 100 골드
- 성장률: 1.15 (15% 증가)
- 공식: `비용 = 100 × (1.15)^레벨`

### 크리티컬
- 기본 확률: 5%
- 최대 확률: 35%
- 기본 데미지: ×1.5
- 최대 데미지: ×4.0

### 프레스티지
- 첫 프레스티지 권장: 1e15 골드
- PP 공식: `PP = 150 × √(LifetimeGold / 1e15)`
- 영구 보너스: +2% DPS per PP

### 진화 단계
1. **초보 복서** (Lv.1): 보너스 없음
2. **아마추어** (Lv.10): +50% 스탯
3. **프로 복서** (Lv.25): +100% 스탯
4. **챔피언** (Lv.50): +200% 스탯
5. **전설** (Lv.100): +500% 스탯

## 📱 빌드 설정

### Android
```
File > Build Settings > Android
- Minimum API Level: 24 (Android 7.0)
- Target API Level: 33
- Scripting Backend: IL2CPP
- Target Architectures: ARM64
- Compression Method: LZ4
```

### iOS
```
File > Build Settings > iOS
- Target SDK: Device SDK
- Architecture: ARM64
- Minimum iOS Version: 12.0
```

## 🔧 최적화 팁

### 성능 최적화
- 타겟 프레임레이트: 30 FPS
- VSync 비활성화
- UI 업데이트 간격: 0.1초
- Canvas 분리 (Static/Dynamic)

### 메모리 관리
- BigDouble 캐싱
- 오브젝트 풀링 (데미지 넘버)
- Update()에서 할당 최소화

## 📝 TODO

### 구현 완료 ✅
- [x] 코어 시스템 (Currency, Upgrade, Prestige)
- [x] 스킬 시스템
- [x] 캐릭터 진화
- [x] 저장/로드
- [x] UI 시스템
- [x] 오프라인 진행
- [x] 일일 보상

### 추가 구현 필요 🔨
- [ ] Unity Scene 구성
- [ ] UI 프리팹 제작
- [ ] 캐릭터 스프라이트
- [ ] 파티클 이펙트
- [ ] 사운드/음악
- [ ] 애니메이션 (DOTween)
- [ ] 업적 시스템
- [ ] 설정 메뉴

## 📄 라이선스

이 프로젝트는 학습 및 개발 목적으로 제작되었습니다.

### 외부 라이브러리
- **BreakInfinity.cs**: MIT License
- **TextMeshPro**: Unity Asset Store License

## 👥 개발자

- **프로젝트**: 펀치킹 (Punch King)
- **엔진**: Unity 6
- **개발 기간**: 2025

## 📞 문의

버그 리포트 및 기능 제안은 GitHub Issues를 통해 제출해 주세요.

---

**Made with Unity 6 & BreakInfinity.cs** 🥊
