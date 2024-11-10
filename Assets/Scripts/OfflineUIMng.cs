using UnityEngine;
using UnityEngine.UI;

public class OfflineUIMng : MonoBehaviour
{   
    //===== •Ï” =====
    [SerializeField] Text ammoText;

    public void SettingBulletsText(int ammoClip, int ammunition)
    {
        // ƒ}ƒKƒWƒ““à‚Ì’e–ò/Š’e–ò
        ammoText.text = ammoClip + "/ ‡";
    }
}
