using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource))]
public class AnimatedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    private AudioSource audioSource;

    [Header("Animation")]
    [SerializeField] private float pressScale = 0.9f;
    [SerializeField] private float duration = 0.1f;

    [Header("Sound")]
    [SerializeField] private AudioClip clickSound;

    void Start()
    {
        InitScale(transform.localScale);
      //  originalScale = transform.localScale;
        audioSource = GetComponent<AudioSource>();
    }
    public void InitScale(Vector3 scale)
    {
        originalScale = scale;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();

        
        PlaySound();

        transform.DOScale(originalScale * pressScale, duration)
            .SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOKill();

        transform.DOScale(originalScale, duration)
            .SetEase(Ease.OutBack);
    }

    private void PlaySound()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}