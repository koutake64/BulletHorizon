using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{

    float speed = 3f;
    SpawnManager spawnManager;



    private void Awake()
    {
        spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
    }

    void Update()
    {

        if (!photonView.IsMine)
        {
            return;
        }

        transform.position += new Vector3(Input.GetAxisRaw("Horizontal"),
            0, 
            Input.GetAxisRaw("Vertical")) * speed * Time.deltaTime;
    }

}
