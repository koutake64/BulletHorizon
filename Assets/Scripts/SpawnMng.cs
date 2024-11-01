using UnityEngine;
using Photon.Pun;

public class SpawnMng : MonoBehaviour
{
    // spawnPoint格納配列作成
    public Transform[] spawnPoints;

    // 生成するプレイヤーオブジェクト
    public GameObject playerPrefab;

    // 生成したプログラムオブジェクト
    private GameObject player;

    // スポーンまでのインターバル
    public float respawnInterval = 5f;

    //===== Start =====
    private void Start()
    {
        // スポーンオブジェクトをすべて非表示
        foreach (Transform position in spawnPoints)
        {
            position.gameObject.SetActive(false);
        }

        // 生成関数呼び出し
        if(PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    // 関数
    // randomにスポーンポイントの１つを選択する関数
    public Transform GetSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    // ネットワークオブジェクトとしてプレイヤーを生成
    public void SpawnPlayer()
    {
        // randomなスポーンポジションを変数に格納
        Transform spawnPoint = GetSpawnPoint();

        // ネットワークオブジェクト生成
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }

    // 削除とリスポーン
    public void Die()
    {
        if(player != null)
        {
            // リスポーン関数を呼ぶ
            Invoke("SpawnPlayer", 5f);
        }

        PhotonNetwork.Destroy(player);
    }
}
