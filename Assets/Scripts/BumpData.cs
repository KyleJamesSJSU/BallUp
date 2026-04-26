using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpData
{
    public Vector3 origin;
    public float strength;
    public float speedNegation;

    public BumpData(Vector3 origin, float strength, float speedNegation)
    {
        this.origin = origin;
        this.strength = strength;
        this.speedNegation = speedNegation;
    }
}
