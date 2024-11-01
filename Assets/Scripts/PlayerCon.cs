using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : MonoBehaviourPunCallbacks
{
    //===== �ϐ� =====
    //�@�J�����e�I�u�W�F�N�g
    public Transform viewPoint;

    // ���_�ړ��̑��x
    public float mouseSensitivity = 1f;

    // ���[�U�[�̃}�E�X���͊i�[
    private Vector2 mouseInput;

    // y���̉�]�i�[
    private float verticalMouseInput;

    // �J����
    private Camera cam;

    // ���͂��ꂽ�l���i�[
    private Vector3 moveDir;

    // �i�ޕ������i�[
    private Vector3 movement;

    // �ړ����x
    private float MoveSpeed = 4f;

    // �W�����v��
    public Vector3 jumpForce = new Vector3(0f, 6f, 0f);

    // ���C���΂��I�u�W�F�N�g�̈ʒu
    public Transform groundCheckPoint;

    // �n�ʃ��C���[
    public LayerMask groundLayers;

    // ����
    private Rigidbody rb;

    // �����̑��x
    public float walkSpeed = 4f;

    // ����̑��x
    public float runSpeed = 8f;

    // �J�[�\���̕\������
    private bool cursorLock = true;

    // ����i�[���X�g
    public List<Gun> guns = new List<Gun>();

    // �I�𒆂̕���Ǘ��p���l
    private int selectedGun = 0;

    // �ˌ��Ԋu
    private float shotTime;

    // ���L�e��
    [Tooltip("���L�e��")]
    public int[] ammunition;

    // �ő�e��
    [Tooltip("�ő及�L�e��")]
    public int[] maxAmmunition;

    // �}�K�W�����e��
    [Tooltip("�}�K�W�����e��")]
    public int[] ammoClip;

    // �}�K�W�����ő�e��
    [Tooltip("�}�K�W���ɓ���ő�e��")]
    public int[] maxAmmoClip;

    // UIManager
    private UIManager uiMgr;

    // SpawnManager�i�[
    private SpawnMng spawnMng;

    // �ő�HP
    public int maxHP = 100;

    // ����HP
    private int currentHP;

    // ���̃G�t�F�N�g
    public GameObject hitEffect;

    // GameManager�i�[
    GameManager gameManager;

    public float handou = 0.2f;

    //===== Awake =====
    private void Awake()
    {
        // UIMGR�i�[
        uiMgr = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        // SpawnMng�i�[
        spawnMng = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnMng>();

        // �ϐ��ɃR���|�[�l���g�i�[
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    //===== start =====
    private void Start()
    {
        // ����HP�ɍő�HP����
        currentHP = maxHP;

        // �J�����i�[
        cam = Camera.main;

        rb = GetComponent<Rigidbody>();

        // �J�[�\���̕\������
        UpdateCursorLock();

        // random�̈ʒu�ŃX�|�[��������
        //transform.position = spawnMng.GetSpawnPoint().position;

        if(photonView.IsMine)
        {
            // HP���X���C�_�[�ɔ��f
            uiMgr.UpdateHP(maxHP, currentHP);
        }
    }

    //===== Update =====
    private void Update()
    {
        if(!photonView.IsMine)
        {
            return;
        }

        // ���_�ړ��֐��Ăяo��
        PlayerRotate();

        // �ړ��֐�
        PlayerMove();

        if (IsGround())
        {
            // ����֐��Ăяo��
            Run();

            // �W�����v�֐�
            Jump();
        }

        // �ˌ��{�^�����m
        Fire();

        // �����[�h
        Reload();

        // �`�����݊֐�
        Aim();

        // ����̕ύX�L�[���m
        SwitchingGuns();

        // �J�[�\���̕\������
        UpdateCursorLock();

        if (photonView.IsMine)
        {
            if (transform.position.y <= -2)
            {
                currentHP = 0;

                uiMgr.UpdateDeathUI(name);

                spawnMng.Die();
            }
        }
        if(Input.GetMouseButtonUp(0) || ammoClip[2] <= 0)
        {
            photonView.RPC("SoundStop", RpcTarget.All);
        }
    }

    //===== FixedUpdate =====
    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        // �e�L�X�g�X�V�֐�
        uiMgr.SettingBulletsText(ammoClip[selectedGun], ammunition[selectedGun]);
    }

    // ���_�ړ��֐�
    public void PlayerRotate()
    {
        // �ϐ��Ƀ��[�U�[�̃}�E�X�̓������i�[
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X") * mouseSensitivity, Input.GetAxisRaw("Mouse Y") * mouseSensitivity);

        // �}�E�X��x���̓����𔽉f
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + mouseInput.x, transform.eulerAngles.z);

        // y���ɒl�Ɍ��݂̒l�𑫂�
        verticalMouseInput += mouseInput.y;

        // ���l���ۂ߂�
        verticalMouseInput = Mathf.Clamp(verticalMouseInput, -60f, 60f);

        // viewPoint�Ɋۂ߂����l�𔽉f
        viewPoint.rotation = Quaternion.Euler(-verticalMouseInput,
            viewPoint.transform.rotation.eulerAngles.y,
            viewPoint.transform.rotation.eulerAngles.z);
    }

    //===== LastUpdate =====
    private void LateUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        // �J�����̈ʒu����
        cam.transform.position = viewPoint.position;

        // ��]
        cam.transform.rotation = viewPoint.rotation;
    }

    //===== PlayerMove =====
    public void PlayerMove()
    {
        // �ړ��p�L�[�̓��͊��m���Ēl���i�[����
        moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // �i�ޕ������o���ĕϐ��Ɋi�[
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized;

        // ���݈ʒu�ɔ��f���Ă���
        transform.position += movement * MoveSpeed * Time.deltaTime;
    }

    //===== Jump =====
    public void Jump()
    {
        // �n�ʂɂ��Ă��āA�X�y�[�X�L�[�������ꂽ��
        if(IsGround()&&Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(jumpForce, ForceMode.Impulse);
        }
    }

    //===== IsGround =====
    public bool IsGround()
    {
        // ���肵��bool�l��Ԃ�
        return Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.25f, groundLayers);
    }

    //===== Run =====
    public void Run()
    {
        // �V�t�g������Ă���Ƃ��ɃX�s�[�h�؂�ւ���
        if(Input.GetKey(KeyCode.LeftShift))
        {
            MoveSpeed = runSpeed;
        }
        else
        {
            MoveSpeed = walkSpeed;
        }
    }

    //===== UpdateCursorLock =====
    public void UpdateCursorLock()
    {
        // �ؑ�
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLock = false; // �\��
        }
        else if(Input.GetMouseButton(0))
        {
            cursorLock = true; // ��\��
        }

        if(cursorLock) 
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    //===== SwitchingGuns =====
    public void SwitchingGuns()
    {
        // �z�C�[���ŏe�ؑ�
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            selectedGun++;

            if (selectedGun >= guns.Count)
            {
                selectedGun = 0;

            }
            // �e�����ۂɐ؂�ւ���֐�
            switchGun();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;

            if (selectedGun < 0)
            {
                selectedGun = guns.Count - 1;
            }

            // �e�����ۂɐ؂�ւ���֐�
            switchGun();
        }

        // ���l�L�[�ŏe�ؑ�
        for(int i = 0; i < guns.Count; i++)
        {
            // ���l�L�[����������
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                // �e�ؑ�
                selectedGun = i;

                switchGun();
            }
        }
    }

    //===== switchGun =====
    public void switchGun()
    {
        foreach (Gun gun in guns)
        {
            gun.gameObject.SetActive(false);
        }

        guns[selectedGun].gameObject.SetActive(true);
    }

    //===== Aim =====
    public void Aim()
    {
        // �E�N���b�N���m
        if(Input.GetMouseButton(1))
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, guns[selectedGun].adsZoom, guns[selectedGun].adsSoeed * Time.deltaTime);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f, guns[selectedGun].adsSoeed * Time.deltaTime);
        }
    }

    //===== Fire =====
    public void Fire()
    {
        // �ł��o���邩
        if(Input.GetMouseButton(0) && ammoClip[selectedGun] > 0 && Time.time > shotTime)
        {
            // �ʑł��o��
            FiringBullet();
        }
    }

    //===== FiringBullet =====
    public void FiringBullet()
    {
        // �ʌ��炷
        ammoClip[selectedGun]--;

        // ���������
        Ray ray = cam.ViewportPointToRay(new Vector2(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Debug.Log("���������I�u�W�F�N�g��" + hit.collider.gameObject.name);
            if(hit.collider.gameObject.gameObject.tag == "Player")
            {
                PhotonNetwork.Instantiate(hitEffect.name,hit.point, Quaternion.identity);

                hit.collider.gameObject.GetPhotonView().RPC("Hit",RpcTarget.All,
                    guns[selectedGun].shootDamage, photonView.Owner.NickName, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else
            {
                // �e������
                GameObject bullletImpactObject = Instantiate(guns[selectedGun].bulletImpact,
                    hit.point + (hit.normal * 0.02f),
                    Quaternion.LookRotation(hit.normal, Vector3.up));

                Destroy(bullletImpactObject, 10f);
            }
        }

        // �ˌ����ԊԊu
        shotTime = Time.time + guns[selectedGun].shootInterval;
        // �J�����̎��_��y������0.2��Ɉړ�
        verticalMouseInput -= handou;
        verticalMouseInput = Mathf.Clamp(verticalMouseInput, -60f, 60f);
        viewPoint.rotation = Quaternion.Euler(-verticalMouseInput, viewPoint.transform.rotation.eulerAngles.y, viewPoint.transform.rotation.eulerAngles.z);

        photonView.RPC("SoundGeneration", RpcTarget.All);
    }

    //===== Reload =====
    private void Reload()
    {
        // �{�^������
        if(Input.GetKeyDown(KeyCode.R))
        {
            // �����[�h�ŕ�[����e��擾
            int amountNeed = maxAmmoClip[selectedGun] - ammoClip[selectedGun];

            // ��[�������e��Ə����e�򐔂��r
            int ammoAvailable = amountNeed < ammunition[selectedGun] ? amountNeed : ammunition[selectedGun];

            // �e�򂪖��^���������烊���[�h�ł��Ȃ�OR�e����������Ă��鎞
            if(amountNeed != 0 && ammunition[selectedGun] != 0)
            {
                // �����e�򂩂烊���[�h����e�������
                ammunition[selectedGun] -= ammoAvailable;

                // �e�ɒe��Z�b�g
                ammoClip[selectedGun] += ammoAvailable;
            }
        }
    }

    // ��e�֐�
    [PunRPC]
    public void Hit(int damage, string name, int actor)
    {
        // HP�����炷�֐�
        ReceiveDamage(name, damage, actor);
    }

    // HP�����炷�֐�
    public void ReceiveDamage(string name, int damage, int actor)
    {
        if (photonView.IsMine)
        {
            currentHP -= damage;
            PhotonView damageSourcePhotonView = PhotonView.Find(actor);

            if (damageSourcePhotonView != null)
            {
                Vector3 damageSourcePosition = damageSourcePhotonView.transform.position;
                uiMgr.ShowDamageDirection(damageSourcePosition);
            }
            else
            {
                Debug.LogWarning("Damage source PhotonView not found for actor ID: " + actor);
            }

            if (currentHP <= 0)
            {
                Death(name, actor);
            }

            uiMgr.UpdateHP(maxHP, currentHP);
        }
    }

    // ���S�֐�
    public void Death(string name, int actor)
    {
        currentHP = 0;

        uiMgr.UpdateDeathUI(name);

        spawnMng.Die();

        // �L���f�X�C�x���g�Ăяo��
        gameManager.ScoreGet(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);  // �������S���̃C�x���g�Ăяo��

        gameManager.ScoreGet(actor, 0, 1);

        // �L�����O��S�v���C���[�ɕ\��
        PhotonView actorPhotonView = PhotonView.Find(actor);
        if (actorPhotonView != null)
        {
            string killerName = actorPhotonView.Owner.NickName;
            photonView.RPC("UpdateKillLog", RpcTarget.All, killerName, name);
        }
    }

    [PunRPC]
    public void UpdateKillLog(string killer, string victim)
    {
        uiMgr.UpdateKillLog(killer, victim);
    }

    public override void OnDisable()
    {
        // �}�E�X�\��
        cursorLock = false;
        Cursor.lockState = CursorLockMode.None;
    }

    [PunRPC]
    public void SoundGeneration()
    {
        if(selectedGun == 2)
        {
            guns[selectedGun].LoopOff_SubmachinGun();
        }
        else 
        {
            guns[selectedGun].SoundGunShot();
        }
    }

    [PunRPC]
    public void SoundStop()
    {
        guns[2].LoopOff_SubmachinGun();
    }
}
