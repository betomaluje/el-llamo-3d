using UnityEngine;

public class LayerMaskUtils
{
    public static bool LayerMatchesObject(LayerMask layer, GameObject gameObject)
    {
        return ((1 << gameObject.gameObject.layer) & layer) != 0;
    }
}
