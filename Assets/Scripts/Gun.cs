using UnityEngine;

public class Gun : MonoBehaviour
{
    //===== 変数 =====
    // 射撃間隔
    [Tooltip("射撃間隔")]
    public float shootInterval = 0.1f;

    // 威力
    [Tooltip("威力")]
    public int shootDamage;

    // ズーム
    [Tooltip("ズーム")]
    public float adsZoom;

    // 覗き込み時の速度
    [Tooltip("覗き込み時の速度")]
    public float adsSoeed;

    // 弾痕オブジェクト
    public GameObject bulletImpact;

    public AudioSource shotSound;

    public void SoundGunShot()
    {
        shotSound.Play();
    }

    public void LoopOn_SubmachinGun()
    {
        if(!shotSound.isPlaying)
        {
            shotSound.loop = true;
            shotSound.Play();
        }
    }

    public void LoopOff_SubmachinGun()
    {        
        shotSound.loop = false;
        shotSound.Stop();
        
    }
}
