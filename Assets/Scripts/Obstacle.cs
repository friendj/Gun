using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public ParticleSystem destroyEffect;

    public void Destroyed(float distance, Vector3 direction, Color color)
    {
        StartCoroutine(BeginDestroy(distance, direction, color));
    }

    IEnumerator BeginDestroy(float distance, Vector3 direction, Color color)
    {
        yield return new WaitForSecondsRealtime(1f);
        ParticleSystem particle = Instantiate(destroyEffect, transform.position, Quaternion.FromToRotation(transform.forward, direction));
        Material particleMat = particle.GetComponent<Renderer>().material;
        Material material = GetComponent<Renderer>().material;  // get when need use
        particleMat.color = material.color;
        Destroy(particle.gameObject, destroyEffect.startLifetime);
        Destroy(gameObject);
    }
}
