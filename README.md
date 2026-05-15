# Action Anticipation — V-JEPA 2 with EK100

---

## 1. EK100 Verb·Noun 클래스는 어디서 정의되는가

### 원본 정의: CSV 어노테이션 파일

EK100의 Verb·Noun 클래스는 다음 CSV 파일의 `verb_class` / `noun_class` 컬럼에 숫자 ID로 내장되어 있다.

```
data/annotations/
├── EPIC_100_train.csv           (67,217 rows)
└── EPIC_100_validation.csv      (9,668 rows)
```

각 행(annotation)은 하나의 액션 구간을 나타내며, 아래 컬럼을 포함한다.

| 컬럼 | 내용 | 예시 |
|------|------|------|
| `narration_id` | 고유 클립 ID | `P01_01_0` |
| `video_id` | 영상 ID | `P01_01` |
| `start_frame` / `stop_frame` | 액션 시작·종료 프레임 번호 | `8` / `202` |
| `verb` | verb 텍스트 (복수 표기 통합) | `take` |
| `verb_class` | verb 정수 ID (0~96) | `0` |
| `noun` | noun 텍스트 | `bag` |
| `noun_class` | noun 정수 ID (0~288) | `19` |
| `narration` | 원문 내러티브 | `take bag` |

train set 기준 고유 클래스 수는 **verb 97개 / noun 289개**이다.  
`verb_class=0`은 "take/grab/pick-up" 계열 동사를 통합한 클래스이고,  
`noun_class=0`은 "tap/flow:tap" 계열 명사를 묶은 클래스이다.  
즉 복수의 유사 텍스트 표현이 하나의 정수 ID로 집계된다.

### 코드에서의 클래스 매핑 처리

실제 inference 시에는 클래스가 한 번 더 재매핑된다. `filter_annotations()` 함수  
([evals/action_anticipation_frozen/epickitchens.py](evals/action_anticipation_frozen/epickitchens.py#L232))에서 다음 과정을 거친다.

```
1. train CSV에 등장한 (verb_class, noun_class) 쌍만 유효 클래스로 추출
2. val CSV에서 이 쌍에 포함되지 않는 annotation을 제거
3. 유효한 verb/noun/action 집합에 0부터 시작하는 연속 정수 ID를 새로 부여
   verb_classes  = {orig_ek100_id: unified_id, ...}
   noun_classes  = {orig_ek100_id: unified_id, ...}
   action_classes = {(orig_verb_id, orig_noun_id): unified_id, ...}
```

따라서 모델이 예측하는 logit 인덱스는 EK100 원본 ID가 아닌 **unified ID**이며,  
CSV로 저장 시에는 역매핑(`inv_verb_classes`, `inv_noun_classes`)으로 원본 EK100 ID를 복원한다.

---

## 2. Action Anticipation 원리 (V-JEPA 2 논문 기반)

### 2-1. 전체 구조

V-JEPA 2는 두 개의 주요 모듈로 구성된다.

```
[Observed Video Clip]
        │
        ▼
  ┌───────────┐
  │  Encoder  │  ViT-L/16 — context token 생성
  │  (frozen) │  patch size 16, tubelet size 2
  └───────────┘
        │  context tokens [B, N, D]
        ▼
  ┌───────────┐
  │ Predictor │  ViT-based — 미래 프레임 token 예측
  │  (frozen) │  anticipation_time만큼 앞의 패치 표현 추정
  └───────────┘
        │  predicted future tokens [B, M, D]
        ▼
  ┌─────────────────┐
  │ Attentive Probe │  학습 가능한 경량 분류기
  │  (trainable)    │  Verb head + Noun head + Action head
  └─────────────────┘
        │
        ▼
  [Verb logits | Noun logits | Action logits]
```

### 2-2. Encoder: 현재 영상을 공간·시간 토큰으로 변환

입력 비디오 클립 `[B, 3, T, H, W]`을 `patch_size=16`, `tubelet_size=2`의 3D patch로 분할하여  
각 patch를 1024-dim 토큰으로 임베딩한다. 출력은 `[B, N, 1024]`이다.  
(T=32 frames, H=W=256px → N ≈ 8×8×16 = 1024 tokens)

### 2-3. Predictor: 미래 시점의 표현을 예측

Predictor는 encoder token에서 **아직 보이지 않는 미래 프레임의 표현**을 예측한다.  
`anticipation_time` (초 단위)을 조건으로 받아, 그만큼 미래의 특정 위치 patch token을 추정한다.

```python
# AnticipativeWrapper.forward() — vit_encoder_predictor_concat_ar.py
x_full = self.encoder(x)                  # 현재 프레임 encode
...
predicted = self.predictor(x, ctxt_positions, tgt_positions, ...)  # 미래 위치 예측
```

이 예측은 JEPA(Joint Embedding Predictive Architecture) 방식으로 학습된 것이다:  
pixel reconstruction 대신 **잠재 표현 공간에서의 예측**이므로 의미 있는 고수준 표현을 학습한다.

### 2-4. Attentive Probe: 예측된 표현을 분류

Predictor가 출력한 future token에 Attentive Probe(multi-head cross-attention pooling + linear)를 붙여  
verb / noun / action 각각의 logit을 계산한다. Encoder와 Predictor는 **frozen** 상태이며, probe만 학습된다.

```
Attentive Probe 구조 (num_probe_blocks=4, num_heads=16):
  Multi-head Attention → LayerNorm → ... (4 blocks)
  → Linear(D, |verb_classes|)    → verb logits
  → Linear(D, |noun_classes|)    → noun logits
  → Linear(D, |action_classes|)  → action logits
```

### 2-5. 데이터 샘플링 방식

각 클립은 다음과 같이 구성된다.

```
───────────────────────────────────────────────────────────────────▶ time
              [  입력 클립 (32 frames @ 8fps = 4초)  ]
                                                  │
                                             anticipation_point
                                             (start~stop 사이 샘플)
                                                  ├── anticipation_time: 1.0s
                                                  │
                                                  ▼
                                            [ACTION START]
```

`anticipation_time=1.0s`는 클립 마지막 프레임이 액션 시작 1초 전에 끝남을 의미한다.  
`anticipation_point=[0.0, 0.0]`(val)은 항상 액션 시작 직전을 관찰 끝점으로 고정한다.

### 2-6. 평가 지표: Class-Mean Recall@5

EK100 공식 지표는 **Class-Mean Recall@5**이다.  
단순 accuracy(예측 top-1 정답률)와 달리, 각 클래스별로 recall@5를 계산한 뒤 평균을 낸다.

```
Recall@5 = (class별로) top-5 예측 중 정답이 있는 샘플 수 / 해당 클래스 전체 샘플 수
Class-Mean Recall@5 = 위를 모든 클래스에 대해 평균
```

이 지표는 자주 등장하는 클래스(예: take)에 편중되지 않고, 드문 클래스도 균등하게 반영한다.

---

## 3. predictions.csv 컬럼 설명

파일 위치: `evals-output/action_anticipation_frozen/ek100-vitl16/predictions.csv`  
행 수: **704 rows** (44개 비디오 서브셋의 val annotation 중 유효한 샘플)

### 3-1. 식별 정보

| 컬럼 | 타입 | 설명 | 예시 |
|------|------|------|------|
| `narration_id` | str | 클립 고유 ID (`{video_id}_{순번}`) | `P01_13_0` |
| `start_frame` | int | 어노테이션된 액션 시작 프레임 번호 | `36` |
| `stop_frame` | int | 어노테이션된 액션 종료 프레임 번호 | `105` |
| `anticipation_time_sec` | float | 관찰 종료 ~ 액션 시작 사이 간격(초) | `1.0` |

> `narration_id`의 순번은 해당 비디오 내에서 `start_frame` 기준 정렬된 인덱스다.

### 3-2. 정답 (Ground Truth)

| 컬럼 | 타입 | 설명 | 예시 |
|------|------|------|------|
| `true_verb_id` | int | EK100 원본 verb_class ID | `0` |
| `true_verb` | str | verb 텍스트 | `take` |
| `true_noun_id` | int | EK100 원본 noun_class ID | `19` |
| `true_noun` | str | noun 텍스트 | `bag` |

### 3-3. 모델 예측 (Prediction)

| 컬럼 | 타입 | 설명 | 예시 |
|------|------|------|------|
| `pred_verb_top1` | str | 가장 높은 확률의 예측 verb | `take` |
| `pred_verb_top5` | str | 상위 5개 예측 verb (`\|` 구분, 확률 내림차순) | `take\|fold\|put\|put-in\|pour-in` |
| `pred_noun_top1` | str | 가장 높은 확률의 예측 noun | `cup` |
| `pred_noun_top5` | str | 상위 5개 예측 noun (`\|` 구분) | `cup\|dish\|bag\|milk\|bowl` |

> verb와 noun은 **독립적으로** 예측된다. Action(verb+noun 조합) top-5는 별도 head에서 나오며 CSV에는 포함하지 않았다.

### 3-4. 정오 판별 플래그

| 컬럼 | 타입 | 설명 | 예시 |
|------|------|------|------|
| `verb_correct_top1` | int (0/1) | top-1 예측이 정답 verb와 일치 | `1` |
| `noun_correct_top1` | int (0/1) | top-1 예측이 정답 noun과 일치 | `0` |
| `verb_correct_top5` | int (0/1) | top-5 예측 안에 정답 verb 포함 | `1` |
| `noun_correct_top5` | int (0/1) | top-5 예측 안에 정답 noun 포함 | `1` |

> `verb_correct_top1=1`, `noun_correct_top1=0`이면 행동 유형(verb)은 맞혔지만 대상 물체(noun)는 틀린 경우다.

### 3-5. 이번 실행 결과 요약

| 지표 | Verb | Noun | Action (둘 다 top-1 정답) |
|------|:----:|:----:|:------------------------:|
| **Top-1 Accuracy** | 54.5% | 47.9% | 30.8% |
| **Top-5 Accuracy** | 90.1% | 76.1% | — |
| **Class-Mean Recall@5** | 64.2% | 57.3% | 44.4% |

> Top-1 Accuracy와 Class-Mean Recall@5의 차이가 큰 이유:  
> Accuracy는 등장 빈도가 높은 클래스(take, put 등)가 결과를 지배하고,  
> Class-Mean Recall@5는 드문 클래스도 동등한 가중치로 평균하므로 서로 다른 수치가 나온다.

### 3-6. 예시 행 해석

```
narration_id  : P01_13_0
start_frame   : 36   stop_frame: 105
anticipation  : 1.0s

true          : take bag
pred_verb_top1: take         (✓ 맞음)
pred_noun_top1: cup          (✗ 틀림)
pred_noun_top5: cup|dish|bag|milk|bowl   → bag은 3번째 → top-5 정답
→ verb_correct_top1=1, noun_correct_top1=0, verb_correct_top5=1, noun_correct_top5=1
```

액션 시작 1초 전까지의 영상만 보고 "bag을 집으려 한다"는 것을 완전히 맞히지는 못했지만,  
top-5 후보 안에는 bag이 포함되어 있다.
