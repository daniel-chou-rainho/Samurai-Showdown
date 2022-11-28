using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
    private Rigidbody rb;
    private Transform target;
    private float speed;

    public Transform srk;
    private float dps = 1000; // degrees/sec
    private bool rotate = true;

    public AudioClip metalClip;
    public AudioClip poofClip;
    public AudioClip whooshClip;
    private AudioSource source;
    private float minVelocity = 1.0f;
    private float maxVelocity = 5.0f;
    private float minVolume = 0.05f;
    private float maxVolume = 0.1f;
    private float minPitch = 0.9f;
    private float maxPitch = 1.1f;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        srk.Rotate(0, 0, 45 * Random.Range(0, 7));

        // Smoke Poof SFX
        source.pitch = Random.Range(minPitch, maxPitch);
        source.volume = Random.Range(0.4f, 0.6f);
        source.clip = poofClip;
        source.Play();
    }

    private void Update()
    {
        if (!rotate) { return; }
        srk.Rotate(0, dps * Time.deltaTime, 0);
    }

    public void Attack(Transform target, float holdTime, float speed)
    {
        this.target = target;
        this.speed = speed;
        StartCoroutine(Hold(holdTime));
    }

    private IEnumerator Hold(float holdTime)
    {
        yield return new WaitForSeconds(holdTime);
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = 0.1f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Fly();
    }

    private void Fly()
    {
        rb.velocity = (target.position - transform.position).normalized * speed;
        Destroy(this.gameObject, 4.0f);

        // Whoosh SFX
        source.loop = true;
        source.clip = whooshClip;
        source.volume = Random.Range(minVolume, maxVolume);
        source.pitch = Random.Range(minPitch, maxPitch);
        source.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Stop Whoosh SFX
        source.Stop();
        source.loop = false;

        if (other.tag == "Head")
        {
            other.gameObject.GetComponent<Head>().TakeLife();
            Destroy(this.gameObject);
        }

        if (other.tag == "Blade")
        {
            rotate = false;
            rb.useGravity = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Blade")
        {
            // Vibration
            other.gameObject.GetComponent<HapticBlade>().Vibrate(0.1f, 0.1f);

            VelocityEstimator estimator = other.gameObject.GetComponent<VelocityEstimator>();
            if (estimator)
            {
                float v = estimator.GetVelocityEstimate().magnitude;

                // Velocity-based SFX
                float diff = maxVolume - minVolume;
                double volume = (double)Mathf.InverseLerp(minVelocity, maxVelocity, v) * (double)diff + (double)minVolume;
                source.pitch = Random.Range(minPitch, maxPitch);
                source.PlayOneShot(metalClip, (float)volume);

                // Velocity-based Rebound
                Vector3 dir = rb.velocity;
                dir = -dir.normalized;
                GetComponent<Rigidbody>().AddForce(dir * v * 8);
            }
        }
    }
}