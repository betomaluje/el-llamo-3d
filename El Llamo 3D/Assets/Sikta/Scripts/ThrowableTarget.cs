using DG.Tweening;
using UnityEngine;

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
            {
                return;
            }

            rb.isKinematic = isTargetDead;
            rb.interpolation = isTargetDead ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
            rb.useGravity = isTargetDead;
            col.isTrigger = isTargetDead;
        }

        public void Pickup(PlayerGrab playerGrab, Transform weaponHolder)
        {
            Debug.Log("Picking up " + gameObject.name);
            playerGrab.target = this;
            ChangeSettings(true);

            transform.parent = weaponHolder;

            transform.DOLocalMove(Vector3.zero, .25f).SetEase(Ease.OutBack).SetUpdate(true);
            transform.DOLocalRotate(Vector3.zero, .25f).SetUpdate(true);
        }

        public void Throw(float throwForce, Vector3 direction)
        {
            SoundManager.instance.Play("Throw");

            Sequence s = DOTween.Sequence();
            s.Append(transform.DOMove(transform.position - transform.forward, .01f)).SetUpdate(true);
            s.AppendCallback(() => transform.parent = null);
            s.AppendCallback(() => ChangeSettings(false));
            s.AppendCallback(() => rb.AddForce(direction * throwForce, ForceMode.Impulse));
            s.AppendCallback(() => rb.AddTorque(transform.transform.right + transform.transform.up * throwForce, ForceMode.Impulse));
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

    }
}

