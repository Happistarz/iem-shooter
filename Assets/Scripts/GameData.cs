using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons;

[CreateAssetMenu(menuName = "Game Data")]
public class GameData : ScriptableObject
{
    [TitleGroup("General Game Data")] 
    public float SpawnMultiplier = 1;
    public WaveParameters WaveParameters;
    
    [TitleGroup("Setup")]
    public PlayerComponent PlayerPrefab;

    [TitleGroup("Gameplay Data")] 
    public AWeaponType StartingWeapon;
    public List<EnemyData> Enemies;

    public void FindAllEnemies()
    {
        
    }
}