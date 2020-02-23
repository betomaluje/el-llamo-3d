using UnityEngine;
using DG.Tweening;

namespace BetoMaluje.Sikta
{
    public class GunTarget : MonoBehaviour, ITarget
    {
        [SerializeField] private Transform shootingPosition;
        [SerializeField] private KeyCode bulletChangerKey;
        [SerializeField] private GameObject[] bulletPrefabs;
        [SerializeField] private float shootingSpeed = 100f;

        public float impactForce = 100f;

        [SerializeField] private ParticleSystem shootingParticles;

        [SerializeField] private int maxDamage = 30;

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

        private void ChangeSettings(bool isTargetDead)
        {         
            rb.isKinematic = isTargetDead;
            rb.interpolation = isTargetDead ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
            col.isTrigger = isTargetDead;
        }

        public void Pickup(PlayerGrab playerGrab, Transform weaponHolder)
        {
            playerGrab.target = this;
            ChangeSettings(true);

            transform.parent = weaponHolder;

            Vector3 rotation = new Vector3(-90, 0, 90);

            transform.DOLocalMove(Vector3.zero, .25f).SetEase(Ease.OutBack).SetUpdate(true);
            transform.DOLocalRotate(rotation, .25f).SetUpdate(true);
        }

        public void Throw(float throwForce)
        {
            Sequence s = DOTween.Sequence();
            s.Append(transform.DOMove(transform.position - transform.forward, .01f)).SetUpdate(true);
            s.AppendCallback(() => ChangeSettings(false));
            s.AppendCallback(() => transform.parent = null);            
            s.AppendCallback(() => rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse));
            s.AppendCallback(() => rb.AddTorque(transform.transform.right + transform.transform.up * throwForce, ForceMode.Impulse));
        }

        public void Shoot()
        {
            // we can shoot here
            shootingParticles.Play();
            /*
            GameObject bullet = Instantiate(bulletPrefab, shootingPosition.position, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            rb.AddForce(shootingPosition.right * bulletScript.GetShootingSpeed(), ForceMode.Impulse);
            */
        }

        public TargetType getType()
        {
            return TargetType.Shootable;
        }

        public int GetDamage() 
        {
            return Random.Range(1, maxDamage);
        }

        private void Update() 
        {
            isAttachedToPlayer = transform.parent != null;

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

            ChangeBulletByIndex(currentBullet);       
        }

        private void ChangeBulletByIndex(int index)
        {
            bulletPrefab = bulletPrefabs[currentBullet];
            Debug.Log("Change to bullet " + bulletPrefab.name);
        }
    }
}