using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
    public Transform srk;
    private GameObject blade;
    private Rigidbody rb;
    private Transform target;
    private float speed;

    private float dps = 1000; // degrees/sec
    private bool rotate = false;
    private bool bladed = false;
    private bool headed = false;

    // VFX
    public ParticleSystem impact;

    // SFX
    public AudioClip metalClip;
    public AudioClip poofClip;
    public AudioClip whooshClip;
    private AudioSource source;
    private float minVolume = 0.1f;
    private float maxVolume = 0.2f;
    private float minPitch = 0.9f;
    private float maxPitch = 1.1f;

    private void Start()
    {
        blade = GameObject.FindGameObjectsWithTag("Blade")[0];
        source = GetComponent<AudioSource>();
        srk.Rotate(0, 0, 45 * Random.Range(0, 7));

        // Smoke Poof SFX
        source.pitch = Random.Range(minPitch, maxPitch);
        source.volume = 0.8f;
        source.clip = poofClip;
        source.Play();
    }

    private void Update()
    {
        if (!rotate) { return; }
        srk.Rotate(0, dps * Time.deltaTime, 0);
    }

    public void Attack(Transform target, float holdTime, float speed, bool rotate)
    {
        this.target = target;
        this.speed = speed;
        this.rotate = rotate;
        StartCoroutine(Hold(holdTime));
    }

    private IEnumerator Hold(float holdTime)
    {
        yield return new WaitForSeconds(holdTime);
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = 0.1f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        Fly();
    }

    private void Fly()
    {
        rb.velocity = (target.position - transform.position).normalized * speed;
        Destroy(this.gameObject, 4.0f);

        // Whoosh SFX
        if(!rotate) { return; }
        source.loop = true;
        source.clip = whooshClip;
        source.volume = Random.Range(minVolume, maxVolume);
        source.pitch = Random.Range(minPitch, maxPitch);
        source.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Unique Collision
        if (headed) { return; }

        if (other.tag == "Head")
        {
            headed = true;
            other.gameObject.GetComponent<Head>().TakeLife();
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // Unique Collision
        if (bladed) { return; }
        bladed = true;

        rotate = false;
        rb.useGravity = true;

        // Haptics
        blade.GetComponent<HapticBlade>().Vibrate(0.1f, 0.1f);

        // SFX
        source.Stop();
        source.loop = false;
        source.PlayOneShot(metalClip, Random.Range(0.7f, 0.9f));

        // VFX
        impact.Play();
        
        // Rebound
        VelocityEstimator estimator = blade.GetComponent<VelocityEstimator>();
        if (estimator)
        {
            float v = estimator.GetVelocityEstimate().magnitude;
            double vc = Mathf.Clamp(v, 2.0f, 5.0f);

            Vector3 dir = rb.velocity;
            dir = -dir.normalized;
            GetComponent<Rigidbody>().AddForce(dir * (float)vc * 15);
        } 
    }
}