using Network = EmeraldNetwork.Network;
using C = ClientPackets;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScreenManager : MonoBehaviour
{
    //[SerializeField]
    public Slider slider;

    private Canvas canvas;

    void Awake()
    {
        canvas = GetComponentInChildren<Canvas>(true);
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        slider.value = 0;
        canvas.gameObject.SetActive(true);

        StartCoroutine(BeginLoad(sceneName));
    }

    private IEnumerator BeginLoad(string sceneName)
    {       
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            Network.Process();
            float progress = Mathf.Clamp01(operation.progress / .9f);            
            slider.value = progress;
            yield return null;
        }

        slider.value = operation.progress;
        operation = null;
        canvas.gameObject.SetActive(false);
        Network.Enqueue(new C.MapLoaded { });
    }    
}
