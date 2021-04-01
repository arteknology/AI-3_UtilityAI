using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public Utility Curve;
    public Transform Transform;

    public Action(Utility utility, Transform transform)
    {
        Curve = utility;
        Transform = transform;
    }
}
