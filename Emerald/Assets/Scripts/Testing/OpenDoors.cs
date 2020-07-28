using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoors : MonoBehaviour
{
    public Animator leftdoor, rightdoor, camera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            leftdoor.SetBool("openGate", true);
            rightdoor.SetBool("openGate", true);
            camera.SetBool("openGate", true);
        }  
    }
}