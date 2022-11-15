using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Head : MonoBehaviour
{
    public int lifes;
    public Fader fader;

    public AudioClip gong;
    public AudioClip taiko;
    private AudioSource source;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void TakeLife()
    {
        source.pitch = Random.Range(minPitch, maxPitch);

        if (lifes == 1){
            source.PlayOneShot(gong);
            StartCoroutine(EndGame());
        } else {
            source.PlayOneShot(taiko);
            lifes--;
        }
    }

    private IEnumerator EndGame()
    {
        Debug.Log("You died.");

        // Slow Motion
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(0.2f); // 1s

        // Fade
        fader.FadeOut(0.2f);
        yield return new WaitForSeconds(0.2f);

        // Normal Motion
        Time.timeScale = 1.0f;

        // Reload Level
        SceneManager.LoadScene(0);
    }
}
