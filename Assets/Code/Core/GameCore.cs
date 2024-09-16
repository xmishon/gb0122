using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCore
{
    public static GameCore instance;

    public CarSetup carSetup;
    public TrackSetup trackSetup;

    public GameCore()
    {
        instance = this;
        carSetup = new CarSetup();
        trackSetup = new TrackSetup();
    }
}
