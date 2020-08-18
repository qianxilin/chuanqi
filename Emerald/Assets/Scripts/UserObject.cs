using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserObject : MonoBehaviour
{
    public PlayerObject Player;
    public int WalkStep;
    public UserItem[] Inventory = new UserItem[46];
    public int BeltIdx = 6;
    

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
    }

}
