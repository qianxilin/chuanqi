using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCharacterAnimationManager : MonoBehaviour
{
    public static int IdleCount;

    public void Activate()
    {
       gameObject.GetComponent<AudioSource>().Play();
    }

    public void IncreaseCount()
    {
        IdleCount++;

        if (IdleCount >= 7)
        {
            IdleCount = 0;
            gameObject.GetComponent<Animator>().SetBool("bored", true);
        }
    }
}
