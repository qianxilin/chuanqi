using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirScene : MonoBehaviour
{
    public GameObject UserObject;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.CurrentScene = this;
    }
}
