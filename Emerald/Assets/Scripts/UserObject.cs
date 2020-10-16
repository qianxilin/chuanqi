using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserObject : MonoBehaviour
{
    public uint Level;
    public PlayerObject Player;
    public AttackMode AMode;
    public int WalkStep;

    public UserItem[] Inventory = new UserItem[46];
    public UserItem[] Equipment = new UserItem[14];
    public int BeltIdx = 6;

    public List<ClientMagic> Magics = new List<ClientMagic>();


    void Awake()
    {
        GameManager.User = this;
    }

    public void BindAllItems()
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i] == null) continue;            
            GameManager.Bind(Inventory[i]);
        }

        for (int i = 0; i < Equipment.Length; i++)
        {
            if (Equipment[i] == null) continue;
            GameManager.Bind(Equipment[i]);
        }
    }

    public void RefreshStats()
    {
        if (Equipment[(int)EquipmentSlot.Armour] != null)
            Player.Armour = Equipment[(int)EquipmentSlot.Armour].Info.Shape;
    }

}
