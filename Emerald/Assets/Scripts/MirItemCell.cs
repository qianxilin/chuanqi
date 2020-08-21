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
            ItemImage.color = HideColor;
            return;
        }
        ItemImage.color = VisibleColor;
        ItemImage.sprite = Resources.Load<Sprite>($"UI/Items/{Item.Info.Image}");
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
                    }
                    break;
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
}
