using DG.Tweening;
using UnityEngine;
using BetoMaluje.Sikta;

public class ThrowableRagdoll : MonoBehaviour, ITarget
    {
        private Rigidbody rb;
        private Collider col;

        private Transform parentTransform;
        private bool isGrabbed = false;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            parentTransform = GetComponentInParents<Transform>(gameObject);
            Debug.Log("parent is " + parentTransform.gameObject.name);
        }

        private void LateUpdate()
        {
            if (isGrabbed) 
            {
                parentTransform.localPosition = Vector3.zero;
            }
        }

        private void ChangeSettings(bool isTargetDead)
        {
            if (parentTransform.parent != null)
            {
                return;
            }

            rb.isKinematic = isTargetDead;
            rb.interpolation = isTargetDead ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
            col.isTrigger = isTargetDead;
            rb.useGravity = isTargetDead;
        }

        public void Pickup(PlayerGrab playerGrab, Transform weaponHolder)
        {
            Debug.Log("Picking up " + gameObject.name);
            playerGrab.target = this;
            isGrabbed = true;
            ChangeSettings(true);
    
            parentTransform.transform.parent = weaponHolder;

            transform.DOLocalMove(Vector3.zero, .25f).SetEase(Ease.OutBack).SetUpdate(true);
            transform.DOLocalRotate(Vector3.zero, .25f).SetUpdate(true);
        }

        public void Throw(float throwForce)
        {
            SoundManager.instance.Play("Throw");
            isGrabbed = false;

            Sequence s = DOTween.Sequence();
            s.Append(transform.DOMove(transform.position - transform.forward, .01f)).SetUpdate(true);
            s.AppendCallback(() => parentTransform.parent = null);
            s.AppendCallback(() => ChangeSettings(false));
            s.AppendCallback(() => rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse));
            s.AppendCallback(() => rb.AddTorque(transform.transform.right + transform.transform.up * throwForce, ForceMode.Impulse));
        }

        public T GetComponentInParents<T>(GameObject startObject) where T : Component
        {
            T returnObject = null;
            GameObject currentObject = startObject;
            while(!returnObject)
            {
                if (currentObject == currentObject.transform.root) return null;
                currentObject = (GameObject) currentObject.transform.parent.gameObject;
                returnObject = currentObject.GetComponent<T>();
            }
            return returnObject;
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
