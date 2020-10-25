using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MapObject
{
    public int Image;

    public override void Start()
    {
        base.Start();
        Blocking = false;
        NameLabel.gameObject.SetActive(false);
    }

    public override void OnSelect()
    {
        base.OnSelect();
        NameLabel.gameObject.SetActive(true);
    }

    public override void OnDeSelect()
    {
        base.OnDeSelect();
        NameLabel.gameObject.SetActive(false);
    }
}
