using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    public GameObject Camera;
    public GameObject Model;

    [HideInInspector]
    public MirClass Class;
    [HideInInspector]
    public MirGender Gender;
    [HideInInspector]
    public Vector2 CurrentLocation;

    void Start()
    {
    }

    void LateUpdate()
    {
    }
}
