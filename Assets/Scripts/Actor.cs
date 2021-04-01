using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Utility EatCurve;
    public Utility SleepCurve;
    public Utility PeeCurve;
    public Utility WashCurve;

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

    private List<Action> _options = new List<Action>();
    private float _principalDist;
    private float _secondaryDist;
    
    private NavMeshAgent _navMesh;
    
    private float _currEatTimer = 0f;
    private float _currSleepTimer = 0f;
    private float _currPeeTimer = 0f;
    private float _currWashTimer = 0f;

    private Action _eatAction;
    private Action _sleepAction;
    private Action _peeAction;
    private Action _washAction;

    private Dictionary<Action, float> dict = new Dictionary<Action, float>();
    private void Start()
    {
        _navMesh = GetComponent<NavMeshAgent>();
        _navMesh.isStopped = true;
        
        SetValues();
        SetTimer();
        SetActions();
        SetOptions();
    }
    
    private void Update()
    {
        Live();
        InvokeRepeating("EvaluateOptions", 0.1f, 1);
    }
    
    
    //IncreaseActionsValues
    private void Live()
    {
        if (_currEatTimer > 0)
        {
            _currEatTimer -= Time.deltaTime;
        }
        else
        {
            if (EatValue.Value < 1)
            { 
                EatValue.Value+= 0.1f;
            }
            _currEatTimer = EatTimer;
        }

        if (_currSleepTimer > 0)
        {
            _currSleepTimer -= Time.deltaTime;
        }
        else
        {
            if (SleepValue.Value < 1)
            {
                SleepValue.Value += 0.1f;
            }
            _currSleepTimer = SleepTimer;
        }

        if (_currPeeTimer > 0)
        {
            _currPeeTimer -= Time.deltaTime;
        }
        else
        {
            if (PeeValue.Value < 1)
            {
                PeeValue.Value += 0.1f;
            }
            _currPeeTimer = PeeTimer;
        }

        if (_currWashTimer > 0)
        {
            _currWashTimer -= Time.deltaTime;
        }
        else
        {
            if (WashValue.Value < 1)
            {
                WashValue.Value += 0.1f;
            }
            _currWashTimer = WashTimer;
        }
    }

    private void SetValues()
    {
        EatValue.Value = 0f;
        SleepValue.Value = 0f;
        PeeValue.Value = 0f;
        WashValue.Value = 0f;
    }


    private void SetTimer()
    {
        _currEatTimer = EatTimer;
        _currSleepTimer = SleepTimer;
        _currPeeTimer = PeeTimer;
        _currWashTimer = WashTimer;
    }
    
    
    private void SetActions()
    {
        _eatAction = new Action(EatCurve, Kitchen);
        _sleepAction = new Action(SleepCurve, Bed);
        _peeAction = new Action(PeeCurve, Toilets);
        _washAction = new Action(WashCurve, Shower);
    }

    private void SetOptions()
    {
        _options.Add(_eatAction);
        _options.Add(_sleepAction);
        _options.Add(_peeAction);
        _options.Add(_washAction);    
    }
    
    private void EvaluateOptions()
    {
        if (_navMesh.isStopped)
        {
            foreach (Action action in _options)
            {
                if (action.Curve.Value.Value != 0)
                {
                    if (!dict.ContainsKey(action))
                    {
                        dict.Add(action, action.Curve.Evaluate());
                    }
                }
                else
                {
                    if (dict.ContainsKey(action))
                    {
                        dict.Remove(action);
                    }
                }

            }

            dict.OrderByDescending(o => o.Value);


            if (dict.Count > 2)
            {
                Action firstKey = dict.ElementAt(0).Key;
                Action secondKey = dict.ElementAt(1).Key;

                Action principal = null;
                Action secondary = null;
            
                if (firstKey.Curve.Evaluate() > firstKey.Curve.MinValue)
                {
                    principal = firstKey;
                }

                if (secondKey.Curve.Evaluate() > secondKey.Curve.MinValue)
                {
                    secondary = secondKey;
                }

                GetDistance(principal, secondary);
            }
        }
    }


    private void GetDistance(Action principal, Action secondary)
    {
        if(principal == null) return;
        if(secondary == null) return;
        _principalDist = Vector3.Distance(transform.position, principal.Transform.position);
        _secondaryDist = Vector3.Distance(transform.position, secondary.Transform.position);

        if (_principalDist < _secondaryDist)
        {
            if (transform.position != principal.Transform.position)
            {
                DoAction(principal);
            }
        }
        else
        {
            DoAction(secondary);
        }
    }
    
    
    private void DoAction(Action action)
    {
        _navMesh.isStopped = false;
        _navMesh.SetDestination(action.Transform.position);

        //Log the action
        if (action == _eatAction)
        {
            Debug.Log("Je vais manger");
        }

        if (action == _sleepAction)
        {
            Debug.Log("Je vais dormir");
        }

        if (action == _peeAction)
        {
            Debug.Log("Je vais aux toilettes");
        }

        if (action == _washAction)
        {
            Debug.Log("Je vais me laver");
        }
        
        if(transform.position.x == action.Transform.position.x && transform.position.z == action.Transform.position.z)
        {    
            action.Curve.Value.Value = 0f;
            Debug.Log(action.Curve.Value.Value);
            _navMesh.isStopped = true;
        }
    }
}
