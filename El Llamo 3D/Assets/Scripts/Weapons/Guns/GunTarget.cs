using UnityEngine;

public class GunTarget : Gun
{
    public GunEffects gunEffects;

    protected override void DoShooting(Vector3 shootHit)
    {
        SoundManager.instance.Play("Shoot");

        gunEffects.PlayParticles();
        gunEffects.PlayEffects(shootHit);
        gunEffects.PlaceImpactEffect(shootHit);
    }
}