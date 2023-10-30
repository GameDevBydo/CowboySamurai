using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.IO;

public class AudioControlador : MonoBehaviour
{
    #region Audio Mixer e Sliders
    public static AudioControlador instance;
    public AudioMixer mixer;
    public Image sfxBar, masterVolBar, BMGBar;

    void Start()
    {
        instance = this;
        if(File.Exists(Application.persistentDataPath + "/SaveConfigs.txt"))
            SaveController.LoadConfigs();
    }

    void Update()
    {
        mixer.SetFloat("SFXVol", Mathf.Log10(sfxBar.fillAmount) * 20);
        mixer.SetFloat("MasterVol", Mathf.Log10(masterVolBar.fillAmount) * 20);
        mixer.SetFloat("BGMVol", Mathf.Log10(BMGBar.fillAmount) * 20);
    }
    
    public void AlterarVolumeMaster()
    {
        //mixer.SetFloat("MasterVol", Mathf.Log10(valor) * 20);
        masterVolBar.fillAmount += 0.249f;
        if(masterVolBar.fillAmount >= 1.0f){
            masterVolBar.fillAmount = 0.001f;
        }
    }
    public void AlterarVolumeSFX()
    {
        //mixer.SetFloat("SFXVol", Mathf.Log10(valor) * 20);
        sfxBar.fillAmount += 0.249f;
        if(sfxBar.fillAmount>=1.0f){
            sfxBar.fillAmount = 0.001f;
        }
    }
    public void AlterarVolumeBGM()
    {
        BMGBar.fillAmount += 0.249f;
        if(BMGBar.fillAmount >= 1.0f){
            BMGBar.fillAmount = 0.001f;
        }
    }

    #endregion

    #region Jukebox / Musicas

    public AudioClip[] songs;

    public AudioSource musicSpeaker;

    public void StopSong()
    {
        musicSpeaker.Stop();
    }

    public void PlaySong(int id)
    {
        if(id>=songs.Length)
        {
            id = 0;
            Debug.Log("<color=red>Index de música fora do Array.</color>");
        } 
        StopSong();
        musicSpeaker.clip = songs[id];
        musicSpeaker.Play();
    }

    public void PauseSong()
    {
        musicSpeaker.Pause();
    }
    public void UnpauseSong()
    {
        musicSpeaker.UnPause();
    }
    #endregion
}