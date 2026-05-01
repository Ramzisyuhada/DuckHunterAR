using UnityEngine;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Refrences")]

    [SerializeField] private GameObject ValidasiTutorial;

    [SerializeField] private TMP_Text TF_Ammo;

    [SerializeField] private RectTransform ParentTFAmmo;


    public TMP_Text TF_Rank;

    public GameObject Tutorial;
    private CanvasGroup canvasGroup;

    public void SetTextAmmo(float ammo)
    {
        TF_Ammo.text = ammo.ToString();
    }

    public void PlayEmptyAnimation()
    {
        ParentTFAmmo.DOKill();

        ParentTFAmmo.DOShakeAnchorPos(0.5f, new Vector2(20, 0), 20, 90);

        TF_Ammo.DOColor(Color.red, 0.2f)
            .SetLoops(4, LoopType.Yoyo)
            .OnComplete(() =>
            {
                TF_Ammo.color = Color.black;
            });

        ParentTFAmmo.DOScale(1.5f, 0.2f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }



    public void PlayDecreaseAnimation()
    {
        ParentTFAmmo.DOKill();

        ParentTFAmmo.localScale = Vector3.one;

        ParentTFAmmo
            .DOScale(1.3f, 0.15f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                ParentTFAmmo.DOScale(1f, 0.1f);
            });

        ParentTFAmmo.DOPunchAnchorPos(new Vector2(10f, 0), 0.2f, 10, 1);
    }

    public void SetRankTF(int value)
    {
        Debug.Log("Kamu Rank " + value);
        TF_Rank.text = value.ToString();
    }
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Update()
    {
        
    }

    void Start()
    {
        FireBase.instance.GetTopScore(PlayerPrefs.GetString("Name"), (rank) =>
        {
            if (rank != -1)
            {
                UIManager.Instance.SetRankTF(rank);
            }
          
        });
        // Ambil CanvasGroup
        canvasGroup = ValidasiTutorial.GetComponent<CanvasGroup>();

        ValidasiTutorial.transform.localScale = Vector3.zero;
        ValidasiTutorial.SetActive(false);

        // Pastikan awal tidak blok input
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;
        }
    }

    public void ShowValidasi()
    {
        ValidasiTutorial.SetActive(true);

        if (canvasGroup != null)
        {
            // Aktifkan blok semua input
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

            // Fade in
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, 0.3f);
        }

        ValidasiTutorial.transform.localScale = Vector3.zero;

        ValidasiTutorial.transform.DOScale(Vector3.one, 0.4f)
            .SetEase(Ease.OutBack);
    }

    public void HideValidasi()
    {
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0, 0.2f);
        }

        ValidasiTutorial.transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                // Matikan blok input
                if (canvasGroup != null)
                {
                    canvasGroup.blocksRaycasts = false;
                    canvasGroup.interactable = false;
                }

                ValidasiTutorial.SetActive(false);
            });
    }
}