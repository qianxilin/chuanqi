using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;

public class PlayerObject : MapObject
{
    [HideInInspector]
    public GameManager gameManager;

    public GameObject Camera;    
    [HideInInspector]
    public MirClass Class;
    [HideInInspector]
    public MirGender Gender;
    [HideInInspector]
    public bool InSafeZone;

    private GameObject ArmourModel;
    private GameObject WeaponModel;
    private GameObject HeadBone;
    private GameObject WeaponRBone;
    private GameObject WeaponBackBone;

    private int weapon = -1;
    public int Weapon
    {
        get { return weapon; }
        set
        {
            if (value == weapon) return;
            weapon = value;
            
            if (WeaponRBone == null) return;

            if (WeaponModel != null)
                Destroy(WeaponModel);

            if (value <= 0) return;

            WeaponModel = Instantiate(gameManager.WeaponModels[value - 1], InSafeZone ? WeaponBackBone.transform : WeaponRBone.transform);
        }
    }


    private int armour = -1;
    public int Armour
    {
        get { return armour; }
        set
        {
            if (value == armour) return;

            armour = value;

            if (ArmourModel != null)
                Destroy(ArmourModel);

            if (value < gameManager.WarriorModels.Count)
                ArmourModel = Instantiate(gameManager.WarriorModels[value], Model.transform);
            else
                ArmourModel = Instantiate(gameManager.WarriorModels[0], Model.transform);

            ArmourModel.GetComponentInChildren<PlayerAnimationController>().ParentObject = this;
            ArmourModel.GetComponentInChildren<Animator>().SetInteger("CurrentAction", (int)CurrentAction);

            ObjectRenderer = ArmourModel.GetComponentInChildren<SkinnedMeshRenderer>();

            foreach (Transform child in ArmourModel.GetComponentsInChildren<Transform>())
            {
                if (child.name == "Bip01-Head")
                {
                    HeadBone = child.gameObject;
                    break;
                }
            }
            Instantiate(gameManager.WarriorFaces[0], HeadBone.transform);

            foreach (Transform child in ArmourModel.GetComponentsInChildren<Transform>())
            {
                if (child.name == "Weapon_R")
                {
                    WeaponRBone = child.gameObject;
                    break;
                }
            }
            foreach (Transform child in ArmourModel.GetComponentsInChildren<Transform>())
            {
                if (child.name == "Weapon_B2")
                {
                    WeaponBackBone = child.gameObject;
                    break;
                }
            }

            if (Weapon > 0)
            {
                if (WeaponModel != null)
                    Destroy(WeaponModel);
                WeaponModel = Instantiate(gameManager.WeaponModels[Weapon - 1], InSafeZone ? WeaponBackBone.transform : WeaponRBone.transform);
            }
        }
    }

    public override void Start()
    {        
        base.Start();
    }

    public override void SetAction()
    {        
        if (this == GameManager.User.Player && GameScene.QueuedAction != null)
        {
            ActionFeed.Clear();
            ActionFeed.Add(GameScene.QueuedAction);
            GameScene.QueuedAction = null;
        }

        if (ActionFeed.Count == 0)
        {           
            CurrentAction = MirAction.Standing;
        }
        else
        {
            if (this == GameManager.User.Player && Time.time < GameManager.NextAction) return;

            QueuedAction action = ActionFeed[0];
            ActionFeed.RemoveAt(0);

            CurrentAction = action.Action;
            Direction = action.Direction;
            Model.transform.rotation = ClientFunctions.GetRotation(Direction);

            switch (CurrentAction)
            {
                case MirAction.Walking:
                case MirAction.Running:
                    int steps = 1;
                    if (CurrentAction == MirAction.Running) steps = 2;

                    Vector3 targetpos = GameManager.CurrentScene.Cells[(int)action.Location.x, (int)action.Location.y].position;                    
                    TargetPosition = targetpos;

                    if (this != GameManager.User.Player)
                    {
                        Vector2 back = ClientFunctions.Back(action.Location, Direction, steps);
                        gameObject.transform.position = GameManager.CurrentScene.Cells[(int)back.x, (int)back.y].position;
                    }

                    GameManager.CurrentScene.Cells[(int)CurrentLocation.x, (int)CurrentLocation.y].RemoveObject(this);
                    GameManager.CurrentScene.Cells[(int)action.Location.x, (int)action.Location.y].AddObject(this);

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
                    case MirAction.Standing:
                        Network.Enqueue(new C.Turn { Direction = action.Direction });
                        GameManager.NextAction = Time.time + 2.5f;
                        GameManager.InputDelay = Time.time + 0.5f;
                        GameManager.User.CanRun = false;
                        break;
                    case MirAction.Walking:
                        Network.Enqueue(new C.Walk { Direction = action.Direction });
                        GameManager.NextAction = Time.time + 2.5f;
                        GameManager.InputDelay = Time.time + 0.5f;
                        GameManager.User.LastRunTime = Time.time;
                        GameManager.User.CanRun = true;
                        break;
                    case MirAction.Running:
                        Network.Enqueue(new C.Run { Direction = action.Direction });
                        GameManager.NextAction = Time.time + 2.5f;
                        GameManager.InputDelay = Time.time + 0.5f;
                        GameManager.User.LastRunTime = Time.time;
                        break;
                    case MirAction.Attack:
                        Network.Enqueue(new C.Attack { Direction = Direction, Spell = Spell.None });
                        GameManager.NextAction = Time.time + 2.5f;
                        break;
                }
            }

            switch (CurrentAction)
            {
                case MirAction.Attack:
                    GetComponentInChildren<Animator>().Play("Attack", -1, normalizedTime: 0f);
                    break;
            }
        }
        GetComponentInChildren<Animator>()?.SetInteger("CurrentAction", (int)CurrentAction);
    }
}
