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
        }
    }
}