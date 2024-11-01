using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemyScript = FindObjectOfType<Enemy>();
            enemyScript.RemoveEnemy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("SpawnModeSwitch"))
        {
            Enemy enemyScript = FindObjectOfType<Enemy>();
            enemyScript.ToggleSpawnMode();
            UpdateSwitchColor(collision.gameObject, enemyScript.continuousSpawn);
        }
        else if (collision.gameObject.CompareTag("SpawnToggle"))
        {
            Enemy enemyScript = FindObjectOfType<Enemy>();
            enemyScript.ToggleSpawning();
            UpdateSwitchColor(collision.gameObject, enemyScript.spawning);
        }

        Destroy(gameObject);
    }

    private void UpdateSwitchColor(GameObject switchObject, bool isActive)
    {
        Renderer renderer = switchObject.GetComponent<Renderer>();
        if (isActive)
        {
            renderer.material.color = Color.red;
        }
        else
        {
            renderer.material.color = Color.blue;
        }
    }
}
