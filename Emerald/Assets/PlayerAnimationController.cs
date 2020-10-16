using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [HideInInspector]
    public PlayerObject ParentObject;

    void SetAction()
    {
        ParentObject?.SetAction();
    }
}
