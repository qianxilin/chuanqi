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
        User.transform.position = new Vector3(p.x, p.y, p.z);
        User.gameObject.SetActive(true);
        User.Class = p.Class;
        User.Gender = p.Gender;
        User.CurrentLocation = new Vector2(p.Location.X, p.Location.Y);
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

        if (Input.GetMouseButton(0))
        {
            if (!CurrentScene.UserObject.GetComponent<PlayerObject>().IsMoving)
            {
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                Vector2 middle = new Vector2(Screen.width / 2, Screen.height / 2);

                Vector2 v2 = (mousePosition - middle);
                float angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
                if (angle < 0)
                    angle = 360 + angle;
                angle = 360 - angle;


                System.Drawing.Point newpos = Functions.PointMove(new System.Drawing.Point((int)User.CurrentLocation.x, (int)User.CurrentLocation.y), Functions.MirDrectionFromAngle(angle), 1);
                User.CurrentLocation = new Vector2(newpos.X, newpos.Y);
                Vector3 targetpos = CurrentScene.Cells[newpos.X, newpos.Y].position;
                Vector3 lookpos = new Vector3(targetpos.x, CurrentScene.UserObject.GetComponent<PlayerObject>().Model.transform.position.y, targetpos.z);

                CurrentScene.UserObject.GetComponent<PlayerObject>().Model.transform.LookAt(lookpos);

                CurrentScene.UserObject.GetComponent<PlayerObject>().TargetPosition = targetpos;
                CurrentScene.UserObject.GetComponent<PlayerObject>().StartPosition = CurrentScene.UserObject.transform.position;
                CurrentScene.UserObject.GetComponent<PlayerObject>().TargetDistance = Vector3.Distance(CurrentScene.UserObject.transform.position, targetpos);
                CurrentScene.UserObject.GetComponent<PlayerObject>().IsMoving = true;
                CurrentScene.UserObject.GetComponentInChildren<Animator>().SetBool("canWalk", true);
            }
        }
        else
        {
            if (!CurrentScene.UserObject.GetComponent<PlayerObject>().IsMoving)
                CurrentScene.UserObject.GetComponentInChildren<Animator>().SetBool("canWalk", false);
        }
    }

    public class NetworkInfo
    {
        public string IPAddress = "127.0.0.1";
        public int Port = 7000;
    }
}
