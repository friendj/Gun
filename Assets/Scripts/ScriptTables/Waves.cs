using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "WaveData/Create Data")]
public class Waves : ScriptableObject
{
    [System.Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCount;
        public int timeBetweenSpawns;
        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;
    }

    public Wave[] waves;

    public Wave GetWave(int index)
    {
        return waves[Mathf.Clamp(index, 0, waves.Length - 1)];
    }
}
