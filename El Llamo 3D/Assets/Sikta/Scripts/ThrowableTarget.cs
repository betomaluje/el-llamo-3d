using UnityEngine;
using DG.Tweening;

namespace BetoMaluje.Sikta
{
    public class ThrowableTarget : MonoBehaviour, ITarget
    {    
        private Rigidbody rb;
        private Collider col;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
        }

        void ChangeSettings()
        {
            if (transform.parent != null)
                return;

            rb.isKinematic = (PlayerGrab.instance.weapon == this) ? true : false;
            rb.interpolation = (PlayerGrab.instance.weapon == this) ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
            col.isTrigger = (PlayerGrab.instance.weapon == this);
        }
    
        public void Pickup(Transform weaponHolder)
        {        
            PlayerGrab.instance.weapon = this;
            ChangeSettings();

            transform.parent = weaponHolder;

            transform.DOLocalMove(Vector3.zero, .25f).SetEase(Ease.OutBack).SetUpdate(true);
            transform.DOLocalRotate(Vector3.zero, .25f).SetUpdate(true);
        }

        public void Throw(float throwForce)
        {
            Debug.Log("throwing target " + throwForce);
            Sequence s = DOTween.Sequence();
            s.Append(transform.DOMove(transform.position - transform.forward, .01f)).SetUpdate(true);
            s.AppendCallback(() => transform.parent = null);
            s.AppendCallback(() => ChangeSettings());
            s.AppendCallback(() => rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse));
            s.AppendCallback(() => rb.AddTorque(transform.transform.right + transform.transform.up * throwForce, ForceMode.Impulse));
        }

        public void Shoot()
        {
            // not implemented here        
        }

        public TargetType getType()
        {
            return TargetType.Throwable;
        }

    }
}

