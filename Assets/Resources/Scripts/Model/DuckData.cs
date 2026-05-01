using UnityEngine;
using SQLite;

public class DuckData 
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; } 
    public string NamaBebek { get; set; }

    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }

   
}
