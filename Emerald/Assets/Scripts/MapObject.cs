using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Network = EmeraldNetwork.Network;
using C = ClientPackets;

public class MapObject : MonoBehaviour
{
    public GameSceneManager GameScene
    {
        get { return GameManager.GameScene; }
    }
    public Renderer ObjectRenderer;
    public GameObject NameLabelObject;
    public Transform NameLocation;
    [HideInInspector]
    public TMP_Text NameLabel;
    public GameObject Model;
    [Range(0f, 10f)]
    public float MoveSpeed;
    [Range(0f, 10f)]
    public float OutlineWidth;

    public string Name;
    public int Light;
    [HideInInspector]
    public bool Dead;

    private byte percentHealth;
    public byte PercentHealth
    {
        get { return percentHealth; }
        set
        {
            if (percentHealth == value) return;
            percentHealth = value;

            if (this != GameManager.User.Player) return;

            GameScene.HPGlobe.material.SetFloat("_Percent", 1 - value / 100F);
        }
    }
    public long HealthTime;

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

    public virtual void Start()
    {
        CurrentAction = MirAction.Standing;
        ObjectRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        NameLabel = Instantiate(NameLabelObject, NameLocation.position, Quaternion.identity, gameObject.transform).GetComponent<TMP_Text>();
        SetNameLabel();
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
            var distance = (TargetPosition - StartPosition) * MoveSpeed * Time.deltaTime;
            var newpos = transform.position + distance;

            if (Vector3.Distance(StartPosition, newpos) > TargetDistance)
            {
                transform.position = TargetPosition;
                IsMoving = false;
                SetAction();
                return;
            }

            transform.position = newpos;
        }
    }

    public virtual void SetAction()
    {
    }

    public void SetNameLabel()
    {
        NameLabel.text = Name;
    }
}
