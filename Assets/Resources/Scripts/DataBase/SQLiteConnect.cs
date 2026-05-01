using UnityEngine;

public class SQLiteConnect : MonoBehaviour
{

    public static SQLiteConnect Singleton;

    public DBConnection dbconnect;

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
        dbconnect = new DBConnection();
        dbconnect.Connection();
        
    }

    private void OnApplicationQuit()
    {
        dbconnect.Disconnect();
    }

}
