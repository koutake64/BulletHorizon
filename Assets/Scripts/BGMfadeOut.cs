using System.Collections;
using UnityEngine;

public class BGMfadeOut : MonoBehaviour
{
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void VolumeChange()
    {
        StartCoroutine("VolumeDown");
    }

    IEnumerator VolumeDown()
    {
        while (audioSource.volume > 0)
        {
            audioSource.volume -= 0.02f;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
