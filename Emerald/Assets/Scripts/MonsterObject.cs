using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;

public class MonsterObject : MapObject
{
    public override void SetAction()
    {
        if (ActionFeed.Count == 0)
        {
            CurrentAction = MirAction.Standing;
            if (this == GameManager.User.Player)
                GameManager.User.WalkStep = 0;
        }
        else
        {
            if (Time.time < GameManager.NextAction) return;

            QueuedAction action = ActionFeed[0];
            ActionFeed.RemoveAt(0);

            CurrentAction = action.Action;
            Direction = action.Direction;

            switch (CurrentAction)
            {
                case MirAction.Walking:
                case MirAction.Running:
                    int steps = 1;
                    if (CurrentAction == MirAction.Running) steps = 2;

                    Vector3 targetpos = GameManager.CurrentScene.Cells[(int)action.Location.x, (int)action.Location.y].position;
                    Model.transform.rotation = ClientFunctions.GetRotation(Direction);
                    TargetPosition = targetpos;

                    Vector2 back = ClientFunctions.Back(action.Location, Direction, steps);
                    gameObject.transform.position = GameManager.CurrentScene.Cells[(int)back.x, (int)back.y].position;

                    StartPosition = gameObject.transform.position;
                    TargetDistance = Vector3.Distance(transform.position, targetpos);
                    IsMoving = true;                    
                    break;
            }

            CurrentLocation = action.Location;
        }
        GetComponentInChildren<Animator>().SetInteger("CurrentAction", (int)CurrentAction);
    }
}
