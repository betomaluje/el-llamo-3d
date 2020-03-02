using System.Collections;
using UnityEngine;
using DG.Tweening;

public class GunEffects : MonoBehaviour
{
    public LineRenderer gunLine;
    public ParticleSystem gunParticles;
    public GameObject impactEffect;

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
        //StopEffects();
    }

    public void PlaceBullet(Vector3 shootHit)
    {
        // despite if it's a target or not, we try and instantiate an impact effect
        if (impactEffect != null)
        {
            Instantiate(impactEffect, shootHit, Quaternion.LookRotation(shootHit));
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
        yield return new WaitForSeconds(1f);
        StopEffects();
    }

}
