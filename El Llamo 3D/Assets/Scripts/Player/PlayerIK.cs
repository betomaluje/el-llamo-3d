using UnityEngine;
using BetoMaluje.Sikta;

public class PlayerIK : MonoBehaviour
{
    public PlayerGrab playerAim;
    public Transform chest;
    //public Vector3 offset;

    private void LateUpdate()
    {
        chest.LookAt(playerAim.aimPoint);
    }
}