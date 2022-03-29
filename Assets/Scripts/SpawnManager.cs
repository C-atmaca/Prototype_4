using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject[] powerupPrefabs;
    public GameObject bossPrefab;
    public GameObject minion;

    private GameObject boss;
    private float spawnRange = 9.0f;
    private int enemyCount;
    private int waveNumber = 1;
    private bool bossFight = false;
    private float nextSpawn = 2.0f;
    private float spawnTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemyWave(waveNumber);
        SpawnPowerUp();
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;

        // If all enemies are gone spawn new enemies and powerups
        if (enemyCount == 0)
        {
            bossFight = false;
            waveNumber++;
            SpawnBossWave(waveNumber);
            if (!bossFight)
            {
                SpawnEnemyWave(waveNumber);
            }
            SpawnPowerUp();
        }

        if (bossFight)
        {
            Debug.Log("In boss fight");
            if (Time.time - spawnTime > nextSpawn)
            {
                Debug.Log("Spawn Minions");
                spawnTime = Time.time;
                SpawnMinions();
            }
        }
    }

    private void SpawnEnemyWave(int enemiesToSpawn)
    {
        // Spawn enemies 
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int index = Random.Range(0, enemyPrefabs.Length);
            Instantiate(enemyPrefabs[index], GenerateSpawnPosition(), enemyPrefabs[index].transform.rotation);
        }
    }

    private void SpawnPowerUp()
    {
        // Pick a random powerup and spawn it
        int randomPowerup = Random.Range(0, powerupPrefabs.Length);
        Instantiate(powerupPrefabs[randomPowerup], GenerateSpawnPosition(), powerupPrefabs[randomPowerup].transform.rotation);
    }

    // Generates spawn position randomly
    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);

        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);

        // Returns a Vector3
        return randomPos;
    }

    private void SpawnBossWave(int waveNumber)
    {
        if (waveNumber % 3 == 0)
        {
            bossFight = true;
            boss = Instantiate(bossPrefab, GenerateSpawnPosition(), bossPrefab.transform.rotation);
        }
    }

    private void SpawnMinions()
    {
        int numberOfMinions = 2;
        if (boss != null)
        {
            for (int i = 0; i < numberOfMinions; i++)
            {
                Instantiate(minion, GenerateSpawnPosition(), minion.transform.rotation);
            }
        }
    }
}
