using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Waves/Parameters")]
public class WaveParameters : ScriptableObject
{
    [Serializable]
    public class WavePart
    {
        public                 EnemyData.ThreatLevel Threat;
        [Range(0, 100)] public int                   Percentage;
    }

    [Serializable]
    public class Wave
    {
        public string Name;
        public float  Duration;
        public int TotalEnemies;
        public List<WavePart> Parts = new();
        public BossReactionData BossReaction;
    }
    
    public List<Wave> Waves;
}