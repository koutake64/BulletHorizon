using UnityEngine;
using UnityEngine.UI;

public class OfflineUIMng : MonoBehaviour
{   
    //===== �ϐ� =====
    public Text ammoText;

    public void SettingBulletsText(int ammoClip, int ammunition)
    {
        // �}�K�W�����̒e��/�����e��
        ammoText.text = ammoClip + "/ ��";
    }
}
