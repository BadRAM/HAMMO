using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameMode
{
    //Gamemode IDs
    // 0 - Standard
    // 1 - Turbo - Unlimited Hammo
    // 2 - Stealth - Enemies dont need to be destroyed
    // 3 - Scavenger - Start with one Hammo
    // 4 - Classic - free Bhopping, no bounce
    // 5 - Frictionless - normal surfaces are now frictionless, but still allow jumping

    private static int _mode;
    private static int _maxEnabled;
    private static int _levelID = -1;

    public static int Get()
    {
        return _mode;
    }

    public static void Set(int g)
    {
        if (g <= _maxEnabled)
        {
            _mode = g;
        }
    }

    public static int GetMax()
    {
        return _maxEnabled;
    }

    public static void SetMax(int max)
    {
        _maxEnabled = max;

        if (_mode > max)
        {
            _mode = 0;
        }
    }

    public static int GetID()
    {
        return _levelID;
    }

    public static void SetID(int id)
    {
        _levelID = id;
    }

}
