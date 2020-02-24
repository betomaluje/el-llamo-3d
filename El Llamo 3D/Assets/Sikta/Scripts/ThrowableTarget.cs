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

        private void ChangeSettings(bool isTargetDead)
        {
            if (transform.parent != null)
                return;

            rb.isKinematic = isTargetDead;
            rb.interpolation = isTargetDead ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
            col.isTrigger = isTargetDead;
        }
    
        public void Pickup(PlayerGrab playerGrab, Transform weaponHolder)
        {
            playerGrab.target = this;
            ChangeSettings(true);

            transform.parent = weaponHolder;

            transform.DOLocalMove(Vector3.zero, .25f).SetEase(Ease.OutBack).SetUpdate(true);
            transform.DOLocalRotate(Vector3.zero, .25f).SetUpdate(true);
        }

        public void Throw(float throwForce)
        {
            Sequence s = DOTween.Sequence();
            s.Append(transform.DOMove(transform.position - transform.forward, .01f)).SetUpdate(true);
            s.AppendCallback(() => transform.parent = null);
            s.AppendCallback(() => ChangeSettings(false));
            s.AppendCallback(() => rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse));
            s.AppendCallback(() => rb.AddTorque(transform.transform.right + transform.transform.up * throwForce, ForceMode.Impulse));
        }

        public void Shoot(RaycastHit shootHit)
        {
            // not implemented here        
        }

        public TargetType getType()
        {
            return TargetType.Throwable;
        }

    }
}

