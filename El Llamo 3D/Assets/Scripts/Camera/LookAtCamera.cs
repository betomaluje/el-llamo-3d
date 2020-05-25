using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform mainCameraPos;

    private void Awake()
    {
        mainCameraPos = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (mainCameraPos == null)
        {
            return;
        }

        transform.rotation = Quaternion.identity;
        transform.LookAt(transform.position + mainCameraPos.forward);
    }
}
