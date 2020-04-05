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

    public void SetupPlayer(Transform theTarget)
    {
        target = theTarget;
    }

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

            transform.position = target.position + offset;
        }
    }

    public void ChangeCameraType()
    {
        if (camMode == CameraMode.FirstPerson)
        {
            camMode = CameraMode.ThirdPerson;
        }
        else
        {
            camMode = CameraMode.FirstPerson;
        }
    }

}
