using BetoMaluje.Sikta;
using DG.Tweening;
using SWNetwork;
using UnityEngine;

public class ThrowableRagdoll : MonoBehaviour, ITarget
{
    [SerializeField] private Vector3 handsOffset;
    [SerializeField] private Transform parentTransform;

    private Rigidbody rb;
    private Collider col;

    private bool grabbed = false;

    #region Networking
    private RemoteEventAgent remoteEventAgent;

    private const string THROWING = "Throwing";
    private const string PICKUP = "Pickup";
    #endregion

    void Start()
    {
        remoteEventAgent = GetComponentInParent<RemoteEventAgent>();

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void ChangeSettings(bool isTargetDead)
    {
        if (rb == null || col == null)
        {
            return;
        }

        rb.isKinematic = isTargetDead;
        rb.useGravity = !isTargetDead;
        col.isTrigger = isTargetDead;
    }

    public void StartPickup(Transform playerHand, PlayerGrab playerGrab, Vector3 from)
    {
        playerGrab.target = this;
        parentTransform.transform.parent = playerHand;

        SWNetworkMessage msg = new SWNetworkMessage();
        // from
        msg.Push(from);
        // to
        msg.Push(playerHand.transform.position);
        remoteEventAgent.Invoke(PICKUP, msg);
    }

    public void RemotePickupObject(SWNetworkMessage msg)
    {
        Debug.Log("remote pickup ragdoll");
        Vector3 from = msg.PopVector3();
        Vector3 to = msg.PopVector3();
        Pickup(from, to);
    }

    public void Pickup(Vector3 from, Vector3 to)
    {
        grabbed = true;

        transform.localPosition = Vector3.zero;

        Sequence s = DOTween.Sequence();
        s.AppendCallback(() => ChangeSettings(true));
        s.AppendCallback(() => parentTransform.DOMove(to, 0.25f));
        s.AppendCallback(() => parentTransform.DOLocalMove(handsOffset, .25f));
        s.AppendCallback(() => transform.DOLocalRotate(Vector3.zero, .25f));
        s.AppendCallback(() =>
        {
            SoundManager.instance.Play("Pickup");

            Debug.Log("finish pickup ragdoll " + parentTransform.gameObject.name);
        });
    }

    public void Throw(float throwForce, Vector3 direction)
    {
        SWNetworkMessage msg = new SWNetworkMessage();
        msg.Push(direction);
        msg.Push(throwForce);
        remoteEventAgent.Invoke(THROWING, msg);
    }

    public void RemoteThrow(SWNetworkMessage msg)
    {
        Vector3 direction = msg.PopVector3();
        float throwForce = msg.PopFloat();

        PlayerGrab playerGrab = parentTransform.parent.GetComponentInParent<PlayerGrab>();
        if (playerGrab != null)
        {
            Debug.Log("PlayerGrab null target");
            playerGrab.target = null;
        }

        SoundManager.instance.Play("Throw");
        grabbed = false;

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
