using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class FadeAudioOnSceneLoad : MonoBehaviour
{
    private bool fade = false;
    public AudioMixer audioMixer;   // by using this instead m_AudioMixer.SetFloat("musicVol", value)
    public string ChannelName;    
    [Range(0f, 10f)]
    public float musicFadeSpeed;
    public bool DestroyAfterFade;
    public bool ResetAfterFade;

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
            audioMixer.GetFloat(ChannelName, out float currentvolume);
            float newvolume = Mathf.Max(-80f, currentvolume - musicFadeSpeed * Time.deltaTime);
            audioMixer.SetFloat(ChannelName, newvolume);
            if (newvolume < -50f)
            {
                if (ResetAfterFade)
                    audioMixer.SetFloat(ChannelName, 0);
                if (DestroyAfterFade)
                    Destroy(gameObject);
            }
        }
    }   
}