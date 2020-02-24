using UnityEngine;

public class GunEffects : MonoBehaviour
{
    public LineRenderer gunLine;
    public ParticleSystem gunParticles;
    public GameObject impactEffect;

    public void PlayEffects(RaycastHit shootHit)
    {
        // Bullet line
        gunLine.enabled = true;
        gunLine.SetPosition(0, gunLine.transform.position);
        gunLine.SetPosition(1, shootHit.point);

        // Particle effect
        gunParticles.Stop();
        gunParticles.Play();

        // despite if it's a target or not, we try and instantiate an impact effect
        if (impactEffect != null)
        {
            Instantiate(impactEffect, shootHit.point, Quaternion.LookRotation(shootHit.normal));
        }
    }

    public void StopEffects()
    {
        // Bullet line
        gunLine.enabled = false;
    }

}
