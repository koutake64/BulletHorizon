using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
    // ���O�e�L�X�g
    public Text playerNameText;

    // �L���e�L�X�g
    public Text killesText;

    // �f�X�e�L�X�g
    public Text deathText;

    // �\�ɖ��O��L���f�X����\������
    public void SetPlayerDetailes(string name, int kill, int death)
    {
        playerNameText.text = name;
        killesText.text = kill.ToString();
        deathText.text = death.ToString();
    }
}
