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

    // Collision Detection
    private bool bladed = false;
    private GameObject blade;
    private GameObject[] bladeCloud;

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

    private void Start()
    {
        blade = GameObject.FindGameObjectsWithTag("Blade")[0];
        bladeCloud = GameObject.FindGameObjectsWithTag("BladeCloud");

        source = GetComponent<AudioSource>();
        gameObject.transform.Rotate(0, 0, 45 * Random.Range(0, 7));

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

    private void FixedUpdate()
    {
        if (bladed) { return; }

        float min = 10.0f;
        foreach(GameObject point in bladeCloud)
        {
            min = Mathf.Min(min, Mathf.Abs(Vector3.Distance(point.transform.position, gameObject.transform.position)));
        }

        if(Mathf.Abs(min) < 0.2f)
        {
            bladed = true;
            Collide();
        }
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
        rotate = false;
        rb.useGravity = true;

        if (other.tag == "Head")
        {
            other.gameObject.GetComponent<Head>().TakeLife();
            Destroy(this.gameObject);
        }
    }

    private void Collide()
    {
        rotate = false;
        rb.useGravity = true;

        // Vibration
        blade.GetComponent<HapticBlade>().Vibrate(0.1f, 0.1f);

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
            Debug.Log(rb.useGravity);
            Vector3 dir = rb.velocity;
            dir = -dir.normalized;
            Debug.Log(vc);
            rb.AddForce(dir * (float)vc * 15);
        }
    }

    //void OnDrawGizmos()
    //{
    //    if(bladeCloud == null) { return; }
    //    foreach (GameObject point in bladeCloud)
    //    {
    //        Gizmos.DrawWireSphere(point.transform.position, 0.1f);
    //    }
    //}
}