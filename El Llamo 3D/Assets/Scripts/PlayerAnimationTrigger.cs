using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
{
    [HideInInspector]
    public Action throwTriggeredCallback;

    [HideInInspector]
    public Action throwDoneCallback;

    public void ThrowObject() 
    {
        throwTriggeredCallback();        
    }

    public void ResetThrow()
    {
        throwDoneCallback();
    }

}
