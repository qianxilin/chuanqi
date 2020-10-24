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

    public void Show()
    {
        slider.value = 0;
        canvas.gameObject.SetActive(true);
    }

    public void Hide()
    {
        canvas.gameObject.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        Show();

        StartCoroutine(BeginLoad(sceneName));
    }

    void Update()
    {
        transform.SetAsLastSibling();
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
        Hide();
        Network.Enqueue(new C.MapLoaded { });
    }    
}
