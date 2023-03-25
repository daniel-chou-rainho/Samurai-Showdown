using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Statics
{
    public static int startRound { get; set; }
    public static float musicVolume { get; set; }

    static Statics()
    {
        // init values
        startRound = 0;
        musicVolume = 1;
    }
}
