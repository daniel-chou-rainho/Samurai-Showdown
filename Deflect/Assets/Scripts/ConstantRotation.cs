using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    private float dps = 750;

    void Update()
    {
        transform.Rotate(0, dps * Time.deltaTime, 0);
    }
}
