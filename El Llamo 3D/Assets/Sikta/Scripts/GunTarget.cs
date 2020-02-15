using UnityEngine;
using DG.Tweening;

namespace BetoMaluje.Sikta
{
    public class GunTarget : MonoBehaviour, ITarget
    {
        [SerializeField] private Transform shootingPosition;
        [SerializeField] private KeyCode bulletChangerKey;
        [SerializeField] private GameObject[] bulletPrefabs;
        [SerializeField] private float shootingSpeed = 100;

        private Rigidbody rb;
        private Collider col;

        private int currentBullet = 0;
        private GameObject bulletPrefab;
        private bool isAttachedToPlayer = false;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            bulletPrefab = bulletPrefabs[currentBullet];
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

            Vector3 rotation = new Vector3(-90, 0, 90);

            transform.DOLocalMove(Vector3.zero, .25f).SetEase(Ease.OutBack).SetUpdate(true);
            transform.DOLocalRotate(rotation, .25f).SetUpdate(true);
        }

        public void Throw(float throwForce)
        {
            Sequence s = DOTween.Sequence();
            s.Append(transform.DOMove(transform.position - transform.forward, .01f)).SetUpdate(true);
            s.AppendCallback(() => transform.parent = null);
            s.AppendCallback(() => ChangeSettings());
            s.AppendCallback(() => rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse));
            s.AppendCallback(() => rb.AddTorque(transform.transform.right + transform.transform.up * throwForce, ForceMode.Impulse));
        }

        public void Shoot()
        {
            // we can shoot here
            GameObject bullet = Instantiate(bulletPrefab, shootingPosition.position, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(shootingPosition.right * shootingSpeed, ForceMode.Impulse);
        }

        public TargetType getType()
        {
            return TargetType.Shootable;
        }

        private void Update() 
        {
            isAttachedToPlayer = (PlayerGrab.instance.weapon == this);

            if (isAttachedToPlayer && Input.GetKeyDown(bulletChangerKey)) 
            {
                ChangeBullet();
            }    
        }

        private void ChangeBullet() 
        {
            currentBullet++;

            if (currentBullet >= bulletPrefabs.Length) 
            {
                currentBullet = 0;
            }

            bulletPrefab = bulletPrefabs[currentBullet];
        }
    }
}