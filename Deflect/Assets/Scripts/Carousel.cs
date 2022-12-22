using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Carousel : MonoBehaviour
{
    // General
    public Transform centerObject;
    private Vector3 center;
    public Transform target;
    public GameObject Cannonball;
    public TextMeshProUGUI scoreBoard;
    private bool running = false;

    // Levels
    public Cannon[] L1Cannons;
    public Cannon[] L2Cannons;
    public Cannon[] L3Cannons;

    // Private Lists
    private List<Cannon[]> Cannons = new List<Cannon[]>();
    private List<Transform> Muzzles = new List<Transform>();
    private List<float> Angles = new List<float>();

    // FOR DEBUGGING
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            StartGame();
        }
    }

    void Start()
    {
        CannonCollection();
        MuzzleCollection();
        center = centerObject.position;
        scoreBoard.text = Statics.startRound.ToString();
    }

    public void StartGame()
    {
        if (running) { return; }
        running = true;

        StartCoroutine(Dealer(50));
    }

    private IEnumerator Dealer(int rounds)
    {
        int nMin = 1, nMax = 3;
        int pMin = 5, pMax = 7;
        int xMin = 2, xMax = 5;
        float tMin = 1.50f, tMax = 0.25f;
        float aMin = 60.0f, aMax = 90.0f;
        float sMin = 5.0f, sMax = 10.0f;
        float hMin = 1.0f, hMax = 0.75f;
        float bMin = 1.0f, bMax = 0.75f;

        // Complete Reset
        if (Statics.startRound > rounds)
            Statics.startRound = 0;

        for (int r = Statics.startRound; r < rounds; r++)
        {
            // Difficulty Scaling
            int n = LinScaleInt(nMin, nMax, r, rounds);
            int p = LinScaleInt(pMin, pMax, r, rounds);
            int x = LinScaleInt(xMin, xMax, r, rounds);
            float t = LinScaleFloat(tMin, tMax, r, rounds);
            float a = LinScaleFloat(aMin, aMax, r, rounds);
            float s = LinScaleFloat(sMin, sMax, r, rounds);
            float h = LinScaleFloat(hMin, hMax, r, rounds); ;
            float b = LinScaleFloat(bMin, bMax, r, rounds); ;

            yield return StartCoroutine(Shuffle(n, t, p, a, x, s, h, b));
            Statics.startRound++;
            scoreBoard.text = Statics.startRound.ToString();
        }
    }

    private int LinScaleInt(int min, int max, int round, int rounds)
    {
        double progress = (double)round / (double)rounds;
        double val = progress * ((double)max - (double)min) + (double)min;
        return Mathf.RoundToInt((float)val);
    }

    private float LinScaleFloat(float min, float max, int round, int rounds)
    {
        double progress = (double)round / (double)rounds;
        double val = progress * ((double)max - (double)min) + (double)min;
        return (float)val;
    }

    /// <summary>
    /// Shuffle the cannons <b><i>n</i></b> times. <br/>
    /// Each shuffle takes <b><i>t</i></b> seconds. <br/>
    /// There are <b><i>p</i></b> positions in the shuffle. <br/>
    /// The positions are across an angle of <b><i>a</i></b> degrees. <br/>
    /// Each shuffle is followed by <b><i>x</i></b> attacks, each with speed <b><i>s</i></b>. <br/>
    /// The time between the creation of 2 cannonballs is <b><i>b</i></b> seconds. <br/>
    /// After the cannonball is created, the attack is launched after <b><i>h</i></b> seconds.
    /// </summary>
    /// <param name="n"></param>
    /// <param name="t"></param>
    /// <param name="p"></param>
    /// <param name="a"></param>
    /// <param name="x"></param>
    /// <param name="s"></param>
    /// <param name="h"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private IEnumerator Shuffle(int n, float t, int p, float a, int x, float s, float h, float b)
    {
        AngleDivision(p, a);

        for (int i = 0; i < n; i++)
        {
            foreach (Cannon[] level in Cannons)
            {
                List<float> AnglesAvailable = new List<float>(Angles);

                foreach (Cannon c in level)
                {
                    int rng = Random.Range(0, AnglesAvailable.Count);
                    float newAngle = AnglesAvailable[rng];
                    AnglesAvailable.RemoveAt(rng);

                    c.Rotate(center, newAngle, t);
                }
            }

            // Wait until rotation finish
            yield return new WaitForSeconds(t);

            // Attack Sequence
            yield return StartCoroutine(Attack(x, h, b, s));
        }

        Reset(t);
        yield return new WaitForSeconds(t + 1.0f);
    }

    // Min = -180, Max = 180
    private void AngleDivision(int count, float angle)
    {
        float unit = angle / count;

        Angles.Clear();
        for (int i = 0; i < count + 1; i++)
        {
            Angles.Add(-angle/2 + i * unit);
        }
    }

    /// Rotate each cannon back to its starting position.
    private void Reset(float duration)
    {
        foreach (Cannon[] level in Cannons)
        {
            foreach (Cannon c in level)
            {
                c.Rotate(center, 0f, duration);
            }
        }  
    }

    // Attack
    private IEnumerator Attack(int count, float holdTime, float period, float speed)
    {
        List<Transform> MuzzlesAvailable = new List<Transform>(Muzzles);

        for (int i = 0; i < count; i++)
        {
            // Get Random Muzzle
            int rng1 = Random.Range(0, MuzzlesAvailable.Count);
            Transform muzzle = MuzzlesAvailable[rng1];
            MuzzlesAvailable.RemoveAt(rng1);

            // Create Cannonball
            var ball = Instantiate(Cannonball, muzzle.position, muzzle.rotation);

            // Launch Cannonball
            Cannonball cb = (Cannonball) ball.GetComponent(typeof(Cannonball));
            cb.Attack(target, holdTime, speed);

            yield return new WaitForSeconds(period);
        }
    }

    private void CannonCollection()
    {
        Cannons.Add(L1Cannons);
        Cannons.Add(L2Cannons);
        Cannons.Add(L3Cannons);
    }

    private void MuzzleCollection()
    {
        foreach (Cannon[] level in Cannons)
        {
            foreach (Cannon c in level)
            {
                Muzzles.Add(c.GetMuzzle());
            }
        }
    }
}
