using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider seSlider;

    private const string MasterVolumeKey = "MasterVolume";
    private const string BGMVolumeKey = "BGMVolume";
    private const string SEVolumeKey = "SEVolume";

    void Start()
    {
        // �X���C�_�[�̏����ݒ�
        masterSlider.value = PlayerPrefs.GetFloat(MasterVolumeKey, 0.2f);
        bgmSlider.value = PlayerPrefs.GetFloat(BGMVolumeKey, 0.2f);
        seSlider.value = PlayerPrefs.GetFloat(SEVolumeKey, 0.2f);

        // �X���C�_�[�̃��X�i�[��ݒ�
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        seSlider.onValueChanged.AddListener(SetSEVolume);

        // �������ʐݒ�
        SetMasterVolume(masterSlider.value);
        SetBGMVolume(bgmSlider.value);
        SetSEVolume(seSlider.value);
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat(MasterVolumeKey, volume);
    }

    public void SetBGMVolume(float volume)
    {
        // BGM�̉��ʐݒ�i��Ƃ��āA�^�O��"BGM"�̃I�u�W�F�N�g�̉��ʂ𒲐��j
        foreach (var source in GameObject.FindGameObjectsWithTag("BGM"))
        {
            source.GetComponent<AudioSource>().volume = volume;
        }
        PlayerPrefs.SetFloat(BGMVolumeKey, volume);
    }

    public void SetSEVolume(float volume)
    {
        // SE�̉��ʐݒ�i��Ƃ��āA�^�O��"SE"�̃I�u�W�F�N�g�̉��ʂ𒲐��j
        foreach (var source in GameObject.FindGameObjectsWithTag("SE"))
        {
            source.GetComponent<AudioSource>().volume = volume;
        }
        PlayerPrefs.SetFloat(SEVolumeKey, volume);
    }
}
