using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Waves/Parameters")]
public class WaveParameters : ScriptableObject
{
    [Serializable]
    public class WavePart
    {
        public EnemyData.ThreatLevel Threat;

        [Range(0, 100)] public int Percentage;
    }

    [Serializable]
    public class Wave
    {
        public string Name;
        public float  Duration;
        public int    TotalEnemies;

        [Header("Beam Settings")]
        [Range(0, 100)] public int MoveBeamChance = 0;

        [Tooltip("-1 for random, otherwise specific beam index")]
        public int BeamIndex = -1;

        public List<WavePart>   Parts = new();
        public BossReactionData BossReaction;
    }

    public List<Wave> Waves;
}