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
        ObjectRenderer.materials[1].SetFloat("_ASEOutlineWidth", OutlineWidth);
        ObjectRenderer.materials[1].SetColor("_ASEOutlineColor", Color.white);
    }

    public override void OnSelect()
    {
        NameLabel.gameObject.SetActive(true);
        ObjectRenderer.materials[1].SetColor("_ASEOutlineColor", Color.red);
    }

    public override void OnDeSelect()
    {
        NameLabel.gameObject.SetActive(false);
        ObjectRenderer.materials[1].SetColor("_ASEOutlineColor", Color.white);
    }
}
