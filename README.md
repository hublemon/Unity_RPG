# 영상 데이터셋 조사 — Action Anticipation / Verb·Noun 구조 중심

**기준일**: 2026-05-13  
**목적**: V-JEPA 2 Action Anticipation에서 사용한 EK100 이외에, verb/noun 예측 또는 action anticipation 실험에 활용할 수 있는 영상 데이터셋 정리

---

## 0. 한눈에 비교

| 데이터셋 | Egocentric 여부 | 시점 | verb/noun 분리 | 규모 | 도메인 | Action Anticipation 적합도 |
|----------|:---------------:|:----:|:--------------:|------|--------|----------------------------|
| **EPIC-KITCHENS-100** | ✅ 예 | 1인칭 | ✅ 별도 ID | 700h / 90K clips | 주방 | 기준 데이터셋. verb/noun/action anticipation 모두 직접 가능 |
| **EGTEA Gaze+** | ✅ 예 | 1인칭 | △ 복합 클래스 | 28h / 10K clips | 주방 | 소규모 1인칭 조리 행동. gaze까지 활용 가능 |
| **Assembly101** | ✅ 부분적 | 1인칭+다중 3인칭 | ✅ 별도 ID | 513h / 1.3M clips | 장난감 조립 | EK100과 가장 비슷하게 verb/noun 구조를 갖춘 절차 데이터셋 |
| **Ego4D** | ✅ 예 | 1인칭 | △ narration 파싱 | 3,670h | 일상 전반 | 대규모 사전학습 및 long/short-term anticipation에 강함 |
| **Ego-Exo4D** | ✅ 예 | 1인칭+3인칭 쌍 | △ keystep/narration | 1,422h | 기술 과제 | 시점 불변 표현, ego-exo 전이, 절차 예측에 적합 |
| **HICO-DET** | ❌ 아니오 | 3인칭 이미지 | ✅ verb+object | 47K images | 일반 장면 | 영상 anticipation용은 아니지만 HOI verb/object 구조 참고 가능 |
| **V-COCO** | ❌ 아니오 | 3인칭 이미지 | ✅ action+object | 10K images | 일반 장면 | 이미지 기반 semantic role/HOI 보조 데이터로 적합 |
| **Charades** | ❌ 아니오 | 3인칭 | △ 복합 문장 | 9.8K videos | 가정 내 일상 | temporal detection, multi-label action 이해에 적합 |
| **Something-Something v2** | ❌ 아니오 | 3인칭 | ✗ 템플릿 문장 | 220K videos | 물체 조작 | 시간적 관계/물리적 변화 추론 평가에 강함 |
| **COIN** | ❌ 아니오 | 3인칭 | ✗ 단일 step ID | 11.8K videos | 절차적 작업 | procedural step anticipation 후보. V-JEPA2 코드에도 분기 존재 |
| **Kinetics-700** | ❌ 아니오 | 주로 3인칭 | ✗ 단일 복합 태그 | 650K videos | 일반 행동 | 범용 action representation 평가용. anticipation 직접성은 낮음 |

> **verb/noun 분리** 기준:  
> ✅ = 데이터셋 자체에서 verb ID / noun ID가 독립 컬럼으로 제공  
> △ = 구조는 있지만 별도 정수 ID 없이 복합 문장이거나 NLP 파싱 필요  
> ✗ = 단일 action 레이블, verb/noun 분리 없음

> **Egocentric 여부** 기준:  
> ✅ 예 = 착용형 카메라 또는 수행자 관점의 1인칭 영상이 핵심  
> ✅ 부분적 = 1인칭 영상이 포함되지만 3인칭/다중시점도 함께 제공  
> ❌ 아니오 = 관찰자 시점, 웹 영상, 영화/가정 촬영, 이미지 중심

---

## 1. EPIC-KITCHENS-100 (EK100) — 기준

- **제작**: University of Bristol (Dima Damen 외, 2022)
- **논문**: *Rescaling Egocentric Vision* (IJCV 2022)
- **Egocentric 여부**: ✅ **예**. 참가자가 머리 또는 몸에 장착한 카메라로 촬영한 1인칭 주방 영상
- **규모**: 700시간 / 90,000+ clips / 37명 참가자
- **Verb 클래스**: 97개  **Noun 클래스**: 300개  **Action 클래스**: ~3,806개 (train 기준)
- **어노테이션 방식**: 참가자가 직접 말한 내러티브("take the bag")를 수집 후, 제작팀이 동의어·표기 변형을 수동으로 통합하여 정수 ID 부여
- **Action Anticipation 관점**:
  - verb_class / noun_class가 CSV에 독립 컬럼으로 존재
  - V-JEPA 2의 EK100 action anticipation probe가 바로 사용하는 표준 벤치마크
  - 관찰 구간이 액션 시작보다 앞에서 끝나도록 구성해, "곧 무엇을 할지"와 "어떤 물체가 대상인지"를 직접 평가 가능
  - 전체 영상 길이의 ~80%는 "배경(no action)"이라 temporal localization도 중요
  - Action Anticipation, Action Recognition, Action Detection 등 다양한 태스크 정의
  - 공식 평가 서버 존재 (EvalAI)
- **주의점**:
  - 주방 도메인에 강하게 치우쳐 있어 일반 생활 행동으로 확장하려면 추가 데이터가 필요
  - noun class가 세밀하고 long-tail이 강해 물체 예측 난도가 높음

```
EPIC_100_train.csv 구조 (주요 컬럼)
narration_id | video_id | start_frame | stop_frame | verb | verb_class | noun | noun_class
P01_01_0     | P01_01   | 8           | 202        | open | 3          | door | 3
```

---

## 2. EGTEA Gaze+

- **제작**: Georgia Tech (Yin Li 외, 2018)
- **논문**: *In the Eye of the Beholder: Gaze and Actions in First Person Video* (ECCV 2018)
- **Egocentric 여부**: ✅ **예**. 요리 수행자의 1인칭 시점으로 촬영된 egocentric video dataset
- **규모**: 28시간 / 10,321 clips / 32명 / 요리 활동
- **Action 클래스**: 106개 (verb+noun 복합, e.g., "take bread", "cut tomato")
- **Verb**: 19종  **Noun(object)**: 51종
- **verb/noun 구조**: action_id가 내부적으로 verb×noun 조합이나, **별도 정수 ID 컬럼은 없음** — action label 텍스트를 파싱해서 분리해야 함
- **추가 제공**: 시선 추적(gaze map) 데이터 — 사람이 어디를 보면서 행동하는지 함께 기록
- **Action Anticipation 관점**:
  - EK100과 같은 주방·조리 도메인이라 domain gap이 비교적 작음
  - gaze map을 행동 직전의 attention signal로 활용할 수 있음
  - 규모가 작아 대규모 probe 학습보다는 transfer test, few-shot 실험, 보조 평가에 적합
- **주의점**:
  - EK100식 공식 anticipation protocol을 그대로 기대하기 어렵고, 독립 verb/noun 평가는 라벨 정제가 필요

---

## 3. Assembly101

- **제작**: TU Munich / ETH Zurich (Sener 외, 2022)
- **논문**: *Assembly101: A Large-Scale Multi-View Video Dataset for Understanding Procedural Activities* (CVPR 2022)
- **Egocentric 여부**: ✅ **부분적**. 수행자의 1인칭 영상과 여러 대의 3인칭 고정 카메라 영상이 함께 제공되는 multi-view 데이터셋
- **규모**: 513시간 / 1,380 sequences / 53명 / 101개 장난감 조립·분해
- **Verb 클래스**: 24개 (attach, detach, rotate, flip 등 조작 동사)  **Noun 클래스**: 90개 (부품 이름)
- **verb/noun 구조**: ✅ **별도 ID로 제공** — EK100과 유사한 구조
- **Action Anticipation 관점**:
  - EK100 이외 후보 중 verb/noun 독립 평가를 가장 직접적으로 설계하기 좋음
  - 다중 카메라(12대) 동시 촬영 → 다양한 시점에서 동일 행동 관찰 가능
  - 조립/분해 절차가 명확하여 temporal ordering 연구에 적합
  - Fine-grained: "attach front-left-wheel to axle" 수준의 세밀한 라벨
- **주의점**:
  - 장난감 조립 도메인이라 EK100의 주방 행동·물체 vocabulary와는 차이가 큼
  - multi-view 구조를 제대로 쓰려면 데이터 로더와 sampling 설계가 EK100보다 복잡함

```
annotation 예시
verb_id | verb    | noun_id | noun
5       | attach  | 12      | front-left-wheel
6       | detach  | 12      | front-left-wheel
```

---

## 4. Ego4D

- **제작**: Meta AI / 9개국 13개 대학 컨소시엄 (Grauman 외, 2022)
- **논문**: *Ego4D: Around the World in 3,000 Hours of Egocentric Video* (CVPR 2022)
- **Egocentric 여부**: ✅ **예**. 착용형 카메라로 촬영된 대규모 1인칭 일상 영상이 핵심
- **규모**: 3,670시간 / 74개 위치 / 약 930명 참가자
- **어노테이션 방식**: 자연어 내러티브 narration ("I pick up the spoon") — 구조화된 verb/noun ID 없음
- **verb/noun 구조**: △ NLP 파싱으로 추출 가능하나 공식 제공은 아님
- **제공 태스크**:
  - **Episodic Memory**: "내가 마지막으로 열쇠를 어디 뒀지?"
  - **Hands & Objects**: 손과 접촉 물체 위치 추적
  - **Short-term Object Interaction Anticipation**: 다음 1초 내 어떤 물체를 집을지
  - **Long-term Action Anticipation**: 3~8분 후 행동 예측
  - **Natural Language Queries, Moments in Time** 등
- **접근**: 학술 신청 후 다운로드 (ego4d-data.org)
- **Action Anticipation 관점**:
  - EK100보다 훨씬 크고 도메인이 넓어 1인칭 사전학습·전이 실험에 강함
  - short-term object interaction anticipation과 long-term action anticipation 태스크가 이미 정의되어 있음
  - narration 기반이라 language-conditioned anticipation 또는 narration-to-label 변환 연구에도 활용 가능
- **주의점**:
  - EK100처럼 바로 verb/noun classification head를 붙이려면 label ontology 구축이 필요
  - 데이터 규모가 커서 저장공간·전처리 비용이 큼

---

## 5. Ego-Exo4D

- **제작**: Meta AI / 컨소시엄 (Grauman 외, 2024)
- **논문**: *Ego-Exo4D: Understanding Skilled Human Activity from First- and Third-person Perspectives* (CVPR 2024)
- **Egocentric 여부**: ✅ **예**. 수행자 1인칭 영상과 외부 3인칭 영상을 동기화해 제공
- **규모**: 1,422시간 / 1인칭·3인칭 동시 촬영 쌍
- **도메인**: 스포츠(야구, 농구), 요리, 자전거 수리, 댄스, 음악 연주 등 "기술 과제"
- **verb/noun 구조**: △ Keystep 어노테이션 (절차적 단계 레이블) — EK100 방식의 독립 ID는 없음
- **Action Anticipation 관점**:
  - 동일 행동을 1인칭(수행자)과 3인칭(관찰자) 양쪽에서 동시에 촬영 → 시점 불변 표현 학습에 활용
  - 숙련 기술 과제는 단계성과 목표가 뚜렷해 다음 keystep, 다음 동작, 실패 가능성 예측으로 확장하기 좋음
  - 1인칭에서 안 보이는 자세·전신 정보를 3인칭 context로 보완할 수 있음
- **주의점**:
  - EK100식 verb/noun pair보다는 keystep·skill 중심이라, verb/noun benchmark로 쓰려면 ontology 설계가 필요

---

## 6. HICO-DET

- **제작**: University of Michigan (Chao 외, 2018)
- **논문**: *Learning to Detect Human-Object Interactions* (WACV 2018)
- **Egocentric 여부**: ❌ **아니오**. COCO 이미지 기반의 3인칭 일반 장면 데이터셋
- **규모**: 47,776 이미지 / 150K HOI 인스턴스
- **구조**: 117개 verb(interaction) × 80개 object(COCO 클래스) = 600개 HOI 조합
- **verb/noun 구조**: ✅ interaction(verb) + object(noun) 독립 정의
- **Action Anticipation 관점**:
  - **이미지 기반** — 영상 아님
  - Human-Object Interaction Detection: bounding box + interaction 레이블 동시 예측
  - "riding horse", "cutting cake" 같은 동사+명사 쌍이 명시적으로 정의됨
  - 시간적 anticipation을 직접 평가할 수는 없지만, HOI verb/object ontology와 object-centric representation 보강에 참고 가능
- **주의점**:
  - temporal ordering, action start, anticipation time 같은 개념이 없음
  - 1인칭 손-물체 상호작용과 카메라 구도가 크게 다름

---

## 7. V-COCO (Verbs in COCO)

- **제작**: COCO 확장 (Gupta & Malik, 2015)
- **논문**: *Visual Semantic Role Labeling* (arXiv 2015)
- **Egocentric 여부**: ❌ **아니오**. COCO 이미지 기반의 3인칭 장면 데이터셋
- **규모**: 10,346 이미지 (COCO 서브셋)
- **구조**: 26개 action × 80개 COCO object 클래스
- **verb/noun 구조**: ✅ action(verb) + object(noun) 독립
- **Action Anticipation 관점**:
  - HICO-DET보다 소규모이며, "sit on", "hold", "cut" 등 일상적 interaction 중심
  - 영상 anticipation 벤치마크는 아니지만, verb와 object role의 결합을 학습하는 보조 데이터로 참고 가능
- **주의점**:
  - 시간 정보가 없으므로 V-JEPA 2 predictor의 미래 token 예측 능력을 직접 평가하기 어려움

---

## 8. Charades

- **제작**: Allen Institute for AI (Sigurdsson 외, 2016)
- **논문**: *Hollywood in Homes: Crowdsourcing Data Collection for Activity Understanding* (ECCV 2016)
- **Egocentric 여부**: ❌ **아니오**. 가정 내부에서 3인칭으로 촬영된 scripted video dataset
- **규모**: 9,848 videos (평균 30초) / 27,847 clips / 267명
- **Action 클래스**: 157개 (e.g., "Holding a blanket", "Washing dishes")
- **verb/noun 구조**: △ 복합 구 형태 — 내부적으로 verb+object가 하나의 문장으로 묶여 있어 분리 가능하지만 공식 ID는 단일
- **Action Anticipation 관점**:
  - Temporal Action Detection 벤치마크로 주로 사용
  - 하나의 클립에 여러 action이 동시에 발생 가능 (multi-label)
  - Charades-STA: 자연어 쿼리로 시간 구간을 찾는 태스크도 제공
  - 실내 일상 행동을 다루므로 EK100보다 넓은 household action으로 확장할 때 참고 가능
- **주의점**:
  - 1인칭 손-물체 조작보다는 3인칭 전신 행동과 실내 활동 중심
  - verb/noun 독립 head를 쓰려면 label phrase를 파싱하고 mapping을 직접 만들어야 함

---

## 9. Something-Something v2

- **제작**: TwentyBN (Goyal 외, 2017)
- **논문**: *The "Something Something" Video Database for Learning and Evaluating Visual Common Sense* (ICCV 2017)
- **Egocentric 여부**: ❌ **아니오**. 주로 테이블 위 물체 조작을 외부 카메라로 촬영한 3인칭 영상
- **규모**: 220,847 videos (2~6초) / 174 action 클래스
- **구조**: "Pushing [something] from left to right" — `[something]` 자리가 있지만 클래스화 안 됨
- **verb/noun 구조**: ✗ 템플릿 문장이 곧 레이블 — object 정체는 라벨에 없음
- **Action Anticipation 관점**:
  - 물체 자체가 아닌 **물체와의 관계·움직임 패턴** 학습에 초점
  - 사전학습 모델의 temporal reasoning 능력 평가에 자주 사용
  - ImageNet 사전학습 특징이 잘 전이되지 않는 것으로 알려져, 시간 정보 의존성 평가에 적합
  - 짧은 클립에서 미래 움직임과 상태 변화를 추론하는 능력을 보기 좋음
- **주의점**:
  - noun label이 없기 때문에 EK100식 verb/noun/action 3-head 평가로 옮기기는 어려움

---

## 10. COIN (Comprehensive Instructional Videos)

- **제작**: Tencent AI Lab (Tang 외, 2019)
- **논문**: *COIN: A Large-scale Dataset for Comprehensive Instructional Video Analysis* (CVPR 2019)
- **Egocentric 여부**: ❌ **아니오**. YouTube instructional video 기반으로, 대부분 3인칭 또는 외부 관찰자 시점
- **규모**: 11,827 videos / 46,354 segments / 180개 task / 778개 step
- **구조**: task ID + step ID — 단일 정수, verb/noun 분리 없음
- **verb/noun 구조**: ✗
- **Action Anticipation 관점**:
  - 요리, 자동차 수리, 스포츠 등 다양한 절차적 작업
  - V-JEPA2 코드베이스에 `COIN_anticipation` 분기가 별도로 존재 (`action_is_verb_noun=False`)
  - Procedural Activity Understanding의 표준 벤치마크 중 하나
  - "다음에 어떤 절차 단계가 오는가"를 맞히는 task-level anticipation에 적합
- **주의점**:
  - 세밀한 손-물체 조작보다 편집된 instructional 영상의 step 구간이 중심
  - verb/noun head를 유지하려면 step label을 별도로 parsing하거나 ontology를 새로 만들어야 함

---

## 11. Kinetics-700

- **제작**: DeepMind / Google 계열 연구진
- **Egocentric 여부**: ❌ **아니오**. 웹에서 수집한 일반 행동 영상으로, 대부분 3인칭 또는 관찰자 시점
- **규모**: 약 650K videos / 700 action classes
- **구조**: "playing guitar", "cutting watermelon" 같은 단일 action class
- **verb/noun 구조**: ✗ action label이 복합 태그이므로 verb/noun 독립 분리가 공식 제공되지 않음
- **Action Anticipation 관점**:
  - 범용 action recognition 사전학습 또는 representation 평가용으로 널리 쓰임
  - 다양한 사람 행동과 장면을 포함하므로 EK100보다 넓은 semantic coverage를 제공
  - V-JEPA 2의 frozen encoder가 일반 action을 얼마나 잘 표현하는지 확인하는 baseline으로 활용 가능
- **주의점**:
  - 클립이 action 중심으로 잘린 경우가 많아, "액션 시작 전 관찰하고 미래를 예측"하는 anticipation protocol과는 거리가 있음
  - 1인칭 손-물체 조작, object anticipation, verb/noun 분리 평가에는 적합하지 않음

---

## 12. 정리: EK100과의 핵심 차이

```
Action Anticipation + verb/noun 독립 평가를 하려면?
→ EK100, Assembly101이 가장 직접적으로 사용 가능

대규모 사전학습 데이터로 활용하려면?
→ Ego4D (3,670h) — NLP 파싱 필요하지만 양이 압도적

시각적 관계·물체 상호작용에 집중하려면?
→ HICO-DET / V-COCO (이미지 기반)

시간 추론(temporal reasoning) 능력을 평가하려면?
→ Something-Something v2
```

| 구분 | 데이터셋 |
|------|---------|
| verb/noun 독립 ID + 영상 | **EK100**, **Assembly101** |
| 대규모 egocentric | **Ego4D**, Ego-Exo4D |
| 1인칭 주방 transfer | **EGTEA Gaze+** |
| HOI (이미지) | **HICO-DET**, V-COCO |
| 절차적 행동 | **COIN**, Assembly101, Breakfast |
| 짧은 물체 조작 / temporal reasoning | **Something-Something v2** |
| 범용 action recognition | Kinetics-700, UCF-101, HMDB-51 |

### 추천 우선순위

| 우선순위 | 데이터셋 | 이유 |
|---------:|----------|------|
| 1 | **Assembly101** | EK100처럼 verb/noun ID를 분리해 다룰 수 있고, 1인칭 영상도 포함하며, 절차 순서가 뚜렷함 |
| 2 | **Ego4D** | 가장 큰 egocentric 후보이며, short-term/long-term anticipation 태스크가 이미 정의되어 있음 |
| 3 | **EGTEA Gaze+** | EK100과 같은 주방·조리 도메인이지만 규모가 작아 보조 평가나 transfer 실험에 적합 |
| 4 | **Ego-Exo4D** | 1인칭과 3인칭이 동기화되어 시점 불변 표현 및 숙련 행동 anticipation 연구에 적합 |
| 5 | **COIN / Something-Something v2** | egocentric은 아니지만 절차 예측 또는 시간 추론 능력을 따로 평가하기 좋음 |

### 결론

EK100의 대체 또는 확장 데이터셋을 찾는 목적이라면, **Assembly101**이 가장 직접적인 후보이다.  
이유는 1인칭 영상이 포함되고, verb/noun을 독립적으로 다룰 수 있으며, 조립 절차의 시간 순서가 뚜렷하기 때문이다.

대규모 egocentric pretraining이나 더 일반적인 일상 행동 anticipation을 보고 싶다면 **Ego4D**가 가장 강력하다.  
다만 EK100처럼 verb/noun head를 그대로 붙이려면 narration parsing, label normalization, ontology 구축이 필요하다.

3인칭 데이터셋들은 EK100식 action anticipation을 그대로 대체하기보다는, temporal reasoning, procedural step prediction, HOI/object interaction 같은 특정 능력을 보강하거나 비교하는 용도로 보는 것이 적절하다.
