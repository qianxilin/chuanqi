using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Network = EmeraldNetwork.Network;

public class GameManager : MonoBehaviour
{
    public static NetworkInfo networkInfo;
    public static GameStage gameStage;

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

    void Update()
    {
        Network.Process();
    }

    public class NetworkInfo
    {
        public string IPAddress = "127.0.0.1";
        public int Port = 7000;
    }
}
