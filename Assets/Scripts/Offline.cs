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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<OfflinePlayer>(); // オフラインプレイヤーの取得
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
                player.SetPauseState(true); // ポーズ状態に設定
            }
            else
            {
                Panel = false;
                Time.timeScale = 1f;
                PausePanel.SetActive(false);
                Buttons.SetActive(false);
                player.SetPauseState(false); // ポーズ解除状態に設定
            }
        }
    }

    public void Retrun()
    {
        Panel = false;
        Time.timeScale = 1f;
        PausePanel.SetActive(false);
        Buttons.SetActive(false);
        player.SetPauseState(false); // ポーズ解除状態に設定
    }


    public void Title()
    {
        SceneManager.LoadScene(SceneName);
    }
}
