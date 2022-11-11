using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
    private bool init = false;
    private Rigidbody rb;
    private Transform target;
    private float velocity;
    private float turn = 10;

    public void Attack(Transform target, float holdTime, float velocity)
    {
        this.target = target;
        this.velocity = velocity;
        StartCoroutine(Hold(holdTime));
    }

    private IEnumerator Hold(float holdTime)
    {
        yield return new WaitForSeconds(holdTime);
        rb = gameObject.AddComponent<Rigidbody>();
        init = true;
    }

    private void FixedUpdate()
    {
        if (init)
        {
            rb.velocity = transform.forward * velocity;

            var targetRot = Quaternion.LookRotation(target.position - transform.position);

            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRot, turn));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Head")
        {
            other.gameObject.GetComponent<Head>().TakeLife();
            Destroy(this.gameObject);
        }
    }
}
