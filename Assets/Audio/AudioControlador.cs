using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioControlador : MonoBehaviour
{
    #region Audio Mixer e Sliders
    public AudioMixer mixer;
    
    public void AlterarVolumeMaster(float valor)
    {
        mixer.SetFloat("MasterVol", Mathf.Log10(valor) * 20);
    }
    public void AlterarVolumeSFX(float valor)
    {
        mixer.SetFloat("SFXVol", Mathf.Log10(valor) * 20);
    }
    public void AlterarVolumeBGM(float valor)
    {
        mixer.SetFloat("BGMVol", Mathf.Log10(valor) * 20);
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