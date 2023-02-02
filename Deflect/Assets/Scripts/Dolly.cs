using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dolly : MonoBehaviour
{
    void Update()
    {
        transform.position -= transform.forward * Time.deltaTime * 1.0f;
    }
}
