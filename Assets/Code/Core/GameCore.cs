using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCore
{
    public static GameCore instance;

    public CarSetup carSetup;

    public GameCore()
    {
        instance = this;
        carSetup = new CarSetup();
    }
}
