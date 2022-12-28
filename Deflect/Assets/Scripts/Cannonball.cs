using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
    // Type
    private bool ghost = false;
    private float blood = 1.0f;
    public Material ghostMat;
    public Material bloodMat;
    public Collider collider;

    private Rigidbody rb;
    private Transform target;
    private float speed;

    public Transform srk;
    private float dps = 1000; // degrees/sec
    private bool rotate = true;
    private bool bladed = false;
    private bool headed = false;
    private GameObject blade;

    public AudioClip metalClip;
    public AudioClip poofClip;
    public AudioClip whooshClip;
    private AudioSource source;
    private float minVelocity = 1.0f;
    private float maxVelocity = 5.0f;
    private float minVolume = 0.1f;
    private float maxVolume = 0.2f;
    private float minPitch = 0.9f;
    private float maxPitch = 1.1f;

    private void typeSet()
    {
        int rn = Random.Range(1, 6);

        // Blood
        if (rn == 1)
        {
            srk.GetComponent<Renderer>().material = bloodMat;
            blood = 1.5f;
        }

        // Ghost
        if (rn == 2)
        {
            srk.GetComponent<Renderer>().material = ghostMat;
            collider.enabled = false;
            ghost = true;
        }
    }

    private void Start()
    {
        typeSet();
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

    public void Attack(Transform target, float holdTime, float speed)
    {
        this.target = target;
        this.speed = speed * blood;
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
        headed = true;

        // Stop Whoosh SFX
        source.Stop();
        source.loop = false;

        if (other.tag == "Head")
        {
            other.gameObject.GetComponent<Head>().TakeLife();
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // Unique Collision
        if (bladed || ghost) { return; }
        bladed = true;

        rotate = false;
        rb.useGravity = true;

        // Vibration
        blade.GetComponent<HapticBlade>().Vibrate(0.1f * blood, 0.1f * blood);

        VelocityEstimator estimator = blade.GetComponent<VelocityEstimator>();
        if (estimator)
        {
            float v = estimator.GetVelocityEstimate().magnitude;
            double v01 = (double)Mathf.InverseLerp(minVelocity, maxVelocity, v);
            double vc = Mathf.Clamp(v, 2.0f, 5.0f);

            // Velocity-based SFX
            float diff = maxVolume - minVolume;
            double volume = v01 * (double)diff + (double)minVolume;
            source.pitch = Random.Range(minPitch, maxPitch);
            source.PlayOneShot(metalClip, (float)volume * 1.0f);

            // Velocity-based Rebound
            Vector3 dir = rb.velocity;
            dir = -dir.normalized;
            GetComponent<Rigidbody>().AddForce(dir * (float)vc * 15);
        } 
    }
}