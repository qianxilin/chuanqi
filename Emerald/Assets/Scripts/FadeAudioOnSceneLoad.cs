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

    void Awake()
    {        
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {       
        fade = true;
    }

    void Update()
    {        
        if (fade == true)
        {
            float oldvolume = m_AudioSource.volume;
            m_AudioSource.volume = Mathf.Max(0, m_AudioSource.volume - musicFadeSpeed * Time.deltaTime);
            if (m_AudioSource.volume <= 0)
                Destroy(gameObject);
        }
    }   
}