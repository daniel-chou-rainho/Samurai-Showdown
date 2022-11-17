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

    public AudioClip clip;
    private AudioSource source;
    public float minVelocity = 0f;
    public float maxVelocity = 2.5f;
    public float minVolume = 0.05f;
    public float maxVolume = 0.1f;
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        srk.Rotate(0, 0, 45 * Random.Range(0, 7));
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
    }

    private void OnTriggerEnter(Collider other)
    {
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
                source.PlayOneShot(clip, (float)volume);

                // Velocity-based Rebound
                Vector3 dir = rb.velocity;
                dir = -dir.normalized;
                GetComponent<Rigidbody>().AddForce(dir * v * 8);
            }
        }
    }
}