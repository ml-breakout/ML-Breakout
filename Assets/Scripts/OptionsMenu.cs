using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TextMeshProUGUI volumeText;

    public void SetVolume(float volume){
        Debug.Log(volume);

        audioMixer.SetFloat("Volume", volume * 8f);

        float volumePercent = (volume + 10f) * 10f;

        volumeText.text = volumePercent.ToString();

    }

    public void SetFullScreen(bool fullscreen){

        Debug.Log(fullscreen);

        Screen.fullScreen = fullscreen;
    }
}
