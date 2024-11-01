using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflinePlayer : MonoBehaviour
{
    //===== ïœêî =====
    public Transform viewPoint;
    public float mouseSensitivity = 1f;
    private Vector2 mouseInput;
    private float verticalMouseInput;
    private Camera cam;
    private Vector3 moveDir;
    private Vector3 movement;
    private float MoveSpeed = 4f;
    public Vector3 jumpForce = new Vector3(0f, 6f, 0f);
    public Transform groundCheckPoint;
    public LayerMask groundLayers;
    private Rigidbody rb;
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    private bool cursorLock = true;
    public List<Gun> guns = new List<Gun>();
    private int selectedGun = 1;
    private float shotTime;
    [Tooltip("èäóLíeñÚ")]
    public int[] ammunition;
    [Tooltip("ç≈ëÂèäóLíeñÚ")]
    public int[] maxAmmunition;
    [Tooltip("É}ÉKÉWÉìì‡íeñÚ")]
    public int[] ammoClip;
    [Tooltip("É}ÉKÉWÉìÇ…ì¸ÇÈç≈ëÂíeñÚêî")]
    public int[] maxAmmoClip;
    private OfflineUIMng uiMgr;
    public GameObject hitEffect;
    public float handou = 0.2f;

    private bool isPaused = false;

    private void Awake()
    {
        uiMgr = GameObject.FindGameObjectWithTag("OfflineUIManager").GetComponent<OfflineUIMng>();
    }

    private void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        UpdateCursorLock();
    }

    private void Update()
    {
        if (isPaused)
        {
            return;
        }

        PlayerRotate();
        PlayerMove();

        if (IsGround())
        {
            Run();
            Jump();
        }

        Fire();
        Reload();
        Aim();  // Aimä÷êîÇåƒÇ—èoÇ∑
        SwitchingGuns();
        UpdateCursorLock();
    }

    private void FixedUpdate()
    {
        if (!isPaused)
        {
            uiMgr.SettingBulletsText(ammoClip[selectedGun], ammunition[selectedGun]);
        }
    }

    public void PlayerRotate()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X") * mouseSensitivity, Input.GetAxisRaw("Mouse Y") * mouseSensitivity);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + mouseInput.x, transform.eulerAngles.z);
        verticalMouseInput += mouseInput.y;
        verticalMouseInput = Mathf.Clamp(verticalMouseInput, -60f, 60f);
        viewPoint.rotation = Quaternion.Euler(-verticalMouseInput, viewPoint.transform.rotation.eulerAngles.y, viewPoint.transform.rotation.eulerAngles.z);
    }

    private void LateUpdate()
    {
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }

    public void PlayerMove()
    {
        moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized;
        transform.position += movement * MoveSpeed * Time.deltaTime;
    }

    public void Jump()
    {
        if (IsGround() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(jumpForce, ForceMode.Impulse);
        }
    }

    public bool IsGround()
    {
        return Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.25f, groundLayers);
    }

    public void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            MoveSpeed = runSpeed;
        }
        else
        {
            MoveSpeed = walkSpeed;
        }
    }

    public void UpdateCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLock = false;
        }
        else if (Input.GetMouseButton(0) && !isPaused)
        {
            cursorLock = true;
        }

        if (cursorLock && !isPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void SwitchingGuns()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            selectedGun++;
            if (selectedGun >= guns.Count)
            {
                selectedGun = 0;
            }
            switchGun();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;
            if (selectedGun < 0)
            {
                selectedGun = guns.Count - 1;
            }
            switchGun();
        }

        for (int i = 0; i < guns.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                selectedGun = i;
                switchGun();
            }
        }
    }

    public void switchGun()
    {
        for (int i = 0; i < guns.Count; i++)
        {
            if (i == selectedGun)
            {
                guns[i].gameObject.SetActive(true);
            }
            else
            {
                guns[i].gameObject.SetActive(false);
            }
        }
    }

    public void Fire()
    {
        if (Input.GetMouseButton(0) && ammoClip[selectedGun] > 0 && Time.time > shotTime)
        {
            FiringBullet();
        }
    }

    public void FiringBullet()
    {
        ammoClip[selectedGun]--;
        Ray ray = cam.ViewportPointToRay(new Vector2(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                Enemy enemyScript = FindObjectOfType<Enemy>();
                enemyScript.RemoveEnemy(hit.collider.gameObject);
            }
            else if (hit.collider.gameObject.CompareTag("SpawnModeSwitch"))
            {
                Enemy enemyScript = FindObjectOfType<Enemy>();
                enemyScript.ToggleSpawnMode();
                UpdateSwitchColor(hit.collider.gameObject, enemyScript.continuousSpawn);
            }
            else if (hit.collider.gameObject.CompareTag("SpawnToggle"))
            {
                Enemy enemyScript = FindObjectOfType<Enemy>();
                enemyScript.ToggleSpawning();
                UpdateSwitchColor(hit.collider.gameObject, enemyScript.spawning);
            }
            else
            {
                GameObject bullletImpactObject = Instantiate(guns[selectedGun].bulletImpact, hit.point + (hit.normal * 0.02f), Quaternion.LookRotation(hit.normal, Vector3.up));
                Destroy(bullletImpactObject, 10f);
            }

            Instantiate(hitEffect, hit.point, Quaternion.identity);
        }

        shotTime = Time.time + guns[selectedGun].shootInterval;

        // ÉJÉÅÉâÇÃéãì_Çyï˚å¸Ç…0.2è„Ç…à⁄ìÆ
        verticalMouseInput -= handou;
        verticalMouseInput = Mathf.Clamp(verticalMouseInput, -60f, 60f);
        viewPoint.rotation = Quaternion.Euler(-verticalMouseInput, viewPoint.transform.rotation.eulerAngles.y, viewPoint.transform.rotation.eulerAngles.z);
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

    public void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            int amountNeed = maxAmmoClip[selectedGun] - ammoClip[selectedGun];
            int ammoAvailable = amountNeed < ammunition[selectedGun] ? amountNeed : ammunition[selectedGun];

            if (amountNeed != 0 && ammunition[selectedGun] != 0)
            {
                ammunition[selectedGun] -= ammoAvailable;
                ammoClip[selectedGun] += ammoAvailable;
            }
        }
    }

    public void Aim()
    {
        if (Input.GetMouseButton(1))
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, guns[selectedGun].adsZoom, guns[selectedGun].adsSoeed * Time.deltaTime);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f, guns[selectedGun].adsSoeed * Time.deltaTime);
        }
    }

    public void SetPauseState(bool paused)
    {
        isPaused = paused;
        UpdateCursorLock();
    }
}
