using BetoMaluje.Sikta;
using SWNetwork;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(RemoteEventAgent))]
public abstract class Grabable : MonoBehaviour, IGrab
{
    [Space]
    [Header("Pickup")]
    [SerializeField] protected float pickupSpeed = 0.25f;

    protected Rigidbody rb;
    private Collider col;
    protected SphereCollider sphereCollider;

    private bool grabbed = false;

    private GrabState grabState = GrabState.Idle;

    #region Networking
    private RemoteEventAgent remoteEventAgent;

    public const string THROWING = "Throwing";
    public const string PICKUP = "Pickup";
    #endregion

    private void Start()
    {
        remoteEventAgent = getRemoteEventAgent();
        sphereCollider = GetComponent<SphereCollider>();

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (grabbed && other.gameObject.CompareTag("Player"))
        {
            PlayerGrab playerGrab = other.gameObject.GetComponent<PlayerGrab>();
            if (playerGrab != null)
            {
                playerGrab.AddGrabable(this);
                Transform playerHand = other.gameObject.GetComponentInChildren<Hand>().transform;
                getParentTransform().parent = playerHand;
            }
        }
    }

    protected abstract Transform getParentTransform();

    protected abstract RemoteEventAgent getRemoteEventAgent();

    protected void ChangeSettings(bool isTargetGrabbed)
    {
        if (rb == null || col == null)
        {
            return;
        }

        rb.isKinematic = isTargetGrabbed;
        rb.useGravity = !isTargetGrabbed;
        col.isTrigger = isTargetGrabbed;

        if (isTargetGrabbed)
        {
            // freeze all positions and rotations except the X rotation
            rb.constraints = RigidbodyConstraints.FreezeRotationY |
                RigidbodyConstraints.FreezeRotationZ |
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezePositionZ;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    public void StartPickup(Vector3 playerHand, Vector3 localPosition)
    {
        if (grabState.Equals(GrabState.Grabbing))
        {
            return;
        }

        grabState = GrabState.Grabbing;

        SWNetworkMessage msg = new SWNetworkMessage();
        // to
        msg.Push(playerHand);
        // player hand local position
        msg.Push(localPosition);
        remoteEventAgent.Invoke(PICKUP, msg);
    }

    public void RemotePickupObject(SWNetworkMessage msg)
    {
        if (grabState.Equals(GrabState.Grabbed))
        {
            return;
        }

        grabState = GrabState.Grabbed;

        Vector3 to = msg.PopVector3();
        Vector3 localPosition = msg.PopVector3();

        // we enabled the player collision handler
        sphereCollider.enabled = true;

        grabbed = true;

        ChangeSettings(true);

        Debug.Log("remote pickup " + gameObject.name + ": " + to);

        Pickup(to, localPosition);
    }

    public abstract void Pickup(Vector3 to, Vector3 localPosition);

    public void StartThrow(float throwForce, Vector3 direction)
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
                playerGrab.RemoveGrabable(this);
            }
        }

        SoundManager.instance.Play("Throw");

        Throw(throwForce, direction);

        grabState = GrabState.Idle;
    }

    public abstract void Throw(float throwForce, Vector3 direction);

    public bool isGrabbed()
    {
        return grabbed;
    }

    public abstract TargetType getTargetType();
}

[SerializeField]
public interface IGrab
{
    TargetType getTargetType();

    void StartPickup(Vector3 playerHand, Vector3 localPosition);

    void Pickup(Vector3 to, Vector3 localPosition);

    void StartThrow(float throwForce, Vector3 direction);

    void Throw(float throwForce, Vector3 direction);

    bool isGrabbed();
}

[SerializeField]
public enum TargetType
{
    Throwable, Shootable
}

[SerializeField]
public enum GrabState
{
    Idle, Grabbing, Grabbed, Throwing
}