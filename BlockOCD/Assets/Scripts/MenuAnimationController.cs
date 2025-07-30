using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimationController : MonoBehaviour
{
    public Animator menuAnimator;


    public void PlayAnimation(string triggerName)
    {
        menuAnimator.SetTrigger(triggerName);
    }
}
