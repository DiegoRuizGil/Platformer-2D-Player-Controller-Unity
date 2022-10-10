using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KnockbackInfo
{
    private Vector2 direction;

    public Vector2 Direction { get { return direction; } set { direction = value.normalized; } }

    public KnockbackInfo(Vector2 direction)
    {
        Direction = direction;
    }
}
