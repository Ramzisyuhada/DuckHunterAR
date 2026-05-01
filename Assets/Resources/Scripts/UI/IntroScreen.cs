using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[RequireComponent(typeof(AudioSource))]
public class IntroScreen : MonoBehaviour
{
    public static IntroScreen Singleton;

    [Header("Text")]
    public TextMeshProUGUI textHead;
    public TextMeshProUGUI textTap;
    public TextMeshProUGUI TFNama;

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
    public float popDuration = 0.4f;
    public float delayBetweenPop = 0.2f;

    private Sequence introSequence;
    private bool isTapped = false;
    private CanvasGroup logoGroup;

    [Header("Identitas")]
    public GameObject ParentDataUser;

    public TMP_InputField inputnama;
    public TMP_InputField inputTelepon;
    public TMP_InputField inputemail;

    public TMP_Text TF_Nama;
    public TMP_Text TF_No;
    public TMP_Text TF_Email;

    public RectTransform namaTransform;
    public RectTransform noTransform;
    public RectTransform emailTransform;

    

    [Header("Anim Parent Data User")]
    public CanvasGroup parentCanvasGroup;
    public float parentAnimDuration = 0.35f;
    public Vector3 parentStartScale = new Vector3(0.8f, 0.8f, 0.8f);

    private Color normalColor;
    private Color errorColor = Color.red;
    private string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    int index = 0;

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

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        normalColor = TF_Nama.color;

        textHead.alpha = 0;
        textTap.alpha = 0;

        if (logo != null)
        {
            logoGroup = logo.GetComponent<CanvasGroup>();
            if (logoGroup == null)
                logoGroup = logo.gameObject.AddComponent<CanvasGroup>();

            logoGroup.alpha = 0;
        }

        // ParentDataUser init
        if (ParentDataUser != null)
            ParentDataUser.SetActive(false);

        if (parentCanvasGroup != null)
        {
            parentCanvasGroup.alpha = 0;
            parentCanvasGroup.transform.localScale = parentStartScale;
        }
        else if (ParentDataUser != null)
        {
            var cg = ParentDataUser.GetComponent<CanvasGroup>();
            if (cg != null)
                cg.alpha = 0;

            ParentDataUser.transform.localScale = parentStartScale;
        }

        // Init UI pop objects
        foreach (var ui in uiPopObjects)
        {
            if (ui == null) continue;

            originalScales[ui] = ui.localScale;
            ui.localScale = Vector3.zero;

            CanvasGroup cg = ui.GetComponent<CanvasGroup>();
            if (cg == null) cg = ui.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0;
        }

        // Intro sequence
        introSequence = DOTween.Sequence();

        if (logoGroup != null)
        {
            introSequence.Append(textHead.DOFade(1, 0.8f))
                         .AppendInterval(delayBeforeText2)
                         .Append(textTap.DOFade(1, 0.8f))
                         .Join(logoGroup.DOFade(1, 0.8f));
        }
        else
        {
            introSequence.Append(textHead.DOFade(1, 0.8f))
                         .AppendInterval(delayBeforeText2)
                         .Append(textTap.DOFade(1, 0.8f));
        }
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
    void HideIntro()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(textHead.DOFade(0, 0.3f))
           .Join(textTap.DOFade(0, 0.3f));

        if (logo != null)
            seq.Join(logo.DOFade(0, 0.4f));

        seq.OnComplete(() =>
        {
            textHead.gameObject.SetActive(false);
            textTap.gameObject.SetActive(false);

            if (logo != null)
                logo.gameObject.SetActive(false);
        });
    }
    void HandleTap()
    {
        index++;

        // Tap pertama -> munculin ParentDataUser
        if (index == 1)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString("ID")))
            {
                if (background != null)
                    background.DOFade(0, 0.4f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            HideIntro();

                            background.transform.parent.gameObject.SetActive(false);
                            PopUI();
                        });

                //PlayerPrefs.SetString("ID", "");
                //PlayerPrefs.Save();

                return;

            }

            ShowParentDataUser();
            HideIntro();
    

        }
    }

    void ShowParentDataUser()
    {
        if (ParentDataUser == null) return;

        ParentDataUser.SetActive(true);

        CanvasGroup cg = parentCanvasGroup != null ? parentCanvasGroup : ParentDataUser.GetComponent<CanvasGroup>();
        if (cg == null) cg = ParentDataUser.AddComponent<CanvasGroup>();

        cg.alpha = 0;
        ParentDataUser.transform.localScale = parentStartScale;

        cg.DOFade(1, parentAnimDuration);
        ParentDataUser.transform
            .DOScale(Vector3.one, parentAnimDuration)
            .SetEase(Ease.OutBack);
    }

    void HideParentDataUser(System.Action onComplete = null)
    {
        if (ParentDataUser == null)
        {
            onComplete?.Invoke();
            return;
        }

        CanvasGroup cg = parentCanvasGroup != null ? parentCanvasGroup : ParentDataUser.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            ParentDataUser.SetActive(false);
            onComplete?.Invoke();
            return;
        }

        Sequence seq = DOTween.Sequence();
        seq.Append(cg.DOFade(0, parentAnimDuration))
           .Join(
               ParentDataUser.transform
                   .DOScale(parentStartScale, parentAnimDuration)
                   .SetEase(Ease.InBack)
           )
           .OnComplete(() =>
           {
               ParentDataUser.SetActive(false);
               onComplete?.Invoke();
           });
    }

    // Button submit -> hubungkan ke function ini
    public void OnInputDataUser()
    {
        string nama = inputnama != null ? inputnama.text : "";
        string no = inputTelepon != null ? inputTelepon.text : "";
        string email = inputemail != null ? inputemail.text : "";

        bool isValid = true;

        ResetColor();

        if (string.IsNullOrEmpty(nama))
        {
            SetError(TF_Nama, namaTransform);
            isValid = false;
        }

        if (string.IsNullOrEmpty(no) || !Regex.IsMatch(no, @"^\d+$"))
        {
            SetError(TF_No, noTransform);
            isValid = false;
        }

        if (string.IsNullOrEmpty(email) || !Regex.IsMatch(email, emailPattern))
        {
            SetError(TF_Email, emailTransform);
            isValid = false;
        }

        if (!isValid)
        {
            Debug.Log("❌ Data belum valid");
            return;
        }

        Debug.Log("✅ Valid");

        index = 2;

        if (index == 2)
        {
            TFNama.text = nama;

            // ✅ FIX DI SINI
            if (string.IsNullOrEmpty(PlayerPrefs.GetString("ID")))
            {
                PlayerPrefs.SetString("ID", no);
                PlayerPrefs.Save();
            }

            // ✅ FIX DI SINI
            FireBase.instance.SavePlayerData(no, nama, 0f);

            HideParentDataUser(() =>
            {
                StartMainSequence();
            });
        }
    }

    void ResetColor()
    {
        if (TF_Nama != null) TF_Nama.color = normalColor;
        if (TF_No != null) TF_No.color = normalColor;
        if (TF_Email != null) TF_Email.color = normalColor;
    }

    void SetError(TMP_Text text, RectTransform target)
    {
        if (text != null)
            text.color = errorColor;

        if (target == null) return;

        target.DOKill();
        target.DOShakeAnchorPos(
            0.4f,
            new Vector2(20f, 0f),
            20,
            90,
            false,
            true
        );
    }

    void StartMainSequence()
    {
        if (isTapped) return;
        isTapped = true;

        if (introSequence != null && introSequence.IsActive())
            introSequence.Kill();

        Sequence seq = DOTween.Sequence();

        seq.Append(textHead.DOFade(0, 0.3f))
           .Append(textTap.DOFade(0, 0.3f));

        if (logo != null)
            seq.Join(logo.DOFade(0, 0.4f).SetEase(Ease.OutQuad));

        if (background != null)
            seq.Join(background.DOFade(0, 0.4f).SetEase(Ease.OutQuad));

        seq.AppendInterval(0.2f)
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

            CanvasGroup cg = ui.GetComponent<CanvasGroup>();
            if (cg == null) cg = ui.gameObject.AddComponent<CanvasGroup>();

            Sequence popSeq = DOTween.Sequence();

            popSeq.AppendInterval(i * delayBetweenPop)

                  // 🔊 Audio dipanggil DI SINI (sinkron)
                  .AppendCallback(() =>
                  {
                      if (popSound != null && audioSource != null)
                      {
                          audioSource.pitch = Random.Range(0.95f, 1.05f);
                          audioSource.PlayOneShot(popSound);
                      }
                  })

                  // 🎬 Animasi mulai setelah audio trigger
                  .Append(ui.DOScale(originalScales[ui], popDuration)
                      .SetEase(Ease.OutBack))

                  .Join(cg.DOFade(1, popDuration));
        }
    }
}