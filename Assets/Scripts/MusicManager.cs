using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MusicManager
{
    public static bool HasPlayerSpawned;
    public static MusicPlayer MusicPlayer;
    //public static int MusicState; // 0 - Menu, 1 - Level

    public static void ChangeTrack(int track)
    {
        MusicPlayer.ChangeTrack(track);
    }
}
