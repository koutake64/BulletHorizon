using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // 既存の変数
    public Text ammoText;
    public Slider HPSlider;
    public GameObject deathPanel;
    public Text deathText;
    public GameObject scoreboard;
    public PlayerInformation info;
    public GameObject endPanel;

    // ダメージ方向を示す矢印
    public GameObject damageIndicatorArrow;

    // キルログ表示用のテキストフィールド
    public Text killLogText;

    // 弾薬テキストを設定
    public void SettingBulletsText(int ammoClip, int ammunition)
    {
        ammoText.text = ammoClip + "/" + ammunition;
    }

    // HPを更新
    public void UpdateHP(int maxHP, int currentHP)
    {
        HPSlider.maxValue = maxHP;
        HPSlider.value = currentHP;
    }

    // デスUIを更新
    public void UpdateDeathUI(string name)
    {
        deathPanel.SetActive(true);
        deathText.text = name + "に倒された。";
        Invoke("CloseDeathUI", 5f);
    }

    // デスUIを閉じる
    public void CloseDeathUI()
    {
        deathPanel.SetActive(false);
    }

    // スコアボードを開く
    public void ChangeScoreUI()
    {
        scoreboard.SetActive(!scoreboard.activeInHierarchy);
    }

    // 終了パネルを開く
    public void OpenEndPanel()
    {
        endPanel.SetActive(true);
    }

    // ダメージ方向を示す矢印を表示
    public void ShowDamageDirection(Vector3 damageSourcePosition)
    {
        Vector3 direction = damageSourcePosition - Camera.main.transform.position;
        direction.y = 0; // 水平方向のみ考慮

        float angle = Vector3.SignedAngle(Camera.main.transform.forward, direction, Vector3.up);
        damageIndicatorArrow.transform.rotation = Quaternion.Euler(0, 0, -angle);

        // 矢印を表示する
        damageIndicatorArrow.SetActive(true);

        // 一定時間後に矢印を非表示にする
        Invoke("HideDamageIndicator", 2f);
    }

    // ダメージ矢印を非表示にする
    private void HideDamageIndicator()
    {
        damageIndicatorArrow.SetActive(false);
    }

    // キルログを更新
    public void UpdateKillLog(string killer, string victim)
    {
        killLogText.text += $"{killer} が {victim} を倒した\n";
        Invoke("ClearKillLog", 5f); // 5秒後にログをクリアする
    }

    // キルログをクリア
    private void ClearKillLog()
    {
        killLogText.text = "";
    }
}
