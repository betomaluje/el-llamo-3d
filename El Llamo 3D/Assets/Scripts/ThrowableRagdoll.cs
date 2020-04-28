using BetoMaluje.Sikta;
using DG.Tweening;
using SWNetwork;
using UnityEngine;

public class ThrowableRagdoll : MonoBehaviour, ITarget
{
    [SerializeField] private Vector3 handsOffset;
    [SerializeField] private Transform parentTransform;

    [Space]
    [Header("Pickup")]
    [SerializeField] private float pickupSpeed = 1f;

    private Rigidbody rb;
    private Collider col;

    #region Grabbing
    private SphereCollider sphereCollider;
    private bool grabbed = false;
    #endregion

    #region Networking
    private RemoteEventAgent remoteEventAgent;

    private const string THROWING = "Throwing";
    private const string PICKUP = "Pickup";
    #endregion

    void Start()
    {
        remoteEventAgent = GetComponentInParent<RemoteEventAgent>();

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
                playerGrab.target = this;
                Transform playerHand = other.gameObject.GetComponentInChildren<Hand>().transform;
                parentTransform.transform.parent = playerHand;
            }
        }
    }

    private void ChangeSettings(bool isTargetGrabbed)
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

    public void StartPickup(Transform playerHand)
    {
        if (grabbed)
        {
            return;
        }

        SWNetworkMessage msg = new SWNetworkMessage();
        // to
        msg.Push(playerHand.transform.position);
        remoteEventAgent.Invoke(PICKUP, msg);
    }

    public void RemotePickupObject(SWNetworkMessage msg)
    {
        Vector3 to = msg.PopVector3();
        Debug.Log("remote pickup ragdoll: " + to);
        Pickup(to);
    }

    public void Pickup(Vector3 to)
    {
        // we enabled the player collision handler
        sphereCollider.enabled = true;

        grabbed = true;

        ChangeSettings(true);

        transform.localPosition = Vector3.zero;

        Sequence s = DOTween.Sequence();
        s.Append(parentTransform.DOMove(to, pickupSpeed));
        s.AppendCallback(() => parentTransform.DOLocalMove(handsOffset, pickupSpeed));
        s.AppendCallback(() => transform.DOLocalRotate(Vector3.zero, pickupSpeed));
        s.AppendCallback(() =>
        {
            SoundManager.instance.Play("Pickup");

            parentTransform.localPosition = handsOffset;

            sphereCollider.enabled = false;

            Debug.Log("finish pickup " + parentTransform.gameObject.name);
        });
    }

    public void Throw(float throwForce, Vector3 direction)
    {
        if (!grabbed)
        {
            return;
        }

        SWNetworkMessage msg = new SWNetworkMessage();
        msg.Push(direction);
        msg.Push(throwForce);
        remoteEventAgent.Invoke(THROWING, msg);
    }

    public void RemoteThrow(SWNetworkMessage msg)
    {
        Vector3 direction = msg.PopVector3();
        float throwForce = msg.PopFloat();

        grabbed = false;

        if (transform.parent != null)
        {
            PlayerGrab playerGrab = parentTransform.GetComponentInParent<PlayerGrab>();
            if (playerGrab != null)
            {
                Debug.Log("PlayerGrab null target");
                playerGrab.target = null;
            }
        }

        SoundManager.instance.Play("Throw");

        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMove(parentTransform.position - parentTransform.forward, .01f));
        s.AppendCallback(() => ChangeSettings(false));
        s.AppendCallback(() => parentTransform.parent = null);
        s.AppendCallback(() => rb.AddForce(direction * throwForce, ForceMode.Impulse));
        s.AppendCallback(() => rb.AddTorque(parentTransform.transform.right + parentTransform.transform.up * throwForce, ForceMode.Impulse));

        Debug.Log("finish throwing " + parentTransform.gameObject.name);
    }

    public void Shoot(Vector3 shootHit)
    {
        // not implemented here        
    }

    public TargetType getType()
    {
        return TargetType.Throwable;
    }

    public int GetDamage()
    {
        return 0;
    }

    public bool isGrabbed()
    {
        return grabbed;
    }
}
