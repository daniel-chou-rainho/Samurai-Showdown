using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    public bool fadeOnStart = true;
    public Color fadeColor;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (fadeOnStart)
        {
            FadeIn(1.0f);
        }
    }

    public void FadeIn(float duration)
    {
        Fade(1, 0, duration);
    }

    public void FadeOut(float duration)
    {
        Fade(0, 1, duration);
    }

    public void Fade(float alphaIn, float alphaOut, float duration)
    {
        StartCoroutine(FadeRoutine(alphaIn, alphaOut, duration));
    }

    public IEnumerator FadeRoutine(float alphaIn, float alphaOut, float duration)
    {
        float timer = 0;
        while(timer <= duration)
        {
            Color newColor = fadeColor;
            newColor.a = Mathf.Lerp(alphaIn, alphaOut, timer / duration);
            rend.material.SetColor("_BaseColor", newColor);

            timer += Time.deltaTime;
            yield return null;
        }

        // Insurance
        Color newColor2 = fadeColor;
        newColor2.a = alphaOut;
        rend.material.SetColor("_BaseColor", newColor2);
    }
}
