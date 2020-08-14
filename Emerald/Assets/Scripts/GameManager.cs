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
    private GameObject UserGameObject;
    public static UserObject User;
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

        UserGameObject = Instantiate(WarriorModels[0], User.transform.position, Quaternion.identity);
        DontDestroyOnLoad(UserGameObject);
        User.Player = UserGameObject.GetComponent<PlayerObject>();
        User.Player.Class = p.Class;
        User.Player.Gender = p.Gender;
        User.Player.CurrentLocation = new Vector2(p.Location.X, p.Location.Y);
    }

    void Update()
    {
        Network.Process();

        ProcessScene();
    }

    void ProcessScene()
    {
        if (CurrentScene == null) return;        
        if (User.Player == null) return;

        if (!User.Player.Camera.activeSelf)
            User.Player.Camera.SetActive(true);

        if (Input.GetMouseButton(0))
        {
            if (!User.Player.IsMoving)
            {
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                Vector2 middle = new Vector2(Screen.width / 2, Screen.height / 2);

                Vector2 v2 = (mousePosition - middle);
                float angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
                if (angle < 0)
                    angle = 360 + angle;
                angle = 360 - angle;


                System.Drawing.Point newpos = Functions.PointMove(new System.Drawing.Point((int)User.Player.CurrentLocation.x, (int)User.Player.CurrentLocation.y), Functions.MirDrectionFromAngle(angle), 1);
                User.Player.CurrentLocation = new Vector2(newpos.X, newpos.Y);
                Vector3 targetpos = CurrentScene.Cells[newpos.X, newpos.Y].position;
                Vector3 lookpos = new Vector3(targetpos.x, User.Player.Model.transform.position.y, targetpos.z);

                User.Player.Model.transform.LookAt(lookpos);

                User.Player.TargetPosition = targetpos;
                User.Player.StartPosition = User.Player.gameObject.transform.position;
                User.Player.TargetDistance = Vector3.Distance(User.Player.transform.position, targetpos);
                User.Player.IsMoving = true;
                User.Player.GetComponentInChildren<Animator>().SetBool("canWalk", true);
            }
        }
        else
        {
            if (!User.Player.IsMoving)
                User.Player.GetComponentInChildren<Animator>().SetBool("canWalk", false);
        }
    }

    public class NetworkInfo
    {
        public string IPAddress = "127.0.0.1";
        public int Port = 7000;
    }
}
