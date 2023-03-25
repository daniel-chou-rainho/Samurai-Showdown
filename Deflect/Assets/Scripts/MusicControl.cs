using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicControl : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] tracks;
    public Slider volumeSlider;

    private void Start()
    {
        float savedVolume = Statics.musicVolume;
        source.volume = savedVolume;
        volumeSlider.value = savedVolume;
        StartCoroutine(Music());
    }

    private IEnumerator Music()
    {
        while (true)
        {
            AudioClip track = tracks[Random.Range(0, tracks.Length)];
            source.PlayOneShot(track);
            yield return new WaitForSeconds(track.length);
        }
    }

    public void SetVolume(float volume)
    {
        source.volume = volume;
        Statics.musicVolume = volume;
    }
}
