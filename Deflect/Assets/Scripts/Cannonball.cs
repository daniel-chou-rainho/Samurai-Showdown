using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
    private Rigidbody rb;
    private Transform target;
    private float speed;
    private float rebound = 1.5f;

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
            rb.velocity = -rb.velocity / rebound;

            // Vibration
            other.gameObject.GetComponent<HapticBlade>().Vibrate(0.1f, 0.1f);

            // SFX
            VelocityEstimator estimator = other.GetComponent<VelocityEstimator>();
            if(estimator)
            {
                // Volume based on blade speed
                float v = estimator.GetVelocityEstimate().magnitude;
                float diff = maxVolume - minVolume;
                double volume = (double)Mathf.InverseLerp(minVelocity, maxVelocity, v) * (double)diff + (double)minVolume;
                Debug.Log(volume);

                // Random Pitch
                source.pitch = Random.Range(minPitch, maxPitch);

                source.PlayOneShot(clip, (float)volume);
            }
        }
    }
}