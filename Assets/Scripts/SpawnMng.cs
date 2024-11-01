using UnityEngine;
using Photon.Pun;

public class SpawnMng : MonoBehaviour
{
    // spawnPoint�i�[�z��쐬
    public Transform[] spawnPoints;

    // ��������v���C���[�I�u�W�F�N�g
    public GameObject playerPrefab;

    // ���������v���O�����I�u�W�F�N�g
    private GameObject player;

    // �X�|�[���܂ł̃C���^�[�o��
    public float respawnInterval = 5f;

    //===== Start =====
    private void Start()
    {
        // �X�|�[���I�u�W�F�N�g�����ׂĔ�\��
        foreach (Transform position in spawnPoints)
        {
            position.gameObject.SetActive(false);
        }

        // �����֐��Ăяo��
        if(PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    // �֐�
    // random�ɃX�|�[���|�C���g�̂P��I������֐�
    public Transform GetSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    // �l�b�g���[�N�I�u�W�F�N�g�Ƃ��ăv���C���[�𐶐�
    public void SpawnPlayer()
    {
        // random�ȃX�|�[���|�W�V������ϐ��Ɋi�[
        Transform spawnPoint = GetSpawnPoint();

        // �l�b�g���[�N�I�u�W�F�N�g����
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }

    // �폜�ƃ��X�|�[��
    public void Die()
    {
        if(player != null)
        {
            // ���X�|�[���֐����Ă�
            Invoke("SpawnPlayer", 5f);
        }

        PhotonNetwork.Destroy(player);
    }
}
