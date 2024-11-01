using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // �����̕ϐ�
    public Text ammoText;
    public Slider HPSlider;
    public GameObject deathPanel;
    public Text deathText;
    public GameObject scoreboard;
    public PlayerInformation info;
    public GameObject endPanel;

    // �_���[�W�������������
    public GameObject damageIndicatorArrow;

    // �L�����O�\���p�̃e�L�X�g�t�B�[���h
    public Text killLogText;

    // �e��e�L�X�g��ݒ�
    public void SettingBulletsText(int ammoClip, int ammunition)
    {
        ammoText.text = ammoClip + "/" + ammunition;
    }

    // HP���X�V
    public void UpdateHP(int maxHP, int currentHP)
    {
        HPSlider.maxValue = maxHP;
        HPSlider.value = currentHP;
    }

    // �f�XUI���X�V
    public void UpdateDeathUI(string name)
    {
        deathPanel.SetActive(true);
        deathText.text = name + "�ɓ|���ꂽ�B";
        Invoke("CloseDeathUI", 5f);
    }

    // �f�XUI�����
    public void CloseDeathUI()
    {
        deathPanel.SetActive(false);
    }

    // �X�R�A�{�[�h���J��
    public void ChangeScoreUI()
    {
        scoreboard.SetActive(!scoreboard.activeInHierarchy);
    }

    // �I���p�l�����J��
    public void OpenEndPanel()
    {
        endPanel.SetActive(true);
    }

    // �_���[�W��������������\��
    public void ShowDamageDirection(Vector3 damageSourcePosition)
    {
        Vector3 direction = damageSourcePosition - Camera.main.transform.position;
        direction.y = 0; // ���������̂ݍl��

        float angle = Vector3.SignedAngle(Camera.main.transform.forward, direction, Vector3.up);
        damageIndicatorArrow.transform.rotation = Quaternion.Euler(0, 0, -angle);

        // ����\������
        damageIndicatorArrow.SetActive(true);

        // ��莞�Ԍ�ɖ����\���ɂ���
        Invoke("HideDamageIndicator", 2f);
    }

    // �_���[�W�����\���ɂ���
    private void HideDamageIndicator()
    {
        damageIndicatorArrow.SetActive(false);
    }

    // �L�����O���X�V
    public void UpdateKillLog(string killer, string victim)
    {
        killLogText.text += $"{killer} �� {victim} ��|����\n";
        Invoke("ClearKillLog", 5f); // 5�b��Ƀ��O���N���A����
    }

    // �L�����O���N���A
    private void ClearKillLog()
    {
        killLogText.text = "";
    }
}
