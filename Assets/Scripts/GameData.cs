using System.Collections.Generic;
using UnityEngine;
using Weapons;

[CreateAssetMenu(menuName = "Game Data")]
public class GameData : ScriptableObject
{
    public float GridExtent = 500;
    public int GridCellCount = 20;

    public WaveParameters WaveParameters;
    
    public PlayerComponent PlayerPrefab;

    public WeaponData StartingWeapon;
    public List<EnemyData> Enemies;
    
    public BulletComponent BulletPrefab;

    public void FindAllEnemies()
    {
        
    }
}