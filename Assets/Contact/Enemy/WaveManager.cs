using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;

public class WaveManager : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();
    public int currWave;
    public EnemySpawner enemySpawner;
    public List<GameObject> enemiesToSpawn = new List<GameObject>();
    public int waveDuration;
    private float waveTimer;
    private float spawnInterval;
    private float spawnTimer;
    

    //public List<GameObject> spawnedEnemies = new List<GameObject>();
    public int spawnedEnemies = 0;
    
    [Header("Wave Value Variables")]
    // Number of enemies is calculated using ax^z
    // where a is value mod, x is wave number, and z is exponent
    [Header("  Write")]
    [SerializeField] private float exponent;
    [SerializeField] private float valueMod;

    [Header("  Read")]
    [SerializeField] private int waveValue;
    [SerializeField] private float preValue;
    [SerializeField] private int flooredValue;
    

    void Start()
    {
        GenerateWave();
    }

    void FixedUpdate()
    {
        if (spawnTimer <= 0)
        {
            _SpawnEnemy();
        }
        else
        {
            spawnTimer -= Time.fixedDeltaTime;
            waveTimer -= Time.fixedDeltaTime;
        }

        if (waveTimer <= 0 && spawnedEnemies <= 0)
        {
            currWave ++;
            GenerateWave();
        }
    }

    private void _SpawnEnemy()
    {
        if (enemiesToSpawn.Count > 0)
        {
            //enemySpawner.SpawnEnemy();
            GameObject enemy = Instantiate(enemiesToSpawn[0], enemySpawner.RandomDonutPosition(), Quaternion.identity);
            enemiesToSpawn.RemoveAt(0);
            spawnedEnemies ++;
            spawnTimer = spawnInterval;
        }
        else
        {
            waveTimer = 0;
        }
    }

    public void GenerateWave()
    {
        waveValue = CalculateWaveValue();

        waveValue = Mathf.FloorToInt(Mathf.Pow(currWave, exponent));
        GenerateEnemies();

        if (enemiesToSpawn.Count > 0)
        {
            spawnInterval = waveDuration / Mathf.Max(1, enemiesToSpawn.Count);
        }

        waveTimer = waveDuration;
    }

    public int CalculateWaveValue()
    {
        preValue = valueMod * Mathf.Pow(currWave, exponent);
        flooredValue = Mathf.FloorToInt(preValue);
        return flooredValue;
        
    }

    public void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();
        while (waveValue > 0 && generatedEnemies.Count < 50)
        {
            if (waveValue >= 1)
            {
                generatedEnemies.Add(enemies[0].enemyPrefab);
                waveValue -=  enemies[0].cost;
            }
        }

        enemiesToSpawn.Clear();
        enemiesToSpawn.AddRange(generatedEnemies);

    }
}

[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    public int cost;
}
