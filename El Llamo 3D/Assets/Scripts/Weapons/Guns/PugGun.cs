using UnityEngine;

public class PugGun : Gun
{
    [SerializeField] private KeyCode bulletChangerKey;
    [SerializeField] private GameObject[] bulletPrefabs;
    [SerializeField] private float shootingSpeed = 100f;

    public GunEffects gunEffects;

    private int currentBullet = 0;
    private GameObject bulletPrefab;
    private bool isAttachedToPlayer = false;

    private void Start()
    {
        base.Start();
        bulletPrefab = bulletPrefabs[currentBullet];
    }

    protected override void DoShooting(Vector3 shootHit)
    {
        SoundManager.instance.Play("Shoot");

        gunEffects.PlayParticles();
        gunEffects.PlayEffects(shootHit);

        GameObject bullet = Instantiate(bulletPrefab, shootingPosition.position, Quaternion.LookRotation(shootHit - shootingPosition.position));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        rb.AddForce(shootingPosition.forward * bulletScript.GetShootingSpeed(), ForceMode.Impulse);
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
