using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimationController : MonoBehaviour
{
    [HideInInspector]
    public MonsterObject ParentObject;

    void SetAction()
    {
        ParentObject?.SetAction();
    }
}
