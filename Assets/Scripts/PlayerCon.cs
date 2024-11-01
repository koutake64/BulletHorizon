using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : MonoBehaviourPunCallbacks
{
    //===== 変数 =====
    //　カメラ親オブジェクト
    public Transform viewPoint;

    // 視点移動の速度
    public float mouseSensitivity = 1f;

    // ユーザーのマウス入力格納
    private Vector2 mouseInput;

    // y軸の回転格納
    private float verticalMouseInput;

    // カメラ
    private Camera cam;

    // 入力された値を格納
    private Vector3 moveDir;

    // 進む方向を格納
    private Vector3 movement;

    // 移動速度
    private float MoveSpeed = 4f;

    // ジャンプ力
    public Vector3 jumpForce = new Vector3(0f, 6f, 0f);

    // レイを飛ばすオブジェクトの位置
    public Transform groundCheckPoint;

    // 地面レイヤー
    public LayerMask groundLayers;

    // 剛体
    private Rigidbody rb;

    // 歩きの速度
    public float walkSpeed = 4f;

    // 走りの速度
    public float runSpeed = 8f;

    // カーソルの表示判定
    private bool cursorLock = true;

    // 武器格納リスト
    public List<Gun> guns = new List<Gun>();

    // 選択中の武器管理用数値
    private int selectedGun = 0;

    // 射撃間隔
    private float shotTime;

    // 所有弾薬
    [Tooltip("所有弾薬")]
    public int[] ammunition;

    // 最大弾薬数
    [Tooltip("最大所有弾薬")]
    public int[] maxAmmunition;

    // マガジン内弾薬
    [Tooltip("マガジン内弾薬")]
    public int[] ammoClip;

    // マガジン内最大弾薬数
    [Tooltip("マガジンに入る最大弾薬数")]
    public int[] maxAmmoClip;

    // UIManager
    private UIManager uiMgr;

    // SpawnManager格納
    private SpawnMng spawnMng;

    // 最大HP
    public int maxHP = 100;

    // 現在HP
    private int currentHP;

    // 血のエフェクト
    public GameObject hitEffect;

    // GameManager格納
    GameManager gameManager;

    public float handou = 0.2f;

    //===== Awake =====
    private void Awake()
    {
        // UIMGR格納
        uiMgr = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        // SpawnMng格納
        spawnMng = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnMng>();

        // 変数にコンポーネント格納
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    //===== start =====
    private void Start()
    {
        // 現在HPに最大HPを代入
        currentHP = maxHP;

        // カメラ格納
        cam = Camera.main;

        rb = GetComponent<Rigidbody>();

        // カーソルの表示判定
        UpdateCursorLock();

        // randomの位置でスポーンさせる
        //transform.position = spawnMng.GetSpawnPoint().position;

        if(photonView.IsMine)
        {
            // HPをスライダーに反映
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

        // 視点移動関数呼び出し
        PlayerRotate();

        // 移動関数
        PlayerMove();

        if (IsGround())
        {
            // 走り関数呼び出し
            Run();

            // ジャンプ関数
            Jump();
        }

        // 射撃ボタン検知
        Fire();

        // リロード
        Reload();

        // 覗きこみ関数
        Aim();

        // 武器の変更キー検知
        SwitchingGuns();

        // カーソルの表示判定
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

        // テキスト更新関数
        uiMgr.SettingBulletsText(ammoClip[selectedGun], ammunition[selectedGun]);
    }

    // 視点移動関数
    public void PlayerRotate()
    {
        // 変数にユーザーのマウスの動きを格納
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X") * mouseSensitivity, Input.GetAxisRaw("Mouse Y") * mouseSensitivity);

        // マウスのx軸の動きを反映
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + mouseInput.x, transform.eulerAngles.z);

        // y軸に値に現在の値を足す
        verticalMouseInput += mouseInput.y;

        // 数値を丸める
        verticalMouseInput = Mathf.Clamp(verticalMouseInput, -60f, 60f);

        // viewPointに丸めた数値を反映
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

        // カメラの位置調整
        cam.transform.position = viewPoint.position;

        // 回転
        cam.transform.rotation = viewPoint.rotation;
    }

    //===== PlayerMove =====
    public void PlayerMove()
    {
        // 移動用キーの入力感知して値を格納する
        moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // 進む方向を出して変数に格納
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized;

        // 現在位置に反映していく
        transform.position += movement * MoveSpeed * Time.deltaTime;
    }

    //===== Jump =====
    public void Jump()
    {
        // 地面についていて、スペースキーが押された時
        if(IsGround()&&Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(jumpForce, ForceMode.Impulse);
        }
    }

    //===== IsGround =====
    public bool IsGround()
    {
        // 判定してbool値を返す
        return Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.25f, groundLayers);
    }

    //===== Run =====
    public void Run()
    {
        // シフト押されているときにスピード切り替える
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
        // 切替
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLock = false; // 表示
        }
        else if(Input.GetMouseButton(0))
        {
            cursorLock = true; // 非表示
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
        // ホイールで銃切替
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            selectedGun++;

            if (selectedGun >= guns.Count)
            {
                selectedGun = 0;

            }
            // 銃を実際に切り替える関数
            switchGun();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;

            if (selectedGun < 0)
            {
                selectedGun = guns.Count - 1;
            }

            // 銃を実際に切り替える関数
            switchGun();
        }

        // 数値キーで銃切替
        for(int i = 0; i < guns.Count; i++)
        {
            // 数値キーを押したか
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                // 銃切替
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
        // 右クリック検知
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
        // 打ち出せるか
        if(Input.GetMouseButton(0) && ammoClip[selectedGun] > 0 && Time.time > shotTime)
        {
            // 玉打ち出し
            FiringBullet();
        }
    }

    //===== FiringBullet =====
    public void FiringBullet()
    {
        // 玉減らす
        ammoClip[selectedGun]--;

        // 光線を作る
        Ray ray = cam.ViewportPointToRay(new Vector2(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Debug.Log("当たったオブジェクト＞" + hit.collider.gameObject.name);
            if(hit.collider.gameObject.gameObject.tag == "Player")
            {
                PhotonNetwork.Instantiate(hitEffect.name,hit.point, Quaternion.identity);

                hit.collider.gameObject.GetPhotonView().RPC("Hit",RpcTarget.All,
                    guns[selectedGun].shootDamage, photonView.Owner.NickName, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else
            {
                // 弾痕生成
                GameObject bullletImpactObject = Instantiate(guns[selectedGun].bulletImpact,
                    hit.point + (hit.normal * 0.02f),
                    Quaternion.LookRotation(hit.normal, Vector3.up));

                Destroy(bullletImpactObject, 10f);
            }
        }

        // 射撃時間間隔
        shotTime = Time.time + guns[selectedGun].shootInterval;
        // カメラの視点をy方向に0.2上に移動
        verticalMouseInput -= handou;
        verticalMouseInput = Mathf.Clamp(verticalMouseInput, -60f, 60f);
        viewPoint.rotation = Quaternion.Euler(-verticalMouseInput, viewPoint.transform.rotation.eulerAngles.y, viewPoint.transform.rotation.eulerAngles.z);

        photonView.RPC("SoundGeneration", RpcTarget.All);
    }

    //===== Reload =====
    private void Reload()
    {
        // ボタン判定
        if(Input.GetKeyDown(KeyCode.R))
        {
            // リロードで補充する弾薬取得
            int amountNeed = maxAmmoClip[selectedGun] - ammoClip[selectedGun];

            // 補充したい弾薬と所持弾薬数を比較
            int ammoAvailable = amountNeed < ammunition[selectedGun] ? amountNeed : ammunition[selectedGun];

            // 弾薬が満タンだったらリロードできないOR弾薬を所持している時
            if(amountNeed != 0 && ammunition[selectedGun] != 0)
            {
                // 所持弾薬からリロードする弾薬を引く
                ammunition[selectedGun] -= ammoAvailable;

                // 銃に弾薬セット
                ammoClip[selectedGun] += ammoAvailable;
            }
        }
    }

    // 被弾関数
    [PunRPC]
    public void Hit(int damage, string name, int actor)
    {
        // HPを減らす関数
        ReceiveDamage(name, damage, actor);
    }

    // HPを減らす関数
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

    // 死亡関数
    public void Death(string name, int actor)
    {
        currentHP = 0;

        uiMgr.UpdateDeathUI(name);

        spawnMng.Die();

        // キルデスイベント呼び出し
        gameManager.ScoreGet(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);  // 自分死亡時のイベント呼び出し

        gameManager.ScoreGet(actor, 0, 1);

        // キルログを全プレイヤーに表示
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
        // マウス表示
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
