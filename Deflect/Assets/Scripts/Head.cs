using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class Head : MonoBehaviour
{
    public int lifes;
    public Fader fader;
    private bool died;

    // SFX
    public AudioClip gong;
    public AudioClip taiko;
    private AudioSource source;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    // Haptics
    public XRBaseController leftCon;
    public XRBaseController rightCon;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void TakeLife()
    {
        source.pitch = Random.Range(minPitch, maxPitch);
        leftCon.SendHapticImpulse(0.3f, 0.2f);
        rightCon.SendHapticImpulse(0.3f, 0.2f);

        if (lifes == 1){
            source.PlayOneShot(gong, 1.0f);
            StartCoroutine(EndGame());
        } else {
            source.PlayOneShot(taiko, 0.5f);
            lifes--;
        }
    }

    private IEnumerator EndGame()
    {
        if (died) { yield break; }
        died = true;

        // Non-Complete Reset
        Statics.startRound = Statics.startRound - 5;
        if (Statics.startRound < 0)
            Statics.startRound = 0;

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
