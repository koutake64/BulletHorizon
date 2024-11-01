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
        // スライダーの初期設定
        masterSlider.value = PlayerPrefs.GetFloat(MasterVolumeKey, 0.2f);
        bgmSlider.value = PlayerPrefs.GetFloat(BGMVolumeKey, 0.2f);
        seSlider.value = PlayerPrefs.GetFloat(SEVolumeKey, 0.2f);

        // スライダーのリスナーを設定
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        seSlider.onValueChanged.AddListener(SetSEVolume);

        // 初期音量設定
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
        // BGMの音量設定（例として、タグが"BGM"のオブジェクトの音量を調整）
        foreach (var source in GameObject.FindGameObjectsWithTag("BGM"))
        {
            source.GetComponent<AudioSource>().volume = volume;
        }
        PlayerPrefs.SetFloat(BGMVolumeKey, volume);
    }

    public void SetSEVolume(float volume)
    {
        // SEの音量設定（例として、タグが"SE"のオブジェクトの音量を調整）
        foreach (var source in GameObject.FindGameObjectsWithTag("SE"))
        {
            source.GetComponent<AudioSource>().volume = volume;
        }
        PlayerPrefs.SetFloat(SEVolumeKey, volume);
    }
}
