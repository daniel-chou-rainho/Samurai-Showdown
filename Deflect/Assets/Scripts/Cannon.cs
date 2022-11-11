using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public Transform muzzle;
    private float clockAngle;

    private void Start()
    {
        clockAngle = 0;
    }

    public Transform GetMuzzle()
    {
        return muzzle;
    }

    /// Rotates the transform to an angle on the clock.
    public void Rotate(Vector3 axis, float angle, float duration)
    {
        // Find smallest angle btw current and final angle
        float rot = Mathf.DeltaAngle(clockAngle, angle);

        // Rotate
        StartCoroutine(RotateAroundAxis(axis, rot, duration));

        // Update the clockangle
        clockAngle = angle;
    }

    // DMGregory https://bit.ly/3exAkeq
    protected IEnumerator RotateAroundAxis(Vector3 axis, float angle, float duration)
    {
        Vector3 startPoint = transform.position;

        float progress = 0f;
        float rate = 1f / duration;

        while (progress < 1f)
        {
            progress += Time.deltaTime * rate;
            Quaternion rotation = Quaternion.Euler(0f, angle * Mathf.Min(progress, 1f), 0f);
    
            // If your output is a Vector2, this will implicitly omit the z.
            transform.position = axis + (rotation * (startPoint - axis));

            // Face Pivot
            Quaternion lookRot = Quaternion.LookRotation(axis - transform.position);
            lookRot.x = 0;
            lookRot.z = 0;
            transform.rotation = lookRot;

            yield return null;
        }
    }
}
