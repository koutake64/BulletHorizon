using UnityEngine;

public class Gun : MonoBehaviour
{
    //===== �ϐ� =====
    // �ˌ��Ԋu
    [Tooltip("�ˌ��Ԋu")]
    public float shootInterval = 0.1f;

    // �З�
    [Tooltip("�З�")]
    public int shootDamage;

    // �Y�[��
    [Tooltip("�Y�[��")]
    public float adsZoom;

    // �`�����ݎ��̑��x
    [Tooltip("�`�����ݎ��̑��x")]
    public float adsSoeed;

    // �e���I�u�W�F�N�g
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
