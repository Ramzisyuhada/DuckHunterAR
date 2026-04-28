using TMPro;
using UnityEngine;
using System.Collections;

public class DuckCounter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text CountDuck;

    public static DuckCounter Singleton;

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
        CountDuck.text = "0";

        string playerId = PlayerPrefs.GetString("Name");
        LoadScoreFromFirebase(playerId);
    }

    public void OnCountDuck(float count)
    {
        // 🔥 Format biar rapi (tanpa .0)
        CountDuck.text = count.ToString("0");
    }

    public void LoadScoreFromFirebase(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogError("PlayerID kosong!");
            return;
        }

        if (FireBase.instance == null)
        {
            Debug.LogError("Firebase instance belum ada!");
            return;
        }

        Debug.Log("Load score untuk: " + playerId);

        StartCoroutine(LoadWhenReady(playerId));
    }

    IEnumerator LoadWhenReady(string playerId)
    {
        // Tunggu Firebase siap
        yield return new WaitUntil(() => FireBase.instance != null && FireBase.instance.IsReady());

        Debug.Log("Firebase ready, ambil data...");

        FireBase.instance.GetPlayerData(playerId, (score) =>
        {
            Debug.Log("Score diterima: " + score);
            OnCountDuck(score);
        });
    }
}