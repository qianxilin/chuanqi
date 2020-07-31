using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class FadeAudioOnSceneLoad : MonoBehaviour
{
    public bool fade = false;
    public AudioSource m_AudioSource; // Probably be better to fade in "Music" audio mixer only
    public AudioMixer m_AudioMixer;   // by using this instead m_AudioMixer.SetFloat("musicVol", value)
    [Range(0f, 10f)]
    public float musicFadeSpeed;

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {       
        fade = true;
    }

    void Update()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (fade == true)
        {
            m_AudioSource.volume = Mathf.Lerp(m_AudioSource.volume, 0, musicFadeSpeed * Time.deltaTime);
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }    
}