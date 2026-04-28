using UnityEngine;
using DG.Tweening;

public class SwipeTutorialAnim : MonoBehaviour
{
    [SerializeField] private RectTransform handIcon;
    [SerializeField] private RectTransform targetPoint;

    [Header("Animation")]
    [SerializeField] private float duration = 1f;
    [SerializeField] private float delay = 0.5f;

    private Vector2 startPos;

    void Start()
    {
        startPos = handIcon.anchoredPosition;

        PlaySwipeAnim();
    }

    void PlaySwipeAnim()
    {
        handIcon.anchoredPosition = startPos;

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(delay);

        // Gerak ke target
        seq.Append(handIcon.DOAnchorPos(targetPoint.anchoredPosition, duration)
            .SetEase(Ease.InOutSine));

        // Fade sedikit (opsional)
        seq.Join(handIcon.GetComponent<CanvasGroup>().DOFade(0.3f, duration));

        // Balik ke awal
        seq.Append(handIcon.DOAnchorPos(startPos, 0f));
        seq.Join(handIcon.GetComponent<CanvasGroup>().DOFade(1f, 0f));

        seq.SetLoops(-1); // loop terus
    }
}