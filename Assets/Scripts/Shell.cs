using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody rb;
    public float forceMin;
    public float forceMax;

    public float lifeTime = 4f;
    public float fadeTime = 3f;

    void Start()
    {
        float force = Random.Range(forceMin, forceMax);
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.right * force);
        rb.AddTorque(Random.insideUnitSphere * force);

        //StartCoroutine(Fade());
        Destroy(gameObject, lifeTime);
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifeTime);

        float percent = 0;
        float fadeSpeed = 1 / fadeTime;

        Material material = GetComponent<Renderer>().material;
        Color baseColor = material.color;

        while (percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            material.color = Color.Lerp(baseColor, Color.clear, percent);
            yield return null;
        }
        Destroy(gameObject);
    }
}
