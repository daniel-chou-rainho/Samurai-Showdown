using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Head : MonoBehaviour
{
    public int lifes;
    public Fader fader;

    public void TakeLife()
    {
        // TODO: Damage SFX

        if(lifes == 1){
            StartCoroutine(EndGame());
        } else {
            lifes--;
            //Debug.Log("Lifes = " + lifes.ToString());
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
