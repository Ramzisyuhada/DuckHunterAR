using SQLite;
using System.IO;
using UnityEngine;

public class DBConnection 
{
    public SQLiteConnection db;

    public void Connection()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, "MyDb.db");

        db = new SQLiteConnection(dbPath);

        db.CreateTable<DuckData>();
    }

    
    public void Disconnect()
    {
        db.Close();
    }
}
