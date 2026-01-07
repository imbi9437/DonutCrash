# 🍩 팀 프로젝트 - Donut Crash

<div align="center">
  <img width="1080" height="720" alt="Image" src="https://github.com/user-attachments/assets/1f44097e-3f95-4168-b4fa-39274aa47593" />
</div>

---

## 게임 소개

"슈팅 액션 X 강화 합성" 당겨서튕기고, 합쳐서강해지고, 강해져서 나아간다.
자신의 도넛을 튕겨서 상대방의 도넛을 날려버리고, 도넛과 제빵사를 강화해 상대방보다 강력한 덱을 구성해 나가는 알까기, 도넛 합성 게임입니다.

---

## 🎮 프로젝트 개요

| 항목 | 내용 |
|------|------|
| **프로젝트명** | 도넛 대격돌 (Donut Crash) |
| **개발 기간** | 2025.10 ~ 2025.12 |
| **개발 인원** | 기획 3인, 개발 6인 |
| **개발 엔진** | Unity 2022.3 LTS |
| **개발 언어** | C# |
| **타겟 플랫폼** | Android |

---

## 주요 기능

### 데이터 및 이벤트 기반 시스템
* 원활한 협업과 비동기 및 타이밍 처리를 위한 이벤트 기반 시스템
* 유연한 레벨디자인 및 밸런싱을 위한 데이터 기반 시스템
> #### 관련 스크립트 및 폴더 링크
> * [EventHub.cs](https://github.com/imbi9437/DonutCrash/blob/main/Assets/_Project/Scripts/Manager/EventHub.cs)
> * [EventParam](https://github.com/imbi9437/DonutCrash/tree/main/Assets/_Project/Scripts/EventStructs)
> * [DataManager.cs](https://github.com/imbi9437/DonutCrash/blob/main/Assets/_Project/Scripts/Manager/DataManager.cs)
> * [Data Getter & Setter](https://github.com/imbi9437/DonutCrash/tree/main/Assets/_Project/Scripts/Manager/DataManagerPartial)
> * [Data](https://github.com/imbi9437/DonutCrash/tree/main/Assets/_Project/Scripts/Data)

<br>

### 사용자 인증
* Firebase와 Google Play Games를 통한 로그인
* SDK 및 UniTask를 통한 비동기 구현
> #### 관련 스크립트 링크
> * [FirebaseAuthMethod.cs](https://github.com/imbi9437/DonutCrash/blob/main/Assets/_Project/Scripts/Manager/Firebase/FirebaseAuthMethod.cs)
> * [PlayGamesManager.cs](https://github.com/imbi9437/DonutCrash/blob/main/Assets/_Project/Scripts/Manager/PlayGamesManager.cs)

<br>

### 테이블 및 사용자 정보
* Firestore를 활용한 게임 정적 데이터 및 사용자 데이터 관리
> #### 관련 스크립트 링크
> * [FireStoreMethod.cs](https://github.com/imbi9437/DonutCrash/blob/main/Assets/_Project/Scripts/Manager/Firebase/FireStoreMethod.cs)

### 알까기 배틀
* 유한 상태 머신을 활용한 턴제 알까기 배틀
> #### 관련 폴더 링크
> * [Battle FSM](https://github.com/imbi9437/DonutCrash/tree/main/Assets/_Project/Scripts/InGame/StatePattern)

<br>

### 실시간 멀티플레이
* Photon Pun2를 활용한 실시간 멀티플레이
* 두 플레이어가 하나의 맵에서 서로의 도넛을 공격
> #### 관련 스크립트 링크
> * [PhotonManager.cs](https://github.com/imbi9437/DonutCrash/blob/main/Assets/_Project/Scripts/Manager/PhotonManager.cs)

<br>

### 리더보드
* Firebase와 쿼리를 통한 리더보드
> #### 관련 스크립트 링크
> * [FirebaseRealtimeMethod.cs](https://github.com/imbi9437/DonutCrash/blob/main/Assets/_Project/Scripts/Manager/Firebase/FirebaseRealtimeMethod.cs)
> * [LeaderboardEntry.cs](https://github.com/imbi9437/DonutCrash/blob/main/Assets/_Project/Scripts/Data/LeaderBoardEntry.cs)

<br>

### 도넛 해금, 생성, 합성
* 레시피를 획득해 다양한 종류의 도넛을 해금
* 해금된 도넛을 오븐에서 생성하여 게임 플레이에 사용
* 도넛을 합성하여 더욱 강한 도넛으로 강화
> #### 관련 폴더 링크
> * [Donut](https://github.com/imbi9437/DonutCrash/tree/main/Assets/_Project/Scripts/Donut)

<br>

### 덱
* 보유한 다양한 마녀와 도넛들로 덱을 구성해 게임에서 활용
* 배치 순서, 마녀 및 도넛의 스킬을 전략적으로 고려해 덱을 구성
> #### 관련 스크립트 링크
> * [DeckData.cs](https://github.com/imbi9437/DonutCrash/blob/main/Assets/_Project/Scripts/Data/DeckData.cs)
> * [DeckController.cs](https://github.com/imbi9437/DonutCrash/blob/main/Assets/_Project/Scripts/UI/DeckControll/DeckController.cs)

<br>

### 상점
* 재화를 통해 다양한 상품을 구매 및 재료 교환
* IAP를 통한 수익구조
> #### 관련 스크립트 링크
> * [ShopController.cs](https://github.com/imbi9437/DonutCrash/blob/main/Assets/_Project/Scripts/OutGame/Shop/ShopController.cs)
> * [MerchandiseData.cs](https://github.com/imbi9437/DonutCrash/blob/main/Assets/_Project/Scripts/Data/MerchandiseData.cs)
> * [ProductData.cs](https://github.com/imbi9437/DonutCrash/blob/main/Assets/_Project/Scripts/Data/ProductData.cs)

---
<br>

## Thrid Party Library



<div align="center">
  <img width="600" height="200" alt="Image" src="https://github.com/user-attachments/assets/962238c9-a2d8-4b1b-bf21-123bc94cfbe4" />
  <br>
</div>

<div align="center">
  <img width="500" height="200" alt="Image" src="https://github.com/user-attachments/assets/2673b41d-9ce4-499a-b7c6-02fcf11f4a45" />
  <br><br>
</div>

<div align="center">
  <img width="500" height="140" alt="Image" src="https://github.com/user-attachments/assets/82248976-8762-44f5-8377-a4191173fe54" />
</div>

  
