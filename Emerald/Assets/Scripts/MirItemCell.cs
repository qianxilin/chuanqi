using System;
using UnityEngine;
using UnityEngine.UI;

public class MirItemCell : MonoBehaviour
{
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
}
