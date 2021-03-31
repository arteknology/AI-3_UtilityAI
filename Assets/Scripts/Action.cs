using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public SoFloat ActionValue;
    public Transform Transform;

    public Action(SoFloat value, Transform transform)
    {
        ActionValue = value;
        Transform = transform;
    }
}
