using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;

public class OutlinePointer
{
    private Outlinable lastTarget = null;
    private float updateSpeedSeconds = 0.2f;

    public void PointToTarget(GameObject gameObject)
    {
        Outlinable outlinable = gameObject.GetComponentInChildren<Outlinable>(true);
        if (outlinable != null)
        {
            lastTarget = outlinable;

            ChangeValues(0f, 2f);
        }
    }

    public void UnPointToTarget()
    {
        if (lastTarget != null)
        {
            ChangeValues(2f, 0f);

            lastTarget = null;
        }
    }

    private void ChangeValues(float from, float to)
    {
        if (lastTarget != null)
        {
            float prePercentage = from;
            float elapsed = 0f;
            float targetValue = to;
            float value = 0;

            while (elapsed < updateSpeedSeconds)
            {
                elapsed += Time.deltaTime;
                value = Mathf.Lerp(prePercentage, targetValue, elapsed / updateSpeedSeconds);
                lastTarget.DilateShift = value;
                lastTarget.BlurShift = value;
            }

            lastTarget.DilateShift = to;
            lastTarget.BlurShift = to;
        }
    }

}
