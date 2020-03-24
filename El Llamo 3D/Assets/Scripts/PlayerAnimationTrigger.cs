using System;
using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
{
    [HideInInspector]
    public Action throwTriggeredCallback = delegate { };

    [HideInInspector]
    public Action throwDoneCallback = delegate { };

    public void ThrowObject()
    {
        throwTriggeredCallback();
    }

    public void ResetThrow()
    {
        throwDoneCallback();
    }

}
