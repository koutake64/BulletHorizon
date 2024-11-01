using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject enemyPrefab; // �G�̃v���n�u
    public Transform[] spawnPoints; // �G�̃X�|�[���|�C���g
    public int maxEnemies = 15; // ���������ő吔
    public float spawnInterval = 5f; // �����Ԋu

    private List<GameObject> enemies = new List<GameObject>();
    public bool continuousSpawn = false;
    public bool spawning = false; // �ŏ���False

    private void Start()
    {
        // �����ł�ToggleSpawning���Ăяo���܂���
    }

    private IEnumerator SpawnEnemies()
    {
        while (spawning)
        {
            if (continuousSpawn)
            {
                // �ő吔�`�F�b�N���Ă��琶��
                if (enemies.Count < maxEnemies)
                {
                    SpawnEnemy();
                }
            }
            else
            {
                // �O�̓G���폜
                if (enemies.Count > 0)
                {
                    RemoveEnemy(enemies[0]);
                }

                // �V�����G�𐶐�
                SpawnEnemy();

                // �����Ԋu��҂�
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
