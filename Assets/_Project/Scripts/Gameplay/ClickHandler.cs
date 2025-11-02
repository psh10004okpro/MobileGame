using UnityEngine;
using BreakInfinity;

namespace PunchKing
{
    public class ClickHandler : MonoBehaviour
    {
        [Header("클릭 대상")]
        public Transform targetObject;
        public bool useRaycast = true;

        [Header("애니메이션")]
        public float punchScale = 0.15f;
        public float punchDuration = 0.3f;
        public float shakeIntensity = 10f;

        [Header("파티클")]
        public ParticleSystem clickParticle;

        [Header("경험치 획득")]
        public bool giveExpOnClick = true;
        public BigDouble expPerClick = 1;

        private Vector3 originalScale;
        private bool isAnimating = false;

        void Start()
        {
            if (targetObject != null)
            {
                originalScale = targetObject.localScale;
            }

            // CurrencyManager 이벤트 구독
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnGoldEarned += OnGoldEarned;
            }
        }

        void Update()
        {
            // 터치 입력 처리 (모바일)
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    HandleClick(touch.position);
                }
            }

            // 마우스 입력 (에디터 테스트용)
            if (Input.GetMouseButtonDown(0))
            {
                HandleClick(Input.mousePosition);
            }
        }

        void HandleClick(Vector2 screenPos)
        {
            if (useRaycast && targetObject != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(screenPos);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.transform == targetObject || hit.collider.transform.IsChildOf(targetObject))
                    {
                        PerformClick(hit.point);
                    }
                }
            }
            else
            {
                // Raycast 없이 모든 클릭 허용
                PerformClick(screenPos);
            }
        }

        void PerformClick(Vector3 position)
        {
            if (CurrencyManager.Instance == null)
                return;

            // 데미지 계산
            bool isCrit = CurrencyManager.Instance.OnClick();

            // 경험치 획득
            if (giveExpOnClick && CharacterEvolution.Instance != null)
            {
                CharacterEvolution.Instance.GainExperience(expPerClick);
            }

            // 애니메이션
            PlayClickAnimation(isCrit);

            // 파티클
            if (clickParticle != null)
            {
                clickParticle.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, 10f));
                clickParticle.Play();
            }

            // 햅틱 (모바일)
            #if UNITY_IOS || UNITY_ANDROID
            if (isCrit)
            {
                Handheld.Vibrate();
            }
            #endif
        }

        void PlayClickAnimation(bool isCrit)
        {
            if (targetObject == null || isAnimating)
                return;

            StartCoroutine(AnimateClick(isCrit));
        }

        System.Collections.IEnumerator AnimateClick(bool isCrit)
        {
            isAnimating = true;

            float scaleMult = isCrit ? punchScale * 1.5f : punchScale;
            Vector3 targetScale = originalScale * (1f + scaleMult);

            // Scale up
            float elapsed = 0f;
            while (elapsed < punchDuration * 0.3f)
            {
                targetObject.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / (punchDuration * 0.3f));
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Scale down
            elapsed = 0f;
            while (elapsed < punchDuration * 0.7f)
            {
                targetObject.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / (punchDuration * 0.7f));
                elapsed += Time.deltaTime;
                yield return null;
            }

            targetObject.localScale = originalScale;
            isAnimating = false;
        }

        void OnGoldEarned(BigDouble amount, bool isCrit)
        {
            // UIManager에 알림 (데미지 넘버 표시)
            // UIManager가 이벤트를 구독하고 있으면 자동으로 처리됨
        }

        void OnDestroy()
        {
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnGoldEarned -= OnGoldEarned;
            }
        }
    }
}
