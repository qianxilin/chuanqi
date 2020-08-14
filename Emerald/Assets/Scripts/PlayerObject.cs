using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;

public class PlayerObject : MonoBehaviour
{
    public GameObject Camera;
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
    public MirClass Class;
    [HideInInspector]
    public MirGender Gender;
    [HideInInspector]
    public Vector2 CurrentLocation;
    [HideInInspector]
    public List<QueuedAction> ActionFeed = new List<QueuedAction>();
    //[HideInInspector]
    public MirAction CurrentAction;

    void Start()
    {
        CurrentAction = MirAction.Standing;
    }

    void Update()
    {       
        if (CurrentAction == MirAction.Standing)
        {
            SetAction();
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
            transform.position += (TargetPosition - StartPosition) * MoveSpeed * (int)CurrentAction * Time.deltaTime;
        }
    }

    void SetAction()
    {
        if (ActionFeed.Count == 0)
        {
            CurrentAction = MirAction.Standing;
        }
        else
        {
            if (Time.time < GameManager.NextAction) return;

            QueuedAction action = ActionFeed[0];
            ActionFeed.RemoveAt(0);

            CurrentAction = action.Action;

            switch (CurrentAction)
            {
                case MirAction.Walking:
                    Vector3 targetpos = GameManager.CurrentScene.Cells[(int)action.Location.x, (int)action.Location.y].position;
                    Vector3 lookpos = new Vector3(targetpos.x, Model.transform.position.y, targetpos.z);
                    Model.transform.LookAt(lookpos);
                    TargetPosition = targetpos;
                    StartPosition = gameObject.transform.position;
                    TargetDistance = Vector3.Distance(transform.position, targetpos);
                    IsMoving = true;                    
                    break;
                case MirAction.Running:
                    targetpos = GameManager.CurrentScene.Cells[(int)action.Location.x, (int)action.Location.y].position;
                    lookpos = new Vector3(targetpos.x, Model.transform.position.y, targetpos.z);
                    Model.transform.LookAt(lookpos);
                    TargetPosition = targetpos;
                    StartPosition = gameObject.transform.position;
                    TargetDistance = Vector3.Distance(transform.position, targetpos);
                    IsMoving = true;
                    break;
            }

            CurrentLocation = action.Location;

            if (this == GameManager.User.Player)
            {
                switch (CurrentAction)
                {
                    case MirAction.Walking:
                        Network.Enqueue(new C.Walk { Direction = action.Direction });
                        GameManager.NextAction = Time.time + 2.5f;
                        GameManager.InputDelay = Time.time + 0.5f;
                        break;
                }
            }
        }
        GetComponentInChildren<Animator>().SetInteger("CurrentAction", (int)CurrentAction);
    }
}

public class QueuedAction
{
    public MirAction Action;
    public Vector2 Location;
    public MirDirection Direction;
    public List<object> Params;
}
