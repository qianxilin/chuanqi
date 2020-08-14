using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserObject : MonoBehaviour
{
    public PlayerObject Player; 

    void Awake()
    {
        GameManager.User = this;
    }
}
