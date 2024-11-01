using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Offline : MonoBehaviour
{
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject Buttons;
    private bool Panel;
    private OfflinePlayer player;
    [SerializeField] private string SceneName; 

    private void Start()
    {
        PausePanel.SetActive(false);
        Buttons.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<OfflinePlayer>(); // �I�t���C���v���C���[�̎擾
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!Panel)
            {
                Panel = true;
                Time.timeScale = 0f;
                PausePanel.SetActive(true);
                Buttons.SetActive(true);
                player.SetPauseState(true); // �|�[�Y��Ԃɐݒ�
            }
            else
            {
                Panel = false;
                Time.timeScale = 1f;
                PausePanel.SetActive(false);
                Buttons.SetActive(false);
                player.SetPauseState(false); // �|�[�Y������Ԃɐݒ�
            }
        }
    }

    public void Retrun()
    {
        Panel = false;
        Time.timeScale = 1f;
        PausePanel.SetActive(false);
        Buttons.SetActive(false);
        player.SetPauseState(false); // �|�[�Y������Ԃɐݒ�
    }


    public void Title()
    {
        SceneManager.LoadScene(SceneName);
    }
}
