using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserObject : MonoBehaviour
{
    public PlayerObject Player;
    public int WalkStep;

    void Awake()
    {
        GameManager.User = this;
    }

}
