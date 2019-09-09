using UnityEngine;
using System.Collections;
 
[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    public AudioClip engineStartClip;
    public AudioClip engineLoopClip;
    public AudioSource Source;
    void Start()
    {
        Source =GetComponent<AudioSource>();
        Source.loop = true;
        StartCoroutine(playEngineSound());
    }
 
    IEnumerator playEngineSound()
    {
        Source.clip = engineStartClip;
        Source.Play();
        yield return new WaitForSeconds(Source.clip.length);
        Source.clip = engineLoopClip;
        Source.Play();
    }
}