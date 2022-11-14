using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HapticBlade : MonoBehaviour
{
    public XRBaseController con;

    public void Vibrate(float amplitude, float duration)
    {
        if (con == null)
            return;

        con.SendHapticImpulse(amplitude, duration);
    }
}
