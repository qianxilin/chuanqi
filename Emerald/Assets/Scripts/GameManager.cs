using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Network = EmeraldNetwork.Network;
using S = ServerPackets;

public class GameManager : MonoBehaviour
{
    public static NetworkInfo networkInfo;
    public static GameStage gameStage;
    public UserObject User;
    public List<GameObject> WarriorModels;
    public static MirScene CurrentScene;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameStage = GameStage.Login;

        networkInfo = new NetworkInfo();
        string fileName = Application.dataPath + "/Settings.json";
        string json = string.Empty;

        if (File.Exists(fileName))
            json = File.ReadAllText(fileName);
        else
        {
            json = JsonUtility.ToJson(networkInfo);
            File.WriteAllText(fileName, json);
        }
        
        networkInfo = JsonUtility.FromJson<NetworkInfo>(json);
        Network.Connect();
    }

    public void UserInformation(S.UserInformation p)
    {
        User.transform.position =new Vector3(p.x, p.y, p.z);
        User.gameObject.SetActive(true);
        User.Class = p.Class;
        User.Gender = p.Gender;
    }

    void Update()
    {
        Network.Process();

        ProcessScene();
    }

    void ProcessScene()
    {
        if (CurrentScene == null) return;
        if (!User.gameObject.activeSelf) return;

        if (CurrentScene.UserObject == null)
        {
            CurrentScene.UserObject = Instantiate(WarriorModels[0], User.transform.position, Quaternion.identity);
            CurrentScene.UserObject.GetComponent<PlayerObject>().Camera.SetActive(true);
        }
    }

    public class NetworkInfo
    {
        public string IPAddress = "127.0.0.1";
        public int Port = 7000;
    }
}
