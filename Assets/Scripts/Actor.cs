using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Actor : MonoBehaviour
{
    [Header("Room Transform")]
    public Transform Toilets;
    public Transform Kitchen;
    public Transform Bed;
    public Transform Shower;


    [Header("Curves SO")]
    public Utility Eat;
    public Utility Sleep;
    public Utility Pee;
    public Utility Wash;


    private NavMeshAgent _navMesh;
    private float _bedDist;
    private float _kitchenDist;
    private float _toiletsDist;
    private float _showerDist;

    void Start()
    {
        Eat.Curve = new AnimationCurve(new Keyframe(Eat.UtilityMin, Eat.ValMin), new Keyframe(Eat.UtilityMax, Eat.ValMax));
        Sleep.Curve = new AnimationCurve(new Keyframe(Sleep.UtilityMin, Sleep.ValMin), new Keyframe(Sleep.UtilityMax, Sleep.ValMax));
        Pee.Curve = new AnimationCurve(new Keyframe(Pee.UtilityMin, Pee.ValMin), new Keyframe(Pee.UtilityMax, Pee.ValMax));
        Wash.Curve = new AnimationCurve(new Keyframe(Wash.UtilityMin, Wash.ValMin), new Keyframe(Wash.UtilityMax, Wash.ValMax));

        _navMesh = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        _bedDist = Vector3.Distance(transform.position, Bed.position);
        _kitchenDist = Vector3.Distance(transform.position, Kitchen.position);
        _toiletsDist = Vector3.Distance(transform.position, Toilets.position); 
        _showerDist = Vector3.Distance(transform.position, Shower.position);
    }
    
    private void Move()
    {
        /*
        if(Eat.Evaluate(Value) > Sleep.Evaluate(Value) &&)
        {
            if(Eat.Evaluate(Value) > Pee.Evaluate(Value))
            {
                if(Eat.Evaluate(Value) > Wash.Evaluate(Value))
                {
                    
                }
            }
        }*/
    }

}
