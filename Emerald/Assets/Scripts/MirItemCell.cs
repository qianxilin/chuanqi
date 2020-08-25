using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;

public class MirItemCell : MonoBehaviour, IPointerDownHandler
{
    protected static GameSceneManager GameScene
    {
        get { return GameManager.GameScene; }
    }

    private Image ItemImage;
    public Sprite IconImage;

    private static Color VisibleColor = new Color(255, 255, 255, 255);
    private static Color HideColor = new Color(255, 255, 255, 0);

    [HideInInspector]
    public UserItem Item
    {
        get
        {
            if (ItemArray != null && _itemSlot >= 0 && _itemSlot < ItemArray.Length)
                return ItemArray[_itemSlot];
            return null;
        }
        set
        {
            if (ItemArray != null && _itemSlot >= 0 && _itemSlot < ItemArray.Length)
                ItemArray[_itemSlot] = value;

            Redraw();
        }
    }

    #region ItemSlot
    [SerializeField]
    private int _itemSlot;
    [HideInInspector]
    public event EventHandler ItemSlotChanged;
    [HideInInspector]
    public int ItemSlot
    {
        get { return _itemSlot; }
        set
        {
            if (_itemSlot == value) return;
            _itemSlot = value;
            OnItemSlotChanged();
        }
    }

    private void OnItemSlotChanged()
    {
        if (ItemSlotChanged != null)
            ItemSlotChanged.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #region GridType
    [SerializeField]
    private MirGridType _gridType;
    [HideInInspector]
    public event EventHandler GridTypeChanged;
    [HideInInspector]
    public MirGridType GridType
    {
        get { return _gridType; }
        set
        {
            if (_gridType == value) return;
            _gridType = value;            
            OnGridTypeChanged();
        }
    }

    private void OnGridTypeChanged()
    {
        if (GridTypeChanged != null)
            GridTypeChanged.Invoke(this, EventArgs.Empty);
    }
    #endregion
    [HideInInspector]
    public UserItem[] ItemArray
    {
        get
        {
            switch (GridType)
            {
                case MirGridType.Inventory:
                    return GameManager.User.Inventory;
                case MirGridType.Equipment:
                    return GameManager.User.Equipment;
                default:
                    throw new NotImplementedException();
            }

        }
    }

    private bool _locked;
    [HideInInspector]
    public bool Locked
    {
        get { return _locked; }
        set
        {
            if (_locked == value) return;
            _locked = value;
            Redraw();
        }
    }

    void Awake()
    {
        ItemImage = gameObject.GetComponent<Image>();
        Redraw();
    }

    void Update()
    {
        if (GridType == MirGridType.None || Item == null || Item.Info == null) return;
        if (Item.NeedRefresh)
        {
            Redraw();
            Item.NeedRefresh = true;
        }
    }

    void Redraw()
    {        
        if (GridType == MirGridType.None || Item == null || Item.Info == null)
        {
            if (IconImage != null)
                ItemImage.sprite = IconImage;
            else
                ItemImage.color = HideColor;
            return;
        }
        ItemImage.color = VisibleColor;

        switch (GridType)
        {
            case MirGridType.Equipment:
                ItemImage.sprite = Resources.Load<Sprite>($"UI/StateItems/{Item.Info.Image}");
                break;
            default:
                ItemImage.sprite = Resources.Load<Sprite>($"UI/Items/{Item.Info.Image}");
                break;
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Locked) return;

        if (GameScene.PickedUpGold || GridType == MirGridType.Inspect || GridType == MirGridType.QuestInventory) return;

        if (GameScene.SelectedCell == null && (GridType == MirGridType.Mail)) return;

        switch (eventData.button)
        {
            case PointerEventData.InputButton.Right:
                //UseItem();
                break;
            case PointerEventData.InputButton.Left:
                if (Item != null && GameScene.SelectedCell == null)
                    PlayItemSound();

                /*if (CMain.Ctrl)
                {
                    if (Item != null)
                    {
                        string text = string.Format("<{0}> ", Item.Name);

                        if (GameScene.Scene.ChatDialog.ChatTextBox.Text.Length + text.Length > Globals.MaxChatLength)
                        {
                            GameScene.Scene.ChatDialog.ReceiveChat("Unable to link item, message exceeds allowed length", ChatType.System);
                            return;
                        }

                        GameScene.Scene.ChatDialog.LinkedItems.Add(new ChatItem { UniqueID = Item.UniqueID, Title = Item.Name, Grid = GridType });
                        GameScene.Scene.ChatDialog.SetChatText(text);
                    }

                    break;
                }*/

                /*if (CMain.Shift)
                {
                    if (GridType == MirGridType.Inventory || GridType == MirGridType.Storage)
                    {
                        if (GameScene.SelectedCell == null && Item != null)
                        {
                            if (FreeSpace() == 0)
                            {
                                GameScene.Scene.ChatDialog.ReceiveChat("No room to split stack.", ChatType.System);
                                return;
                            }

                            if (Item.Count > 1)
                            {
                                MirAmountBox amountBox = new MirAmountBox("Split Amount:", Item.Image, Item.Count - 1);

                                amountBox.OKButton.Click += (o, a) =>
                                {
                                    if (amountBox.Amount == 0 || amountBox.Amount >= Item.Count) return;
                                    Network.Enqueue(new C.SplitItem { Grid = GridType, UniqueID = Item.UniqueID, Count = amountBox.Amount });
                                    Locked = true;
                                };

                                amountBox.Show();
                            }
                        }
                    }
                }
                else*/ MoveItem();
                break;
        }
    }

    private void MoveItem()
    {
        if (GridType == MirGridType.BuyBack || GridType == MirGridType.DropPanel || GridType == MirGridType.Inspect || GridType == MirGridType.TrustMerchant || GridType == MirGridType.Craft) return;

        if (GameScene.SelectedCell != null)
        {
            if (GameScene.SelectedCell.Item == null || GameScene.SelectedCell == this)
            {
                GameScene.SelectedCell = null;
                return;
            }

            switch (GridType)
            {
                #region To Inventory
                case MirGridType.Inventory: // To Inventory
                    switch (GameScene.SelectedCell.GridType)
                    {
                        #region From Inventory
                        case MirGridType.Inventory: //From Invenotry
                            if (Item != null)
                            {
                                /*if (CMain.Ctrl)
                                {
                                    MirMessageBox messageBox = new MirMessageBox("Do you want to try and combine these items?", MirMessageBoxButtons.YesNo);
                                    messageBox.YesButton.Click += (o, e) =>
                                    {
                                        //Combine
                                        Network.Enqueue(new C.CombineItem { IDFrom = GameScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });
                                        Locked = true;
                                        GameScene.SelectedCell.Locked = true;
                                        GameScene.SelectedCell = null;
                                    };

                                    messageBox.Show();
                                    return;
                                }*/

                                if (GameScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                {
                                    //Merge
                                    Network.Enqueue(new C.MergeItem { GridFrom = GameScene.SelectedCell.GridType, GridTo = GridType, IDFrom = GameScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                    Locked = true;
                                    GameScene.SelectedCell.Locked = true;
                                    GameScene.SelectedCell = null;
                                    return;
                                }
                            }

                            Network.Enqueue(new C.MoveItem { Grid = GridType, From = GameScene.SelectedCell.ItemSlot, To = ItemSlot });

                            Locked = true;
                            GameScene.SelectedCell.Locked = true;
                            GameScene.SelectedCell = null;
                            return;
                        #endregion

                        #region From Equipment
                        case MirGridType.Equipment: //From Equipment
                            if (Item != null && GameScene.SelectedCell.Item.Info.Type == ItemType.Amulet)
                            {
                                if (GameScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                {
                                    Network.Enqueue(new C.MergeItem { GridFrom = GameScene.SelectedCell.GridType, GridTo = GridType, IDFrom = GameScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                    Locked = true;
                                    GameScene.SelectedCell.Locked = true;
                                    GameScene.SelectedCell = null;
                                    return;
                                }
                            }

                            if (!CanRemoveItem(GameScene.SelectedCell.Item))
                            {
                                GameScene.SelectedCell = null;
                                return;
                            }
                            if (Item == null)
                            {
                                Network.Enqueue(new C.RemoveItem { Grid = GridType, UniqueID = GameScene.SelectedCell.Item.UniqueID, To = ItemSlot });

                                Locked = true;
                                GameScene.SelectedCell.Locked = true;
                                GameScene.SelectedCell = null;
                                return;
                            }

                            for (int x = 6; x < ItemArray.Length; x++)
                                if (ItemArray[x] == null)
                                {
                                    Network.Enqueue(new C.RemoveItem { Grid = GridType, UniqueID = GameScene.SelectedCell.Item.UniqueID, To = x });

                                    MirItemCell temp = x < GameManager.User.BeltIdx ? GameScene.BeltCells[x] : GameScene.Inventory.Cells[x - GameManager.User.BeltIdx];

                                    if (temp != null) temp.Locked = true;
                                    GameScene.SelectedCell.Locked = true;
                                    GameScene.SelectedCell = null;
                                    return;
                                }
                            break;
                            #endregion
                    }
                    break;
                #endregion

                #region To Equipment
                case MirGridType.Equipment: //To Equipment

                    if (GameScene.SelectedCell.GridType != MirGridType.Inventory && GameScene.SelectedCell.GridType != MirGridType.Storage) return;


                    if (Item != null && GameScene.SelectedCell.Item.Info.Type == ItemType.Amulet)
                    {
                        if (GameScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                        {
                            Network.Enqueue(new C.MergeItem { GridFrom = GameScene.SelectedCell.GridType, GridTo = GridType, IDFrom = GameScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                            Locked = true;
                            GameScene.SelectedCell.Locked = true;
                            GameScene.SelectedCell = null;
                            return;
                        }
                    }

                    if (CorrectSlot(GameScene.SelectedCell.Item))
                    {
                        if (CanWearItem(GameScene.SelectedCell.Item))
                        {
                            Network.Enqueue(new C.EquipItem { Grid = GameScene.SelectedCell.GridType, UniqueID = GameScene.SelectedCell.Item.UniqueID, To = ItemSlot });
                            Locked = true;
                            GameScene.SelectedCell.Locked = true;
                        }
                        GameScene.SelectedCell = null;
                    }
                    return;
                    #endregion
            }

            return;
        }

        if (Item != null)
        {
            GameScene.SelectedCell = this;
        }
    }

    private void PlayItemSound()
    {
        if (Item == null) return;

        switch (Item.Info.Type)
        {
            /*case ItemType.Weapon:
                SoundManager.PlaySound(SoundList.ClickWeapon);
                break;
            case ItemType.Armour:
                SoundManager.PlaySound(SoundList.ClickArmour);
                break;
            case ItemType.Helmet:
                SoundManager.PlaySound(SoundList.ClickHelmet);
                break;
            case ItemType.Necklace:
                SoundManager.PlaySound(SoundList.ClickNecklace);
                break;
            case ItemType.Bracelet:
                SoundManager.PlaySound(SoundList.ClickBracelet);
                break;
            case ItemType.Ring:
                SoundManager.PlaySound(SoundList.ClickRing);
                break;
            case ItemType.Boots:
                SoundManager.PlaySound(SoundList.ClickBoots);
                break;
            case ItemType.Potion:
                SoundManager.PlaySound(SoundList.ClickDrug);
                break;
            default:
                SoundManager.PlaySound(SoundList.ClickItem);
                break;*/
        }
    }

    private int FreeSpace()
    {
        int count = 0;

        for (int i = 0; i < ItemArray.Length; i++)
            if (ItemArray[i] == null) count++;

        return count;
    }


    private bool CanRemoveItem(UserItem i)
    {
        /*if (MapObject.User.RidingMount && i.Info.Type != ItemType.Torch)
        {
            return false;
        }*/

        //stuck
        return FreeSpace() > 0;
    }

    private bool CorrectSlot(UserItem i)
    {
        ItemType type = i.Info.Type;

        switch ((EquipmentSlot)ItemSlot)
        {
            case EquipmentSlot.Weapon:
                return type == ItemType.Weapon;
            case EquipmentSlot.Armour:
                return type == ItemType.Armour;
            case EquipmentSlot.Helmet:
                return type == ItemType.Helmet;
            case EquipmentSlot.Torch:
                return type == ItemType.Torch;
            case EquipmentSlot.Necklace:
                return type == ItemType.Necklace;
            case EquipmentSlot.BraceletL:
                return i.Info.Type == ItemType.Bracelet;
            case EquipmentSlot.BraceletR:
                return i.Info.Type == ItemType.Bracelet || i.Info.Type == ItemType.Amulet;
            case EquipmentSlot.RingL:
            case EquipmentSlot.RingR:
                return type == ItemType.Ring;
            case EquipmentSlot.Amulet:
                return type == ItemType.Amulet;// && i.Info.Shape > 0;
            case EquipmentSlot.Boots:
                return type == ItemType.Boots;
            case EquipmentSlot.Belt:
                return type == ItemType.Belt;
            case EquipmentSlot.Stone:
                return type == ItemType.Stone;
            case EquipmentSlot.Mount:
                return type == ItemType.Mount;
            default:
                return false;
        }
    }

    private bool CanWearItem(UserItem i)
    {
        if (i == null) return false;

        //If Can remove;

        switch (GameManager.User.Player.Gender)
        {
            case MirGender.Male:
                if (!i.Info.RequiredGender.HasFlag(RequiredGender.Male))
                {
                    GameScene.ChatController.RecieveChat(GameLanguage.NotFemale, ChatType.System);
                    return false;
                }
                break;
            case MirGender.Female:
                if (!i.Info.RequiredGender.HasFlag(RequiredGender.Female))
                {
                    GameScene.ChatController.RecieveChat(GameLanguage.NotMale, ChatType.System);
                    return false;
                }
                break;
        }

        switch (GameManager.User.Player.Class)
        {
            case MirClass.Warrior:
                if (!i.Info.RequiredClass.HasFlag(RequiredClass.Warrior))
                {
                    GameScene.ChatController.RecieveChat("Warriors cannot use this item.", ChatType.System);
                    return false;
                }
                break;
            case MirClass.Wizard:
                if (!i.Info.RequiredClass.HasFlag(RequiredClass.Wizard))
                {
                    GameScene.ChatController.RecieveChat("Wizards cannot use this item.", ChatType.System);
                    return false;
                }
                break;
            case MirClass.Taoist:
                if (!i.Info.RequiredClass.HasFlag(RequiredClass.Taoist))
                {
                    GameScene.ChatController.RecieveChat("Taoists cannot use this item.", ChatType.System);
                    return false;
                }
                break;
            case MirClass.Assassin:
                if (!i.Info.RequiredClass.HasFlag(RequiredClass.Assassin))
                {
                    GameScene.ChatController.RecieveChat("Assassins cannot use this item.", ChatType.System);
                    return false;
                }
                break;
            case MirClass.Archer:
                if (!i.Info.RequiredClass.HasFlag(RequiredClass.Archer))
                {
                    GameScene.ChatController.RecieveChat("Archers cannot use this item.", ChatType.System);
                    return false;
                }
                break;
        }

        switch (i.Info.RequiredType)
        {
            case RequiredType.Level:
                if (GameManager.User.Level < i.Info.RequiredAmount)
                {
                    GameScene.ChatController.RecieveChat(GameLanguage.LowLevel, ChatType.System);
                    return false;
                }
                break;
            /*case RequiredType.MaxAC:
                if (MapObject.User.MaxAC < i.Info.RequiredAmount)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough AC.", ChatType.System);
                    return false;
                }
                break;
            case RequiredType.MaxMAC:
                if (MapObject.User.MaxMAC < i.Info.RequiredAmount)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough MAC.", ChatType.System);
                    return false;
                }
                break;
            case RequiredType.MaxDC:
                if (MapObject.User.MaxDC < i.Info.RequiredAmount)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat(GameLanguage.LowDC, ChatType.System);
                    return false;
                }
                break;
            case RequiredType.MaxMC:
                if (MapObject.User.MaxMC < i.Info.RequiredAmount)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat(GameLanguage.LowMC, ChatType.System);
                    return false;
                }
                break;
            case RequiredType.MaxSC:
                if (MapObject.User.MaxSC < i.Info.RequiredAmount)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat(GameLanguage.LowSC, ChatType.System);
                    return false;
                }
                break;
            case RequiredType.MaxLevel:
                if (MapObject.User.Level > i.Info.RequiredAmount)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("You have exceeded the maximum level.", ChatType.System);
                    return false;
                }
                break;
            case RequiredType.MinAC:
                if (MapObject.User.MinAC < i.Info.RequiredAmount)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough Base AC.", ChatType.System);
                    return false;
                }
                break;
            case RequiredType.MinMAC:
                if (MapObject.User.MinMAC < i.Info.RequiredAmount)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough Base MAC.", ChatType.System);
                    return false;
                }
                break;
            case RequiredType.MinDC:
                if (MapObject.User.MinDC < i.Info.RequiredAmount)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough Base DC.", ChatType.System);
                    return false;
                }
                break;
            case RequiredType.MinMC:
                if (MapObject.User.MinMC < i.Info.RequiredAmount)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough Base MC.", ChatType.System);
                    return false;
                }
                break;
            case RequiredType.MinSC:
                if (MapObject.User.MinSC < i.Info.RequiredAmount)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough Base SC.", ChatType.System);
                    return false;
                }
                break;*/
        }

        /*if (i.Info.Type == ItemType.Weapon || i.Info.Type == ItemType.Torch)
        {
            if (i.Weight - (Item != null ? Item.Weight : 0) + MapObject.User.CurrentHandWeight > MapObject.User.MaxHandWeight)
            {
                GameScene.Scene.ChatDialog.ReceiveChat(GameLanguage.TooHeavyToHold, ChatType.System);
                return false;
            }
        }
        else
        {
            if (i.Weight - (Item != null ? Item.Weight : 0) + MapObject.User.CurrentWearWeight > MapObject.User.MaxWearWeight)
            {
                GameScene.Scene.ChatDialog.ReceiveChat("It is too heavy to wear.", ChatType.System);
                return false;
            }
        }*/

        return true;
    }
}
