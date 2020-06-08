using SWNetwork;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class Grabable : LocalGrabable
{
    #region Networking
    private RemoteEventAgent remoteEventAgent;

    public const string THROWING = "Throwing";
    public const string PICKUP = "Pickup";
    #endregion

    protected override void Start()
    {
        remoteEventAgent = getRemoteEventAgent();
        sphereCollider = GetComponent<SphereCollider>();
        base.Start();
    }

    protected abstract RemoteEventAgent getRemoteEventAgent();

    override public void StartPickup(Vector3 playerHand)
    {
        if (grabState.Equals(GrabState.Grabbing))
        {
            return;
        }

        grabState = GrabState.Grabbing;

        SWNetworkMessage msg = new SWNetworkMessage();
        // to
        msg.Push(playerHand);
        remoteEventAgent.Invoke(PICKUP, msg);
    }

    public void RemotePickupObject(SWNetworkMessage msg)
    {
        Vector3 to = msg.PopVector3();

        Debug.Log("remote pickup " + gameObject.name + ": " + to);

        if (grabState.Equals(GrabState.Grabbed))
        {
            return;
        }

        grabState = GrabState.Grabbed;

        // we enabled the player collision handler
        sphereCollider.enabled = true;

        grabbed = true;

        ChangeSettings(true);

        Pickup(to);
    }

    override public void StartThrow(float throwForce, Vector3 direction)
    {
        if (grabState.Equals(GrabState.Throwing))
        {
            return;
        }

        grabState = GrabState.Throwing;

        SWNetworkMessage msg = new SWNetworkMessage();
        msg.Push(direction);
        msg.Push(throwForce);
        remoteEventAgent.Invoke(THROWING, msg);
    }

    public void RemoteThrowObject(SWNetworkMessage msg)
    {
        if (grabState.Equals(GrabState.Idle))
        {
            return;
        }

        Vector3 direction = msg.PopVector3();
        float throwForce = msg.PopFloat();

        Debug.Log("remote throw " + gameObject.name + ": " + direction + " force: " + throwForce);

        grabbed = false;

        if (transform.parent != null)
        {
            PlayerGrab playerGrab = getParentTransform().GetComponentInParent<PlayerGrab>();
            if (playerGrab != null)
            {
                playerGrab.RemoveGrabable();
            }
        }

        SoundManager.instance.Play("Throw");

        Throw(throwForce, direction);

        grabState = GrabState.Idle;
    }
}