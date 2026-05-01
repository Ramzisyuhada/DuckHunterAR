using MultiSet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public enum ModeGame
{
    Tutorial,
    PlayGame,
    None
}

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    private List<GameObject> posisiSpawn;

    [Header("References")]
    [SerializeField] private GameObject spawnDuck;
    [SerializeField] private Transform parent;
    [SerializeField] private Transform player; // cache player (lebih efisien)
    [SerializeField] private Transform mapSpace;

    [SerializeField] private SingleFrameLocalizationManager localizationManager;
    [Header("Options")]
    [SerializeField] private int jumlahSpawnBebekTutorial = 2;
    [SerializeField] private int jumlahSpawnBebek = 7;

    public float jumlahMenangkap;
    public ModeGame modegame = ModeGame.None;
    public DuckController Controller;
    public bool isTutorial = false; // false, state game masih memunculkan 30 bebek
                                    // true, state game ke tutorial, 3 bebek
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

    private void Start()
    {
        Controller = new DuckController(SQLiteConnect.Singleton.dbconnect);
        

        
    }

    public void UbahEnumKeTutorial(bool _isTutorial)
    {
        modegame = ModeGame.PlayGame;

        if (_isTutorial) modegame = ModeGame.Tutorial;
    }



    public void SetScore(int value)
    {
        switch (modegame)
        {
            case ModeGame.PlayGame:
                jumlahMenangkap += value;
                break;
            default:
                break;

        }
    }

    public void StartGame()
    {
        //if(isTutorial)
        //{
        //    Tutorial();
        //}
        //else
        //{
        //    MainGame();
        //}

        switch (modegame)
        {
            case ModeGame.Tutorial:
                Tutorial();
                break;

            case ModeGame.PlayGame:
                MainGame();
                break;
        }
    }


    public void Tutorial()
    {
        if (player == null || mapSpace == null)
        {
            Debug.LogError("Player atau MapSpace belum di-assign!");
            return;
        }

        // ambil arah player tapi flatten (biar tidak miring ke atas/bawah)
        Vector3 forward = player.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = player.right;
        right.y = 0;
        right.Normalize();

        for (int i = 0; i < jumlahSpawnBebekTutorial; i++)
        {
            // base posisi dari player (biar di depan user)
            Vector3 spawnPosition = player.position;

            // depan
            spawnPosition += forward * Random.Range(1.5f, 2f);

            // agak ke kanan
            spawnPosition += right * Random.Range(0.5f, 1f);

            // hardcode tinggi (sesuai request kamu)
            spawnPosition.y = 0 ;

            Instantiate(spawnDuck, spawnPosition, Quaternion.identity, parent);
        }
    }


    public void MainGame()
    {
        modegame = ModeGame.PlayGame;
        UIManager.Instance.Tutorial.SetActive(false);

        // 🔥 Cleanup object lama
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        var dbData = Controller.GetAllDataFull();

        // =========================
        // 🟢 LOAD DARI DATABASE
        // =========================
        if (dbData.Count > 0)
        {
            foreach (var data in dbData)
            {
                GameObject obj = Instantiate(
                    spawnDuck,
                    new Vector3(data.PosX, data.PosY, data.PosZ),
                    Quaternion.identity,
                    parent
                );

                // 🔥 SIMPAN ID DI NAMA
                obj.name = data.Id.ToString();
            }

            return;
        }

        // =========================
        // 🔵 GENERATE BARU
        // =========================
        GameObject[] spawnObjects = GameObject.FindGameObjectsWithTag("Spawn");

        if (spawnObjects.Length == 0)
        {
            Debug.LogError("Tidak ada object dengan tag 'Spawn'!");
            return;
        }

        List<GameObject> posisiSpawn = new List<GameObject>(spawnObjects);

        int jumlahSpawn = Mathf.Min(jumlahSpawnBebek, posisiSpawn.Count);

        for (int i = 0; i < jumlahSpawn; i++)
        {
            int randomIndex = Random.Range(0, posisiSpawn.Count);

            Vector3 pos = posisiSpawn[randomIndex].transform.position;
            pos.y = 0;

            posisiSpawn.RemoveAt(randomIndex);

            GameObject obj = Instantiate(spawnDuck, pos, Quaternion.identity, parent);

            DuckData duckdata = new DuckData
            {
                NamaBebek = "Duck", // bebas
                PosX = pos.x,
                PosY = pos.y,
                PosZ = pos.z
            };

            Controller.InsertDuck(duckdata);

            obj.name = duckdata.Id.ToString();
        }
    }


    private void Update()
    {
       
    }


    public void TutorialGame()
    {
        GameObject[] ducks = GameObject.FindGameObjectsWithTag("Duck");
        if (ducks.Length == 0)
        {
            
            UIManager.Instance.ShowValidasi();
        }
    }
}