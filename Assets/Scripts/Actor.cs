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


    [Header("So Value")]
    public SoFloat EatValue;
    public SoFloat SleepValue;
    public SoFloat PeeValue;
    public SoFloat WashValue;


    [Header("Timers")]
    public float EatTimer;
    public float SleepTimer;
    public float PeeTimer;
    public float WashTimer;
    
    
    private NavMeshAgent _navMesh;
    private float _bedDist;
    private float _kitchenDist;
    private float _toiletsDist;
    private float _showerDist;

    void Start()
    {
        _navMesh = GetComponent<NavMeshAgent>();
    }

    
    private void Update()
    {
        GetDistance();
        Move();
    }
    

    private void GetDistance()
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
