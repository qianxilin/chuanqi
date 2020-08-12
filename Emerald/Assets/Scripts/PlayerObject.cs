using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    public GameObject Camera;
    public GameObject Model;
    [Range(0f, 10f)]
    public float MoveSpeed;
    //[HideInInspector]
    public bool IsMoving;
    //[HideInInspector]
    public Vector3 TargetPosition;
    //[HideInInspector]
    public Vector3 StartPosition;
    //[HideInInspector]
    public float TargetDistance;

    [HideInInspector]
    public MirClass Class;
    [HideInInspector]
    public MirGender Gender;
    [HideInInspector]
    public Vector2 CurrentLocation;

    void Start()
    {
    }

    void Update()
    {
        if (IsMoving)
        {
            if (Vector3.Distance(StartPosition, transform.position) > TargetDistance)
            {
                transform.position = TargetPosition;
                IsMoving = false;
                return;
            }
            transform.position += (TargetPosition - StartPosition) * MoveSpeed * Time.deltaTime;
        }
    }
}
