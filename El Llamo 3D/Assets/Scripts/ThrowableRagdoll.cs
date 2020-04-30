using DG.Tweening;
using SWNetwork;
using UnityEngine;

public class ThrowableRagdoll : Grabable
{
    [SerializeField] private Transform parentTransform;

    private void FixedUpdate()
    {
        if (getParentTransform().parent != null)
        {        
            parentTransform.localPosition = Vector3.zero;
        }
    }

    public override TargetType getTargetType()
    {
        return TargetType.Throwable;
    }

    public override void Pickup(Vector3 to)
    {
        Vector3 rotation = new Vector3(0, 0, -90);
        transform.localPosition = Vector3.zero;

        Sequence s = DOTween.Sequence();
        s.Append(parentTransform.DOMove(to, pickupSpeed));
        s.AppendCallback(() => parentTransform.DORotate(rotation, pickupSpeed));
        s.AppendCallback(() =>
        {
            SoundManager.instance.Play("Pickup");

            parentTransform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;

            sphereCollider.enabled = false;

            Debug.Log("finish pickup " + parentTransform.gameObject.name);
        });
    }

    public override void Throw(float throwForce, Vector3 direction)
    {
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMove(parentTransform.position - parentTransform.forward, .01f));
        s.AppendCallback(() => ChangeSettings(false));
        s.AppendCallback(() => parentTransform.parent = null);
        s.AppendCallback(() => rb.AddForce(direction * throwForce, ForceMode.Impulse));
        s.AppendCallback(() => rb.AddTorque(parentTransform.transform.right + parentTransform.transform.up * throwForce, ForceMode.Impulse));

        Debug.Log("finish throwing " + parentTransform.gameObject.name);
    }

    protected override Transform getParentTransform()
    {
        return parentTransform;
    }

    protected override RemoteEventAgent getRemoteEventAgent()
    {
        return GetComponentInParent<RemoteEventAgent>();
    }
}
