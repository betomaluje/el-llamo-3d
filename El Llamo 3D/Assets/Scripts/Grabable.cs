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
                Transform playerHand = playerGrab.GetActiveHand();
                if (playerHand != null) 
                {
                    getParentTransform().parent = playerHand;
                    //playerGrab.ChangeHand();
                }

                sphereCollider.enabled = false;
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

    public void StartPickup(Vector3 playerHand)
    {
        if (grabState.Equals(GrabState.Grabbing))
        {
            return;
        }

        grabState = GrabState.Grabbing;

        if (GameSettings.instance.usingNetwork)
        {
            SWNetworkMessage msg = new SWNetworkMessage();
            // to
            msg.Push(playerHand);
            remoteEventAgent.Invoke(PICKUP, msg);
        } 
        else 
        {
            Debug.Log("local pickup " + gameObject.name + ": " + playerHand);
            LocalPickUp(playerHand);
        }       
    }

    private void LocalPickUp(Vector3 to) 
    {    
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

    public void RemotePickupObject(SWNetworkMessage msg)
    {    
        Vector3 to = msg.PopVector3();

        Debug.Log("remote pickup " + gameObject.name + ": " + to);

        LocalPickUp(to);
    }

    public abstract void Pickup(Vector3 to);

    public void StartThrow(float throwForce, Vector3 direction)
    {
        if (grabState.Equals(GrabState.Throwing))
        {
            return;
        }

        grabState = GrabState.Throwing;

        if (GameSettings.instance.usingNetwork)
        {
            SWNetworkMessage msg = new SWNetworkMessage();
            msg.Push(direction);
            msg.Push(throwForce);
            remoteEventAgent.Invoke(THROWING, msg);
        } 
        else 
        {
            Debug.Log("local throw " + gameObject.name + ": " + direction + " force: " + throwForce);
            LocalThrowObject(direction, throwForce);
        }
    }

    private void LocalThrowObject(Vector3 direction, float throwForce) 
    {
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

    public void RemoteThrowObject(SWNetworkMessage msg)
    {
        if (grabState.Equals(GrabState.Idle))
        {
            return;
        }

        Vector3 direction = msg.PopVector3();
        float throwForce = msg.PopFloat();

        Debug.Log("remote throw " + gameObject.name + ": " + direction + " force: " + throwForce);

        LocalThrowObject(direction, throwForce);
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

    void StartPickup(Vector3 playerHand);

    void Pickup(Vector3 to);

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