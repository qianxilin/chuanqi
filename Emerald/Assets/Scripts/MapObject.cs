using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;

public class MapObject : MonoBehaviour
{
    public GameObject Model;
    [Range(0f, 10f)]
    public float MoveSpeed;
    [HideInInspector]
    public bool IsMoving;
    //[HideInInspector]
    public Vector3 TargetPosition;
    [HideInInspector]
    public Vector3 StartPosition;
    [HideInInspector]
    public float TargetDistance;

    [HideInInspector]
    public uint ObjectID;
    [HideInInspector]
    public Vector2 CurrentLocation;
    [HideInInspector]
    public MirDirection Direction;
    [HideInInspector]
    public List<QueuedAction> ActionFeed = new List<QueuedAction>();
   // [HideInInspector]
    public MirAction CurrentAction;
    [HideInInspector]
    public int ActionType;

    void Start()
    {
        CurrentAction = MirAction.Standing;
        GetComponentInChildren<Animator>().SetInteger("StateAction", 0);
    }

    void Update()
    {
        if (CurrentAction == MirAction.Standing)
        {
            SetAction();
            return;
        }

        if (IsMoving)
        {
            if (Vector3.Distance(StartPosition, transform.position) > TargetDistance)
            {
                transform.position = TargetPosition;
                IsMoving = false;
                SetAction();
                return;
            }

            transform.position += (TargetPosition - StartPosition) * MoveSpeed * Time.deltaTime;
        }
    }

    public virtual void SetAction()
    {
    }
}
