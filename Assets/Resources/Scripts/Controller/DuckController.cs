using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DuckController
{
    DBConnection db;


    public DuckController(DBConnection db)
    {
        this.db = db;
    }
    public void InsertDuck(DuckData data)
    {
        db.db.Insert(data);
        Debug.Log($"Player new ID: {data.Id}");

    }

    public List<DuckData> GetAllDataFull()
    {
        return db.db.Table<DuckData>().ToList();
    }
    public void DeleteByNama(string namaBebek)
    {
        var ducks = db.db.Table<DuckData>()
                         .Where(d => d.NamaBebek == namaBebek)
                         .ToList();

        foreach (var d in ducks)
        {
            db.db.Delete(d);
        }
    }

    public void DeleteById(int id)
    {
        db.db.Execute("DELETE FROM DuckData WHERE Id = ?", id);
    }
}