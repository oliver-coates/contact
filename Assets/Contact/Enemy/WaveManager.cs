using System;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Settings:")]
    [SerializeField] private float _waveDuraction;

    [Header("References:")]
    [SerializeField] private EnemySpawner _enemySpawner;

    [Header("State:")]
    [SerializeField] private int _currentWave;
    [SerializeField] private float _waveTimer;
    [SerializeField] private int _spawnedEnemiesCurrentlyAlive = 0;
    

    [Header("Wave Value Variables")]
    // Number of enemies is calculated using ax^z
    // where a is value mod, x is wave number, and z is exponent
    [SerializeField] private float _enemySpawnExponentValue;
    [SerializeField] private float _enemySpawnValueBase;

    

    private void Awake()
    {   
        _spawnedEnemiesCurrentlyAlive = 0;

        JetEnemy.OnShotDown += EnemyShotDown;
    }

    private void OnDestroy()
    {
        JetEnemy.OnShotDown -= EnemyShotDown;
    }

    private void EnemyShotDown()
    {
        _spawnedEnemiesCurrentlyAlive -= 1;
    }

    private void FixedUpdate()
    {
        if (GameManager.IsGameRunning == false)
        {
            return;
        }
    
        _waveTimer -= Time.fixedDeltaTime;

        if (_waveTimer <= 0 || _spawnedEnemiesCurrentlyAlive == 0)
        {
            _currentWave += 1;
            _waveTimer = _waveDuraction;

            int numEnemiesToSpawn = CalculateEnemiesToSpawn();
            SpawnWave(numEnemiesToSpawn);

            _spawnedEnemiesCurrentlyAlive = numEnemiesToSpawn;
        }
    }

    private void SpawnWave(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            _enemySpawner.SpawnEnemy();        
        }
    }

    private int CalculateEnemiesToSpawn()
    {
        float numToSpawnRaw = _enemySpawnValueBase * Mathf.Pow(_currentWave, _enemySpawnExponentValue);
        
        int numToSpawn = Mathf.RoundToInt(Mathf.Clamp(numToSpawnRaw, 1, 100));
        
        numToSpawn += _spawnedEnemiesCurrentlyAlive;

        return numToSpawn;        
    }
}
