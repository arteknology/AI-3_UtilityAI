using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Curves/Utility")]
public class Utility : ScriptableObject
{
    public AnimationCurve Curve;
    
    [Header("Curve Values")]
    public int UtilityMin;
    public int ValMin;
    public int UtilityMax;
    public int ValMax;


    public int Evaluate(float Value)
    {
        float evaluatedValue = Curve.Evaluate(Value);



        return 1;
    }

}
