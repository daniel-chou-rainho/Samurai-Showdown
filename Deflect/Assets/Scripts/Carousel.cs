using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carousel : MonoBehaviour
{
    // General
    public Transform centerObject;
    private Vector3 center;
    public Transform target;
    public GameObject Cannonball;
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
    }

    public void StartGame()
    {
        if (running) { return; }
        running = true;

        StartCoroutine(Shuffle(10, 3f, 5));
    }

    private IEnumerator Shuffle(int count, float interval, int positions)
    {
        AngleDivision(positions, -50.0f, 50.0f);

        for (int i = 0; i < count; i++)
        {
            foreach (Cannon[] level in Cannons)
            {
                List<float> AnglesAvailable = new List<float>(Angles);

                foreach (Cannon c in level)
                {
                    int rng = Random.Range(0, AnglesAvailable.Count);
                    float newAngle = AnglesAvailable[rng];
                    AnglesAvailable.RemoveAt(rng);

                    c.Rotate(center, newAngle, interval);
                }
            }

            // Wait until rotation finish
            yield return new WaitForSeconds(interval);

            Attack(1, 0.25f, 7f);
            yield return new WaitForSeconds(0.5f);
        }

        Reset(interval);
    }

    // Min = -180
    // Max = 180
    private void AngleDivision(int count, float minAngle, float maxAngle)
    {
        float unit = (maxAngle - minAngle) / count;

        for (int i = 0; i < count; i++)
        {
            Angles.Add(minAngle + i * unit);
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
    private void Attack(int count, float holdTime, float velocity)
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
            cb.Attack(target, holdTime, velocity);
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
