using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControl : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] tracks;

    public static MusicControl instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
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

    public void setVolume(float vol)
    {
        source.volume = vol;
    }
}
