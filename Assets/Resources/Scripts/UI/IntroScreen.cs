using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class IntroScreen : MonoBehaviour
{

    public static IntroScreen Singleton;

    [Header("Text")]
    public TextMeshProUGUI textHead;
    public TextMeshProUGUI textTap;
    public TextMeshProUGUI TFNama;

    public TMP_InputField inputnama;

    [Header("Images")]
    public Image logo;
    public Image background;

    [Header("UI Pop List")]
    public List<Transform> uiPopObjects;

    [Header("Audio")]
    public AudioClip popSound;

    private AudioSource audioSource;

    private Dictionary<Transform, Vector3> originalScales = new Dictionary<Transform, Vector3>();

    [Header("Settings")]
    public float delayBeforeText2 = 1.5f;
    public float fadeDuration = 0.5f;
    public float popDuration = 0.4f;
    public float delayBetweenPop = 0.2f;

    private Sequence introSequence;
    private bool isTapped = false;
    private CanvasGroup logoGroup;

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetHeadName(string name)
    {
        TFNama.text = "Hallo " + name;
        inputnama.gameObject.SetActive(false);
    }


    void Start()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Name"))) SetHeadName(PlayerPrefs.GetString("Name"));

        audioSource = GetComponent<AudioSource>();

        textHead.alpha = 0;
        textTap.alpha = 0;

        // ambil CanvasGroup
        logoGroup = logo.GetComponent<CanvasGroup>();
        logoGroup.alpha = 0;

        // init UI scale
        foreach (var ui in uiPopObjects)
        {
            if (ui == null) continue;

            originalScales[ui] = ui.localScale;
            ui.localScale = Vector3.zero;
        }

        // intro text
        introSequence = DOTween.Sequence();

        introSequence.Append(textHead.DOFade(1, 0.8f))
                     .AppendInterval(delayBeforeText2)
                     .Append(textTap.DOFade(1, 0.8f))
                     .Join(logoGroup.DOFade(1, 0.8f)); // ✅ bareng textTap
    }

    void Update()
    {
        if (isTapped) return;

        if (Touchscreen.current != null)
        {
            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
                HandleTap();
        }
        else if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
                HandleTap();
        }
    }

    void HandleTap()
    {
        isTapped = true;

        if (introSequence != null && introSequence.IsActive())
            introSequence.Kill();

        Sequence tapSeq = DOTween.Sequence();

        tapSeq.Append(textHead.DOFade(0, 0.3f))
              .Append(textTap.DOFade(0, 0.3f))

              .AppendCallback(() =>
              {
                  if (logo != null)
                      logo.DOFade(0, 0.4f).SetEase(Ease.OutQuad);

                  if (background != null)
                      background.DOFade(0, 0.4f).SetEase(Ease.OutQuad);
              })

              .AppendInterval(0.2f)

              .AppendCallback(() =>
              {
                  PopUI();

                  if (background != null && background.transform.parent != null)
                      background.transform.parent.gameObject.SetActive(false);
              });
    }

    void PopUI()
    {
        for (int i = 0; i < uiPopObjects.Count; i++)
        {
            Transform ui = uiPopObjects[i];
            if (ui == null) continue;

            var btn = ui.GetComponent<AnimatedButton>();
            if (btn != null)
            {
                btn.InitScale(originalScales[ui]);
            }

            ui.DOScale(originalScales[ui], popDuration)
              .SetEase(Ease.OutBack)
              .SetDelay(i * delayBetweenPop)
              .OnStart(() =>
              {
                  if (popSound != null && audioSource != null)
                  {
                      audioSource.pitch = Random.Range(0.95f, 1.05f);
                      audioSource.PlayOneShot(popSound);
                  }
              });
        }
    }
}