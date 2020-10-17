using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Network = EmeraldNetwork.Network;
using S = ServerPackets;

public class QueuedAction
{
    public MirAction Action;
    public Vector2 Location;
    public MirDirection Direction;
    public List<object> Params;
}

public class GameManager : MonoBehaviour
{
    public GameObject PlayerModel;
    public List<GameObject> WarriorModels;
    public List<GameObject> WarriorFaces;
    public List<GameObject> WarriorHairs;
    public List<GameObject> WeaponModels;

    public List<GameObject> MonsterModels;

    private GameObject UserGameObject;
    private Dictionary<uint, MapObject> ObjectList = new Dictionary<uint, MapObject>();
    [HideInInspector]
    public static List<ItemInfo> ItemInfoList = new List<ItemInfo>();

    [HideInInspector]
    public static NetworkInfo networkInfo;
    [HideInInspector]
    public static GameStage gameStage;
    [HideInInspector]
    public static GameSceneManager GameScene;
    [HideInInspector]
    public static UserObject User;    
    [HideInInspector]
    public static MirScene CurrentScene;
    [HideInInspector]
    public static float NextAction;
    [HideInInspector]
    public static float InputDelay;
    [HideInInspector]
    public static bool UIDragging;

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

    public void MapInformation(S.MapInformation p)
    {
        FindObjectOfType<LoadScreenManager>().LoadScene(p.FileName);
    }

    public void UserInformation(S.UserInformation p)
    {
        User.gameObject.SetActive(true);
        UserGameObject = Instantiate(PlayerModel, User.transform.position, Quaternion.identity);
        UserGameObject.GetComponentInChildren<AudioListener>().enabled = true;        

        User.Player = UserGameObject.GetComponent<PlayerObject>();
        User.Player.ObjectID = p.ObjectID;
        User.Player.Name = p.Name;
        User.Player.Class = p.Class;
        User.Player.Gender = p.Gender;
        User.Level = p.Level;
        
        GameScene.UpdateCharacterIcon();

        User.Player.CurrentLocation = new Vector2(p.Location.X, p.Location.Y);
        User.Player.gameManager = this;
        UserGameObject.transform.position = CurrentScene.Cells[(int)User.Player.CurrentLocation.x, (int)User.Player.CurrentLocation.y].position;
        User.Player.Direction = p.Direction;
        User.Player.Model.transform.rotation = ClientFunctions.GetRotation(User.Player.Direction);

        User.Inventory = p.Inventory;
        User.Equipment = p.Equipment;
        User.Magics = p.Magics;

        User.BindAllItems();

        if (p.Equipment[(int)EquipmentSlot.Weapon] != null && p.Equipment[(int)EquipmentSlot.Weapon].Info.Shape <= WeaponModels.Count)
            User.Player.Weapon = p.Equipment[(int)EquipmentSlot.Weapon].Info.Shape;
        else
            User.Player.Weapon = 0;

        if (p.Equipment[(int)EquipmentSlot.Armour] != null && p.Equipment[(int)EquipmentSlot.Armour].Info.Shape < WarriorModels.Count)
            User.Player.Armour = p.Equipment[(int)EquipmentSlot.Armour].Info.Shape;
        else
            User.Player.Armour = 0;

        ObjectList.Add(p.ObjectID, User.Player);
        User.Player.Camera.SetActive(true);

        Tooltip.cam = User.Player.Camera.GetComponent<Camera>();
    }

    public void UserLocation(S.UserLocation p)
    {
        NextAction = 0;
    }

    public void AttackMode(S.ChangeAMode p)
    {
        GameScene.SetAttackMode(p.Mode);
    }

    public void ObjectPlayer(S.ObjectPlayer p)
    {
        MapObject ob;
        PlayerObject player;

        if (ObjectList.TryGetValue(p.ObjectID, out ob))
        {
            player = (PlayerObject)ob;
            player.CurrentLocation = new Vector2(p.Location.X, p.Location.Y);
            player.Direction = p.Direction;
            player.transform.position = CurrentScene.Cells[p.Location.X, p.Location.Y].position;
            player.Model.transform.rotation = ClientFunctions.GetRotation(p.Direction);
            player.Armour = p.Armour;
            player.gameObject.SetActive(true);
            CurrentScene.Cells[p.Location.X, p.Location.Y].AddObject(player);
            return;
        }

        player = Instantiate(PlayerModel, CurrentScene.Cells[p.Location.X, p.Location.Y].position, Quaternion.identity).GetComponent<PlayerObject>();
        player.gameManager = this;
        player.Name = p.Name;
        player.ObjectID = p.ObjectID;
        player.CurrentLocation = new Vector2(p.Location.X, p.Location.Y);
        player.Direction = p.Direction;
        player.Model.transform.rotation = ClientFunctions.GetRotation(p.Direction);
        player.Armour = p.Armour;
        ObjectList.Add(p.ObjectID, player);
        CurrentScene.Cells[p.Location.X, p.Location.Y].AddObject(player);
    }

    public void UpdatePlayer(S.PlayerUpdate p)
    {
        MapObject ob;
        PlayerObject player;

        if (ObjectList.TryGetValue(p.ObjectID, out ob))
        {
            player = (PlayerObject)ob;
            player.Armour = p.Armour;
        }
    }

    public void ObjectMonster(S.ObjectMonster p)
    {
        MapObject ob;
        MonsterObject monster;

        if (ObjectList.TryGetValue(p.ObjectID, out ob))
        {
            monster = (MonsterObject)ob;
            monster.Name = p.Name;
            monster.CurrentLocation = new Vector2(p.Location.X, p.Location.Y);
            monster.Direction = p.Direction;
            monster.transform.position = CurrentScene.Cells[p.Location.X, p.Location.Y].position;
            monster.Model.transform.rotation = ClientFunctions.GetRotation(p.Direction);
            monster.gameObject.SetActive(true);
            CurrentScene.Cells[p.Location.X, p.Location.Y].AddObject(monster);
            return;
        }

        monster = Instantiate(MonsterModels[0], CurrentScene.Cells[p.Location.X, p.Location.Y].position, Quaternion.identity).GetComponent<MonsterObject>();
        monster.Name = p.Name;
        monster.ObjectID = p.ObjectID;
        monster.CurrentLocation = new Vector2(p.Location.X, p.Location.Y);
        monster.Direction = p.Direction;
        monster.Model.transform.rotation = ClientFunctions.GetRotation(p.Direction);

        if (p.Dead)
        {
            monster.Dead = true;
            monster.CurrentAction = MirAction.Dead;
        }

        ObjectList.Add(p.ObjectID, monster);
        CurrentScene.Cells[p.Location.X, p.Location.Y].AddObject(monster);
    }

    public void ObjectRemove(S.ObjectRemove p)
    {
        if (ObjectList.TryGetValue(p.ObjectID, out MapObject ob))
        {
            ob.gameObject.SetActive(false);
            CurrentScene.Cells[(int)ob.CurrentLocation.x, (int)ob.CurrentLocation.y].RemoveObject(ob);
        }
    }

    public void ObjectWalk(S.ObjectWalk p)
    {
        if (ObjectList.TryGetValue(p.ObjectID, out MapObject ob))
        {            
            ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Walking, Direction = p.Direction, Location = new Vector2(p.Location.X, p.Location.Y) });
        }
    }

    public void ObjectRun(S.ObjectRun p)
    {
        if (ObjectList.TryGetValue(p.ObjectID, out MapObject ob))
        {
            ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Running, Direction = p.Direction, Location = new Vector2(p.Location.X, p.Location.Y) });
        }
    }

    public void ObjectAttack(S.ObjectAttack p)
    {
        if (ObjectList.TryGetValue(p.ObjectID, out MapObject ob))
        {
            Debug.Log(ob.Name);
            ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Attack, Direction = p.Direction, Location = new Vector2(p.Location.X, p.Location.Y) });
        }
    }

    public void ObjectStruck(S.ObjectStruck p)
    {
        if (ObjectList.TryGetValue(p.ObjectID, out MapObject ob))
        {
            ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Struck, Direction = p.Direction, Location = new Vector2(p.Location.X, p.Location.Y) });
        }
    }

    public void ObjectDied(S.ObjectDied p)
    {
        if (ObjectList.TryGetValue(p.ObjectID, out MapObject ob))
        {
            ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Die, Direction = p.Direction, Location = new Vector2(p.Location.X, p.Location.Y) });
        }
    }

    public void Chat(S.Chat p)
    {
        GameScene.ChatController.ReceiveChat(p.Message, p.Type);
    }

    public void ObjectChat(S.ObjectChat p)
    {
        if (ObjectList.TryGetValue(p.ObjectID, out MapObject ob))
        {
            //player.ActionFeed.Add(new QueuedAction { Action = MirAction.Running, Direction = p.Direction, Location = new Vector2(p.Location.X, p.Location.Y) });
            GameScene.ChatController.ReceiveChat(p.Text, p.Type);
        }
    }

    public void NewItemInfo(S.NewItemInfo info)
    {
        ItemInfoList.Add(info.Info);
    }

    void Update()
    {
        Network.Process();

        ProcessScene();
    }

    static MirDirection MouseDirection()
    {
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 middle = new Vector2(Screen.width / 2, Screen.height / 2 + 50);

        Vector2 v2 = (mousePosition - middle);
        float angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
        if (angle < 0)
            angle = 360 + angle;
        angle = 360 - angle;

        return Functions.MirDrectionFromAngle(angle);
    }

    public static void CheckMouseInput()
    {
        if (CurrentScene == null) return;
        if (User.Player == null) return;
        if (UIDragging) return;

        if (User.Player.ActionFeed.Count == 0 && Time.time > InputDelay)
        {
            if (Input.GetMouseButton(0))
            {
                MirDirection direction = MouseDirection();
                Vector2 newlocation = ClientFunctions.VectorMove(User.Player.CurrentLocation, direction, 1);
                if (CanWalk(newlocation))
                    GameScene.QueuedAction = new QueuedAction { Action = MirAction.Walking, Direction = direction, Location = newlocation };

            }
            else if (Input.GetMouseButton(1))
            {
                MirDirection direction = MouseDirection();
                Vector2 newlocation = ClientFunctions.VectorMove(User.Player.CurrentLocation, direction, 1);
                if (User.WalkStep < 1)
                {
                    if (CanWalk(newlocation))
                        GameScene.QueuedAction = new QueuedAction { Action = MirAction.Walking, Direction = direction, Location = newlocation };
                    User.WalkStep++;
                }
                else
                {
                    Vector2 farlocation = ClientFunctions.VectorMove(User.Player.CurrentLocation, direction, 2);
                    if (CanWalk(newlocation) && CanWalk(farlocation))
                        GameScene.QueuedAction = new QueuedAction { Action = MirAction.Running, Direction = direction, Location = farlocation };
                }
            }
        }
    }

    public static void AddItem(UserItem item)
    {
        if (item.Info.StackSize > 1) //Stackable
        {
            for (int i = 0; i < User.Inventory.Length; i++)
            {
                UserItem temp = User.Inventory[i];
                if (temp == null || item.Info != temp.Info || temp.Count >= temp.Info.StackSize) continue;

                if (item.Count + temp.Count <= temp.Info.StackSize)
                {
                    temp.Count += item.Count;
                    return;
                }
                item.Count -= temp.Info.StackSize - temp.Count;
                temp.Count = temp.Info.StackSize;
            }
        }

        if (item.Info.Type == ItemType.Potion || item.Info.Type == ItemType.Scroll || (item.Info.Type == ItemType.Script && item.Info.Effect == 1))
        {
            for (int i = 0; i < User.BeltIdx - 2; i++)
            {
                if (User.Inventory[i] != null) continue;
                User.Inventory[i] = item;
                return;
            }
        }
        else if (item.Info.Type == ItemType.Amulet)
        {
            for (int i = 4; i < User.BeltIdx; i++)
            {
                if (User.Inventory[i] != null) continue;
                User.Inventory[i] = item;
                return;
            }
        }
        else
        {
            for (int i = User.BeltIdx; i < User.Inventory.Length; i++)
            {
                if (User.Inventory[i] != null) continue;
                User.Inventory[i] = item;
                return;
            }
        }

        for (int i = 0; i < User.Inventory.Length; i++)
        {
            if (User.Inventory[i] != null) continue;
            User.Inventory[i] = item;
            return;
        }
    }

    public static void Bind(UserItem item)
    {
        for (int i = 0; i < ItemInfoList.Count; i++)
        {
            if (ItemInfoList[i].Index != item.ItemIndex) continue;

            item.Info = ItemInfoList[i];

            item.SetSlotSize();

            for (int s = 0; s < item.Slots.Length; s++)
            {
                if (item.Slots[s] == null) continue;

                Bind(item.Slots[s]);
            }

            return;
        }
    }

    void ProcessScene()
    {                
    }

    static bool CanWalk(Vector2 location)
    {
        return CurrentScene.Cells[(int)location.x, (int)location.y].walkable && CurrentScene.Cells[(int)location.x, (int)location.y].CellObjects == null;
    }

    public class NetworkInfo
    {
        public string IPAddress = "127.0.0.1";
        public int Port = 7000;
    }    
}
