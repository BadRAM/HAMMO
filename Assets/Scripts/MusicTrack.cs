using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Music Track", menuName = "Music Track", order = 51)]
public class MusicTrack : ScriptableObject
{
    public AudioClip IntroClip;
    public AudioClip LoopClip;
}
