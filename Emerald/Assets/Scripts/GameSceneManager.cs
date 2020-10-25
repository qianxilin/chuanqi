using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;
using S = ServerPackets;
using Image = UnityEngine.UI.Image;
using Color = UnityEngine.Color;

public class GameSceneManager : MonoBehaviour
{
    protected static UserObject User
    {
        get { return GameManager.User; }
    }

    public TMP_InputField ChatBar;      
    public Scrollbar ScrollBar;
    public ChatController ChatController;
    public EventSystem eventSystem;
    public TMP_Text CharacterName;
    public TMP_Text CharacterLevel;
    public Image CharacterIcon;
    public Sprite[] CharacterIcons = new Sprite[Enum.GetNames(typeof(MirClass)).Length * Enum.GetNames(typeof(MirGender)).Length];
    public Button AttackModeButton;
    public TMP_Text AttackModeText;
    public Sprite[] AttackModeIcons = new Sprite[Enum.GetNames(typeof(AttackMode)).Length];
    public Sprite[] AttackModeHoverIcons = new Sprite[Enum.GetNames(typeof(AttackMode)).Length];
    public Sprite[] AttackModeDownIcons = new Sprite[Enum.GetNames(typeof(AttackMode)).Length];
    public TMP_Text StatsDisplay;
    public Renderer HPGlobe;
    public Renderer MPGlobe;
    public MirMessageBox MessageBox;

    [HideInInspector]
    public InventoryController Inventory;
    public ItemTooltip ItemToolTip;
    public Image SelectedItemImage;
    public MirItemCell[] EquipmentCells = new MirItemCell[14];
    public MirItemCell[] BeltCells = new MirItemCell[6];

    [HideInInspector]
    public bool PickedUpGold;
    [HideInInspector]
    public float UseItemTime;
    public float PickUpTime;

    private MapObject targetObject;
    public MapObject TargetObject
    {
        get { return targetObject; }
        set
        {
            if (value == targetObject) return;
            targetObject = value;
        }
    }

    private MapObject mouseObject;
    public MapObject MouseObject
    {
        get { return mouseObject; }
        set
        {
            if (value == mouseObject) return;
            if (mouseObject != null)
                mouseObject.OnDeSelect();
            mouseObject = value;
            if (mouseObject != null)
                mouseObject.OnSelect();
        }
    }

    [HideInInspector]
    public float NextHitTime;
    [HideInInspector]
    public QueuedAction QueuedAction;

    private MirItemCell _selectedCell;
    [HideInInspector]
    public MirItemCell SelectedCell
    {
        get { return _selectedCell; }
        set
        {
            if (_selectedCell == value) return;

            _selectedCell = value;
            OnSelectedCellChanged();
        }
    }

    private void OnSelectedCellChanged()
    {
        if (SelectedCell == null)
            SelectedItemImage.gameObject.SetActive(false);
        else
        {
            SelectedItemImage.gameObject.SetActive(true);
            SelectedItemImage.transform.position = Input.mousePosition;
            SelectedItemImage.sprite = Resources.Load<Sprite>($"Items/{SelectedCell.Item.Info.Image}");
        }
    }

    void Awake()
    {
        GameManager.GameScene = this;             
    }

    void Start()
    {
        ScrollBar.size = 0.4f;
        Network.Enqueue(new C.RequestMapInformation { });
        Inventory.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (ChatBar.gameObject.activeSelf)
            {
                
                if (ChatBar.text.Length > 0)
                    Network.Enqueue(new C.Chat() { Message = ChatBar.text });
                ChatBar.text = string.Empty;
                ChatBar.gameObject.SetActive(false);
            }
            else
            {
                ChatBar.gameObject.SetActive(true);
                ChatBar.Select();
            }
        }

        if (SelectedItemImage.gameObject.activeSelf)
        {
            SelectedItemImage.transform.position = Input.mousePosition;
            SelectedItemImage.transform.SetAsLastSibling();
        }

        MouseObject = GetMouseObject();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Time.time > PickUpTime)
            {
                PickUpTime = Time.time + 0.2f;
                Network.Enqueue(new C.PickUp());
            }
        }

        if (Input.GetMouseButton(0) && !eventSystem.IsPointerOverGameObject())
        {
            GameManager.User.CanRun = false;

            if (SelectedCell != null)
            {
                SelectedItemImage.gameObject.SetActive(false);

                MessageBox.Show($"Drop {SelectedCell.Item.Name}?", true, true);
                MessageBox.OK += () =>
                {
                    Network.Enqueue(new C.DropItem { UniqueID = SelectedCell.Item.UniqueID, Count = 1 });
                    SelectedCell.Locked = true;
                    SelectedCell = null;
                };
                MessageBox.Cancel += () =>
                {
                    SelectedCell = null;
                };
                return;
            }

            if (MouseObject != null)
            {
                switch (MouseObject.gameObject.layer)
                {
                    case 9: //Monster
                        MonsterObject monster = (MonsterObject)MouseObject;
                        if (monster.Dead) break;
                        TargetObject = monster;
                        return;
                }
            }

            TargetObject = null;
            GameManager.CheckMouseInput();
        }
        else if (Input.GetMouseButton(1) && !eventSystem.IsPointerOverGameObject())         
            GameManager.CheckMouseInput();
        else
        {
            GameManager.User.CanRun = false;
            if (TargetObject != null && !TargetObject.Dead && TargetObject.gameObject.activeSelf && CanAttack())
            {
                Point self = new Point((int)User.Player.CurrentLocation.x, (int)User.Player.CurrentLocation.y);
                Point targ = new Point((int)TargetObject.CurrentLocation.x, (int)TargetObject.CurrentLocation.y);
                if (Functions.InRange(self, targ, 1))
                {
                    NextHitTime = Time.time + 1.6f;
                    MirDirection direction = Functions.DirectionFromPoint(self, targ);
                    QueuedAction = new QueuedAction { Action = MirAction.Attack, Direction = direction, Location = User.Player.CurrentLocation };
                    return;
                }

                MirDirection targetdirection = Functions.DirectionFromPoint(self, targ);

                if (!CanWalk(targetdirection)) return;

                QueuedAction = new QueuedAction { Action = MirAction.Walking, Direction = targetdirection, Location = ClientFunctions.VectorMove(User.Player.CurrentLocation, targetdirection, 1) };
            }
        }
    }

    private bool CanWalk(MirDirection dir)
    {
        Vector2 newpoint = ClientFunctions.VectorMove(User.Player.CurrentLocation, dir, 1);
        return GameManager.CurrentScene.Cells[(int)newpoint.x, (int)newpoint.y].walkable && GameManager.CurrentScene.Cells[(int)newpoint.x, (int)newpoint.y].CellObjects == null;
    }

    private MapObject GetMouseObject()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selection = hit.transform;
            if (selection.gameObject.layer == 9/*Monster*/ || selection.gameObject.layer == 13/*Item*/)
                return selection.GetComponent<MapObject>();
        }

        return null;
    }

    public bool CanAttack()
    {
        if (Time.time < NextHitTime) return false;

        return true;
    }

    public void GainedItem(S.GainedItem p)
    {
        GameManager.Bind(p.Item);
        GameManager.AddItem(p.Item);
        //User.RefreshStats();

        ChatController.ReceiveChat(string.Format(GameLanguage.YouGained, p.Item.FriendlyName), ChatType.System);
    }

    public void MoveItem(S.MoveItem p)
    {
        MirItemCell toCell, fromCell;

        switch (p.Grid)
        {
            case MirGridType.Inventory:
                fromCell = p.From < User.BeltIdx ? BeltCells[p.From] : Inventory.Cells[p.From - User.BeltIdx];
                break;
            default:
                return;
        }

        switch (p.Grid)
        {
            case MirGridType.Inventory:
                toCell = p.To < User.BeltIdx ? BeltCells[p.To] : Inventory.Cells[p.To - User.BeltIdx];
                break;
            default:
                return;
        }

        if (toCell == null || fromCell == null) return;

        toCell.Locked = false;
        fromCell.Locked = false;

        if (!p.Success) return;

        UserItem i = fromCell.Item;
        fromCell.Item = toCell.Item;
        toCell.Item = i;
    }

    public MirItemCell GetCell(MirItemCell[] cells, ulong id)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].Item == null || cells[i].Item.UniqueID != id) continue;
            return cells[i];
        }
        return null;
    }

    public void EquipItem(S.EquipItem p)
    {
        MirItemCell fromCell;
        MirItemCell toCell = EquipmentCells[p.To];

        switch (p.Grid)
        {
            case MirGridType.Inventory:
                fromCell = GetCell(Inventory.Cells, p.UniqueID) ?? GetCell(BeltCells, p.UniqueID);
                break;
            /*case MirGridType.Storage:
                fromCell = StorageDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);
                break;*/
            default:
                return;
        }

        if (toCell == null || fromCell == null) return;

        toCell.Locked = false;
        fromCell.Locked = false;

        if (!p.Success) return;

        UserItem i = fromCell.Item;
        fromCell.Item = toCell.Item;
        toCell.Item = i;
        User.RefreshStats();
    }

    public void RemoveItem(S.RemoveItem p)
    {
        MirItemCell toCell;

        int index = -1;

        for (int i = 0; i < User.Equipment.Length; i++)
        {
            if (User.Equipment[i] == null || User.Equipment[i].UniqueID != p.UniqueID) continue;
            index = i;
            break;
        }

        MirItemCell fromCell = EquipmentCells[index];


        switch (p.Grid)
        {
            case MirGridType.Inventory:
                toCell = p.To < User.BeltIdx ? BeltCells[p.To] : Inventory.Cells[p.To - User.BeltIdx];
                break;
            /*case MirGridType.Storage:
                toCell = StorageDialog.Grid[p.To];
                break;*/
            default:
                return;
        }

        if (toCell == null || fromCell == null) return;

        toCell.Locked = false;
        fromCell.Locked = false;

        if (!p.Success) return;
        toCell.Item = fromCell.Item;
        fromCell.Item = null;
        User.RefreshStats();
    }

    public void UseItem(S.UseItem p)
    {
        MirItemCell cell = GetCell(Inventory.Cells, p.UniqueID) ?? GetCell(BeltCells, p.UniqueID);

        if (cell == null) return;

        cell.Locked = false;

        if (!p.Success) return;
        if (cell.Item.Count > 1) cell.Item.Count--;
        else cell.Item = null;
        User.RefreshStats();
    }

    public void DropItem(S.DropItem p)
    {
        MirItemCell cell = GetCell(Inventory.Cells, p.UniqueID) ?? GetCell(BeltCells, p.UniqueID);

        if (cell == null) return;

        cell.Locked = false;

        if (!p.Success) return;

        if (p.Count == cell.Item.Count)
            cell.Item = null;
        else
            cell.Item.Count -= p.Count;

        User.RefreshStats();
    }

    public void NewMagic(S.NewMagic p)
    {
        ClientMagic magic = p.Magic;

        User.Magics.Add(magic);
        //User.RefreshStats();
        //foreach (SkillBarDialog Bar in SkillBarDialogs)
        //    Bar.Update();
    }

    public void UpdateCharacterIcon()
    {
        CharacterIcon.sprite = CharacterIcons[(int)GameManager.User.Player.Class * 2 + (int)GameManager.User.Player.Gender];
        CharacterName.text = GameManager.User.Player.Name;
        CharacterLevel.text = GameManager.User.Level.ToString();
    }

    public void ChangeAttackMode(int amode)
    {
        if (amode >= Enum.GetNames(typeof(AttackMode)).Length) return;
        Network.Enqueue(new C.ChangeAMode() { Mode = (AttackMode)amode });
    }

    public void SetAttackMode(AttackMode amode)
    {
        AttackModeButton.GetComponent<Image>().sprite = AttackModeIcons[(int)amode];

        SpriteState state = new SpriteState();
        state = AttackModeButton.spriteState;
        state.highlightedSprite = AttackModeHoverIcons[(int)amode];
        state.pressedSprite = AttackModeDownIcons[(int)amode];

        AttackModeButton.spriteState = state;

        AttackModeText.text = amode.ToString();
    }

    public void RefreshStatsDisplay()
    {
        StatsDisplay.text = $"DC: {User.MinDC}-{User.MaxDC}" + Environment.NewLine +
            $"MC: {User.MinMC}-{User.MaxMC}" + Environment.NewLine +
            $"SC: {User.MinSC}-{User.MaxSC}" + Environment.NewLine +
            $"AC: {User.MinAC}-{User.MaxAC}" + Environment.NewLine +
            $"MAC: {User.MinMAC}-{User.MaxMAC}" + Environment.NewLine +
            $"HP: {User.HP}/{User.MaxHP}" + Environment.NewLine +
            $"MP: {User.MP}/{User.MaxMP}" + Environment.NewLine +
            $"Luck: {User.Luck}";
    }

    public void LogOut_Click()
    {
        MessageBox.Show($"Return to Character Select?", true, true);
        MessageBox.OK += () =>
        {
            Network.Enqueue(new C.LogOut());
            FindObjectOfType<LoadScreenManager>().Show();
        };
    }


}
