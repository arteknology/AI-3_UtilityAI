using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Curves/Utility")]
public class Utility : ScriptableObject
{
    public AnimationCurve Curve;
    public SoFloat Value;
    public float MinValue;
    public float MaxValue;
        
    public float Evaluate()
    {
        float evaluatedValue = Curve.Evaluate(Value.Value);
        return evaluatedValue;
    }

}
