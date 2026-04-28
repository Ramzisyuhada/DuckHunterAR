using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UICloudTween : MonoBehaviour
{
    public List<RectTransform> clouds;

    void Start()
    {
        foreach (RectTransform cloud in clouds)
        {
            SetupCloud(cloud);
        }
    }

    void SetupCloud(RectTransform cloud)
    {
        Vector2 startPos = cloud.anchoredPosition;

        float screenWidth = Screen.width;
        float moveDistance = screenWidth * 2f; // 🔥 responsive

        float duration = Random.Range(15f, 30f);
        float direction = Random.value > 0.5f ? 1f : -1f; // kanan / kiri

        float targetX = startPos.x + (moveDistance * direction);

        cloud.DOAnchorPosX(targetX, duration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }
}
