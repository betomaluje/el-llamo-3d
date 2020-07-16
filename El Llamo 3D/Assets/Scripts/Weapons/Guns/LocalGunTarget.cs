using UnityEngine;

public class LocalGunTarget : LocalGun
{
    [SerializeField] private GunEffects gunEffects;
    [SerializeField] private bool displayLine = false;

    protected override void DoShooting(Vector3 shootHit, Transform shootingPosition)
    {
        SoundManager.instance.Play("Shoot");

        gunEffects.PlayParticles();

        if (displayLine)
        {
            gunEffects.PlayEffects(shootHit);
        }

        gunEffects.PlaceImpactEffect(shootHit);
    }
}