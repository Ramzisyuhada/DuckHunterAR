using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class FireBase : MonoBehaviour
{
    public static FireBase instance;

    DatabaseReference dbReference;

    void Awake()
    {
        // Singleton aman
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    bool isFirebaseReady = false;
    void Start()
    {
        InitFirebase();

        
    }
    public void GetTopScore(string myPlayerName, System.Action<int> onGetRank)
    {
        if (dbReference == null)
        {
            Debug.LogError("Database reference null!");
            onGetRank?.Invoke(-1);
            return;
        }

        dbReference
            .Child("players")
            .OrderByChild("score")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (!task.IsCompleted || task.IsFaulted)
                {
                    Debug.LogError("Gagal ambil data: " + task.Exception);
                    onGetRank?.Invoke(-1);
                    return;
                }

                DataSnapshot snapshot = task.Result;

                if (snapshot == null || !snapshot.Exists)
                {
                    Debug.LogWarning("Data kosong!");
                    onGetRank?.Invoke(-1);
                    return;
                }

                List<(string name, int score)> playerList = new List<(string, int)>();

                foreach (DataSnapshot child in snapshot.Children)
                {
                    string name = child.Key;

                    int score = 0;
                    if (child.HasChild("score") && child.Child("score").Value != null)
                    {
                        int.TryParse(child.Child("score").Value.ToString(), out score);
                    }

                    playerList.Add((name, score));
                }

                // 🔥 Urutkan dari tertinggi → terendah
                playerList.Sort((a, b) => b.score.CompareTo(a.score));

                // 🔥 Cari rank kamu
                int myRank = -1;

                for (int i = 0; i < playerList.Count; i++)
                {
                    if (playerList[i].name == myPlayerName)
                    {
                        myRank = i + 1; // index +1 = rank
                        break;
                    }
                }

                if (myRank != -1)
                {
                    Debug.Log("Rank kamu: " + myRank);
                }
                else
                {
                    Debug.Log("Player tidak ditemukan / belum punya skor");
                }

                // 🔥 kirim hasil keluar
                onGetRank?.Invoke(myRank);
            });
    }
    public bool IsReady()
    {
        return isFirebaseReady;
    }
    void InitFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                isFirebaseReady = true;

                Debug.Log("Firebase siap");

            }
            else
            {
                Debug.LogError("Firebase gagal: " + task.Result);
            }
        });
    }
    public void UpdateScore(string noTelepon, float score)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>()
    {
        { "score", score }
    };

        dbReference.Child("players").Child(noTelepon)
            .UpdateChildrenAsync(updates);
    }


    public void SavePlayerData(string noTelepon, string nama, float score)
    {
        if (!isFirebaseReady || dbReference == null)
        {
            Debug.LogError("Database belum siap!");
            return;
        }

        Dictionary<string, object> data = new Dictionary<string, object>()
    {
        { "nama", nama },
        { "score", score }
    };

        dbReference.Child("players").Child(noTelepon)
            .SetValueAsync(data)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    Debug.Log("Data berhasil disimpan");
                else
                    Debug.LogError("Gagal simpan: " + task.Exception);
            });
    }

    public void GetPlayerData(string playerId, System.Action<float> onSuccess)
    {
        if (!isFirebaseReady || dbReference == null)
        {
            Debug.LogError("Database belum siap!");
            return;
        }

        dbReference.Child("players").Child(playerId)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    if (task.Result.Exists)
                    {
                        DataSnapshot snapshot = task.Result;

                        float score = 0;

                        if (snapshot.HasChild("score"))
                        {
                            float.TryParse(snapshot.Child("score").Value.ToString(), out score);
                        }

                        Debug.Log("Data ditemukan!");
                        Debug.Log("Player: " + playerId + " | Score: " + score);

                        // 🔥 KIRIM DATA KE DUCKCOUNTER
                        onSuccess?.Invoke(score);
                    }
                    else
                    {
                        Debug.Log("Data tidak ditemukan");
                        onSuccess?.Invoke(0);
                    }
                }
                else
                {
                    Debug.LogError("Gagal ambil data: " + task.Exception);
                }
            });
    }


}