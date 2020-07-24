using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class LocalGrabable : MonoBehaviour, IGrab
{
    [Space]
    [Header("Pickup")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] protected float pickupSpeed = 0.25f;

    protected Rigidbody rb;
    private Collider col;
    protected SphereCollider sphereCollider;

    protected bool grabbed = false;

    protected GrabState grabState = GrabState.Idle;

    protected virtual void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        grabState = GrabState.Idle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (grabbed && LayerMaskUtils.LayerMatchesObject(targetLayer, other.gameObject))
        {
            GrabController grabController = other.gameObject.transform.root.GetComponentInChildren<GrabController>(true);
            if (grabController != null)
            {
                grabController.AddGrabable(this, getParentTransform());
                sphereCollider.enabled = false;
                col.isTrigger = false;
            }
        }
    }

    protected abstract Transform getParentTransform();

    protected void ChangeSettings(bool isTargetGrabbed)
    {
        if (rb == null || col == null)
        {
            return;
        }

        EnemyGrab enemyGrab = getParentTransform().GetComponent<EnemyGrab>();
        if (enemyGrab != null)
        {
            enemyGrab.DoRagdoll(isTargetGrabbed);
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

    public virtual void StartPickup(Vector3 playerHand)
    {
        if (grabState.Equals(GrabState.Grabbing))
        {
            return;
        }

        grabState = GrabState.Grabbing;

        Debug.Log("local pickup " + gameObject.name + ": " + playerHand);
        LocalPickUp(playerHand);
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

    public abstract void Pickup(Vector3 to);

    public virtual void StartThrow(float throwForce, Vector3 direction)
    {
        if (grabState.Equals(GrabState.Throwing))
        {
            return;
        }

        grabState = GrabState.Throwing;

        Debug.Log("local throw " + gameObject.name + ": " + direction + " force: " + throwForce);
        LocalThrowObject(direction, throwForce);
    }

    private void LocalThrowObject(Vector3 direction, float throwForce)
    {
        grabbed = false;

        if (transform.parent != null)
        {
            GrabController grabController = getParentTransform().GetComponentInParent<GrabController>();
            if (grabController != null)
            {
                grabController.RemoveGrabable(this);
            }
        }

        SoundManager.instance.Play("Throw");

        Throw(throwForce, direction);

        grabState = GrabState.Idle;
    }

    public abstract void Throw(float throwForce, Vector3 direction);

    public bool IsGrabbed()
    {
        return grabbed && (grabState == GrabState.Grabbing || grabState == GrabState.Grabbed);
    }

    public abstract TargetType getTargetType();

    public abstract bool ShouldChangeOutline();
}

[SerializeField]
public interface IGrab
{
    TargetType getTargetType();

    void StartPickup(Vector3 playerHand);

    void Pickup(Vector3 to);

    void StartThrow(float throwForce, Vector3 direction);

    void Throw(float throwForce, Vector3 direction);

    bool IsGrabbed();
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