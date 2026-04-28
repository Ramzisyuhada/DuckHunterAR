using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

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

    public void SavePlayerData(string playerId, float score)
    {
        if (!isFirebaseReady || dbReference == null)
        {
            Debug.LogError("Database belum siap!");
            return;
        }

        dbReference.Child("players").Child(playerId).Child("score")
            .SetValueAsync(score)
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