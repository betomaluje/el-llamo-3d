using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public enum CameraMode
    {
        FirstPerson,
        ThirdPerson
    }
    public Transform target;
    public CameraMode camMode = CameraMode.ThirdPerson;

    public Vector3 FirstPersonOffset;
    public Vector3 ThirdPersonOffset;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 offset = Vector3.zero;

            switch (camMode)
            {
                case CameraMode.FirstPerson:
                    offset = FirstPersonOffset;
                    break;
                case CameraMode.ThirdPerson:
                    offset = ThirdPersonOffset;
                    break;
            }

            //Vector3 newPosition = target.TransformPoint(offset);
            transform.position = target.position + offset;
        }
    }
}
