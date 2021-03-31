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
    public Utility EatSO;
    public Utility SleepSO;
    public Utility PeeSO;
    public Utility WashSO;

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

    private Options _options = new Options();
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
        if (_navMesh.isStopped)
        {
            EvaluateOptions();
        }
    }
    
    
    //DecreaseActionsTimer
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
        _eatAction = new Action(EatValue, Kitchen);
        _sleepAction = new Action(SleepValue, Bed);
        _peeAction = new Action(PeeValue, Toilets);
        _washAction = new Action(WashValue, Shower);
    }

    private void SetOptions()
    {
        _options.Actions.Add(_eatAction);
        _options.Actions.Add(_sleepAction);
        _options.Actions.Add(_peeAction);
        _options.Actions.Add(_washAction);    
    }
    
    private void EvaluateOptions()
    {
        _options.EvaluatedValues.Clear();
        Action principal = new Action(null, null);
        Action secondary = new Action(null, null);
        
        foreach (Action action in _options.Actions)
        {
            if (action.ActionValue.Value == EatValue.Value && action.ActionValue.Value != 0)
            {
                _options.EvaluatedValues.Add(EatSO.Evaluate());
            }
            if (action.ActionValue.Value == SleepValue.Value && action.ActionValue.Value != 0)
            {
                _options.EvaluatedValues.Add(SleepSO.Evaluate());
            }
            if (action.ActionValue.Value == PeeValue.Value && action.ActionValue.Value != 0)
            {
                _options.EvaluatedValues.Add(PeeSO.Evaluate());
            }
            if (action.ActionValue.Value == WashValue.Value && action.ActionValue.Value != 0)
            {
                    _options.EvaluatedValues.Add(WashSO.Evaluate());
            }
        }

        _options.EvaluatedValues.Sort();
        _options.EvaluatedValues.Reverse();
        
        //SetPrimary
        if (_options.EvaluatedValues[0] == EatSO.Evaluate())
        {
            principal = _eatAction;
        }
        if (_options.EvaluatedValues[0] == SleepSO.Evaluate())
        {
            principal = _sleepAction;
        }
        if (_options.EvaluatedValues[0] == PeeSO.Evaluate())
        {
            principal = _peeAction;
        }
        if (_options.EvaluatedValues[0] == WashSO.Evaluate())
        {
            principal = _washAction;
        }
        
        //SetSecondary
        if (_options.EvaluatedValues[1] == EatSO.Evaluate())
        {
            secondary = _eatAction;
        }
        if (_options.EvaluatedValues[1] == SleepSO.Evaluate())
        {
            secondary = _sleepAction;
        }
        if (_options.EvaluatedValues[1] == PeeSO.Evaluate())
        {
            secondary = _peeAction;
        }
        if (_options.EvaluatedValues[1] == WashSO.Evaluate())
        {
            secondary = _washAction;
        }

        GetDistance(principal, secondary);
    }
    
    
    private void GetDistance(Action principal, Action secondary)
    {
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
        Debug.Log("I move");
        
        if(transform.position.x == Kitchen.position.x && transform.position.z == Kitchen.position.z)
        {
            EatValue.Value = 0f;
            _navMesh.isStopped = true;
            Debug.Log(action.ActionValue.Value);
        }
        if(transform.position.x == Bed.position.x && transform.position.z == Bed.position.y)
        {
            SleepValue.Value = 0f;
            _navMesh.isStopped = true;
        }
        if(transform.position.x == Toilets.position.x && transform.position.z == Toilets.position.z)
        {
            PeeValue.Value = 0f;
            _navMesh.isStopped = true;
        }
        if(transform.position.x == Shower.position.x && transform.position.y == Shower.transform.position.z)
        {
            WashValue.Value = 0f;
            _navMesh.isStopped = true;
        }
    }
}
