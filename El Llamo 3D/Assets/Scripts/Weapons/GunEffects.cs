using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GunEffects : MonoBehaviour
{
    [SerializeField] private float lineDuration = 0.25f;
    [SerializeField] private LineRenderer gunLine;
    [SerializeField] private ParticleSystem gunParticles;
    [SerializeField] private GameObject impactEffect;

    public void PlayParticles()
    {
        // Particle effect
        gunParticles.Stop();
        gunParticles.Play();
    }

    public void PlayEffects(Vector3 shootHit)
    {
        // Bullet line
        gunLine.enabled = true;
        gunLine.SetPosition(0, gunLine.transform.position);
        gunLine.SetPosition(1, shootHit);

        StartCoroutine(StartStoppingEffects());
    }

    public void PlaceImpactEffect(Vector3 shootHit)
    {
        // despite if it's a target or not, we try and instantiate an impact effect
        if (impactEffect != null)
        {
            if (shootHit != Vector3.zero)
            {
                Instantiate(impactEffect, shootHit, Quaternion.LookRotation(shootHit));
            }
            else
            {
                Instantiate(impactEffect, shootHit, Quaternion.identity);
            }
        }
    }

    public void StopEffects()
    {
        // Bullet line
        gunLine.enabled = false;
        Color black = Color.black;
        black.a = 0;
        gunLine.material.DOColor(black, 2f);
    }

    private IEnumerator StartStoppingEffects()
    {
        yield return new WaitForSeconds(lineDuration);
        StopEffects();
    }

}
