using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Directions
{
    public enum Direction { NE, E, SE, SW, W, NW };

    public static Vector3 north_east = new Vector3(1.0f, 1.75f, 0.0f);
    public static Vector3 east = new Vector3(2.0f, 0.0f, 0.0f);
    public static Vector3 south_east = new Vector3(1.0f, -1.75f, 0.0f);
    public static Vector3 south_west = new Vector3(-1.0f, -1.75f, 0.0f);
    public static Vector3 west = new Vector3(-2.0f, 0.0f, 0.0f);
    public static Vector3 north_west = new Vector3(-1.0f, 1.75f, 0.0f);
}
