using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    public MusicTrack[] MusicTracks;
    public int Track;
    private AudioSource _source;
    void Start()
    {
        if (!MusicManager.HasPlayerSpawned)
        {
            MusicManager.HasPlayerSpawned = true;
            MusicManager.MusicPlayer = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        _source = GetComponent<AudioSource>();
        _source.loop = true;
        StartCoroutine(PlayMusic());
    }

    public void ChangeTrack(int track)
    {
        StopCoroutine(PlayMusic());
        Track = track;
        StartCoroutine(PlayMusic());
    }
 
    IEnumerator PlayMusic()
    {
        if (MusicTracks[Track].IntroClip != null)
        {
            _source.clip = MusicTracks[Track].IntroClip;
            _source.Play();
            yield return new WaitForSecondsRealtime(_source.clip.length);
        }
        _source.clip = MusicTracks[Track].LoopClip;
        _source.Play();
    }
}