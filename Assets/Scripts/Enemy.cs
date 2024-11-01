using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject enemyPrefab; // 敵のプレハブ
    public Transform[] spawnPoints; // 敵のスポーンポイント
    public int maxEnemies = 15; // 同時生成最大数
    public float spawnInterval = 5f; // 生成間隔

    private List<GameObject> enemies = new List<GameObject>();
    public bool continuousSpawn = false;
    public bool spawning = false; // 最初はFalse

    private void Start()
    {
        // ここではToggleSpawningを呼び出しません
    }

    private IEnumerator SpawnEnemies()
    {
        while (spawning)
        {
            if (continuousSpawn)
            {
                // 最大数チェックしてから生成
                if (enemies.Count < maxEnemies)
                {
                    SpawnEnemy();
                }
            }
            else
            {
                // 前の敵を削除
                if (enemies.Count > 0)
                {
                    RemoveEnemy(enemies[0]);
                }

                // 新しい敵を生成
                SpawnEnemy();

                // 生成間隔を待つ
                yield return new WaitForSeconds(spawnInterval);
            }

            yield return null;
        }
    }

    public void SpawnEnemy()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        enemies.Add(newEnemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
        Destroy(enemy);
    }

    public void ToggleSpawnMode()
    {
        continuousSpawn = !continuousSpawn;
    }

    public void ToggleSpawning()
    {
        spawning = !spawning;
        if (spawning)
        {
            StartCoroutine(SpawnEnemies());
        }
        else
        {
            StopAllCoroutines();
        }
    }
}
