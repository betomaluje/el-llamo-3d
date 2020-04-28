using BetoMaluje.Sikta;
using DG.Tweening;
using SWNetwork;
using System;
using UnityEngine;

public abstract class Gun : MonoBehaviour, ITarget
{
    [Header("Stats")]
    public Transform shootingPosition;
    [SerializeField] private float fireRate = 3f;
    [SerializeField] private int maxDamage = 30;
    public float impactForce = 100f;

    [Space]
    [Header("Recoil")]
    [SerializeField] private float recoilAmount = 0.3f;
    [SerializeField] private float recoilDuration = 0.3f;

    [Space]
    [Header("Pickup")]
    [SerializeField] private float speed = 0.25f;

    private float nextTimeToFire = 0f;
    private Rigidbody rb;
    private Collider col;
    private SphereCollider sphereCollider;

    public Action OnPickedUp;

    private bool grabbed = false;

    #region Networking
    private RemoteEventAgent remoteEventAgent;

    public const string THROWING = "Throwing";
    public const string PICKUP = "Pickup";
    #endregion

    public void Start()
    {
        remoteEventAgent = GetComponent<RemoteEventAgent>();

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
                transform.parent = playerHand;
            }
        }
    }

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

    public void StartPickup(Transform playerHand)
    {
        SWNetworkMessage msg = new SWNetworkMessage();
        // to
        msg.Push(playerHand.transform.position);
        remoteEventAgent.Invoke(PICKUP, msg);
    }

    public void RemotePickupObject(SWNetworkMessage msg)
    {
        Vector3 to = msg.PopVector3();

        Debug.Log("remote pickup object: " + to);

        Pickup(to);
    }

    public void Pickup(Vector3 to)
    {
        Vector3 rotation = new Vector3(-90, 0, 90);

        // we enabled the player collision handler
        sphereCollider.enabled = true;

        grabbed = true;

        ChangeSettings(true);

        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMove(to, speed));
        s.AppendCallback(() => transform.DOLocalRotate(rotation, speed));
        s.AppendCallback(() =>
        {
            SoundManager.instance.Play("Pickup");

            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.Euler(rotation);

            sphereCollider.enabled = false;

            Debug.Log("finish pickup " + gameObject.name);
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

        grabbed = false;

        if (transform.parent != null)
        {
            PlayerGrab playerGrab = transform.parent.GetComponentInParent<PlayerGrab>();
            if (playerGrab != null)
            {
                Debug.Log("PlayerGrab null target");
                playerGrab.target = null;
            }
        }

        SoundManager.instance.Play("Throw");

        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMove(transform.position - transform.forward, .01f));
        s.AppendCallback(() => ChangeSettings(false));
        s.AppendCallback(() => transform.parent = null);
        s.AppendCallback(() => rb.AddForce(direction * throwForce, ForceMode.Impulse));
        s.AppendCallback(() => rb.AddTorque(transform.transform.right + transform.transform.up * throwForce, ForceMode.Impulse));

        Debug.Log("finish throwing " + gameObject.name);
    }

    public void Shoot(Vector3 shootHit)
    {
        if (Time.time >= nextTimeToFire)
        {
            // we can shoot here
            DoRecoil(shootHit);

            DoShooting(shootHit);
            // we update the frequency of the shooting
            nextTimeToFire = Time.time + 1f / fireRate;
        }
    }

    protected abstract void DoShooting(Vector3 shootHit);

    public void DoRecoil(Vector3 shootHit)
    {
        Vector3 direction = -shootingPosition.right * recoilAmount;
        transform.DOPunchPosition(direction, recoilDuration, 1, 0.05f);
    }

    public TargetType getType()
    {
        return TargetType.Shootable;
    }

    public int GetDamage()
    {
        return UnityEngine.Random.Range(1, maxDamage);
    }

    public bool isGrabbed()
    {
        return grabbed;
    }
}
