using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPositons;
    public GameObject playerPrefab;

    private void Start()
    {
        foreach (var pos in spawnPositons)
        {
            pos.gameObject.SetActive(false);
        }

        
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    public Transform GetSpawnPoint()
    {
        return spawnPositons[Random.Range(0, spawnPositons.Length)];
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = GetSpawnPoint();

        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
}
