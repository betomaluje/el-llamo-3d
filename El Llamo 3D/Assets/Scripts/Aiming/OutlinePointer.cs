using EPOOutline;
using UnityEngine;

public class OutlinePointer
{
    private float finalValue = 2f;
    private bool shouldBlur = false;
    private Outlinable lastTarget = null;
    private float updateSpeedSeconds = 0.2f;

    public void PointToTarget(GameObject gameObject)
    {
        Outlinable outlinable = gameObject.GetComponentInChildren<Outlinable>(true);
        if (outlinable != null)
        {
            // if the object is different, we first unpoint the previous object
            if (lastTarget != outlinable)
            {
                UnPointToTarget();
            }

            lastTarget = outlinable;

            ChangeValues(0f, finalValue);
        }
    }

    public void UnPointToTarget()
    {
        if (lastTarget != null)
        {
            ChangeValues(finalValue, 0f);

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

            while (elapsed < updateSpeedSeconds)
            {
                elapsed += Time.deltaTime;
                float value = Mathf.Lerp(prePercentage, targetValue, elapsed / updateSpeedSeconds);
                lastTarget.DilateShift = value;
                if (shouldBlur)
                    lastTarget.BlurShift = value;
            }

            lastTarget.DilateShift = to;

            if (shouldBlur)
                lastTarget.BlurShift = to;
        }
    }

}
