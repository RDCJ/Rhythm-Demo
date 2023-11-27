using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Times
{
    public int minutes;
    public int remainingSeconds;
    public int hundredths;

    public Times(float second)
    {
        minutes = (int)second / 60;
        remainingSeconds = (int)second % 60;
        hundredths = (int)((second - Math.Truncate(second)) * 1000);
    }

    public override string ToString()
    {
        return minutes.ToString().PadLeft(2, '0') + ":" +
               remainingSeconds.ToString().PadLeft(2, '0') + "." +
               hundredths.ToString().PadLeft(3, '0');
    }

    public float ToSec()
    {
        return minutes * 60.0f + remainingSeconds + hundredths * 0.001f;
    }
}
