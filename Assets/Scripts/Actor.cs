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
    public bool IsDoingAction;

    private Dictionary<Action, float> dict = new Dictionary<Action, float>();
    private void Start()
    {
        _navMesh = GetComponent<NavMeshAgent>();
        _navMesh.isStopped = true;
        IsDoingAction = false;
        
        SetValues();
        SetTimer();
        SetActions();
        SetOptions();
        Live();
    }
    
    private void Update()
    {
        Live();
        if (!IsDoingAction)
        {
            EvaluateOptions();
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
    
    
    private void EvaluateOptions()
    {
        dict.Clear();
        foreach (Action action in _options)
        {
            if (action.Curve.Evaluate() != 0)
            {
                float evaluatedAction = action.Curve.Evaluate();
                float minValueToDoAction = action.Curve.MinValue;
                    
                if (!dict.ContainsKey(action))
                {
                    if (evaluatedAction > minValueToDoAction)
                    {
                        dict.Add(action, action.Curve.Evaluate());
                    }
                }
            }
        }

            
        if (dict.Count > 2)
        {
            dict.OrderBy(o => o.Value);

            Action principal = dict.ElementAt(0).Key;
            Debug.Log("J'ai trop envie de faire ça"+principal.Curve.Value.Value);

            Action secondary = dict.ElementAt(1).Key;
            Debug.Log("J'ai pas mal envie de faire ça aussi"+secondary.Curve.Value.Value);

            GetDistance(principal, secondary);
        }
    }


    private void GetDistance(Action principal, Action secondary)
    {
        IsDoingAction = true;
        if (principal == null || secondary == null) return;
        _principalDist = Vector3.Distance(transform.position, principal.Transform.position);
        _secondaryDist = Vector3.Distance(transform.position, secondary.Transform.position);

        if (_principalDist < _secondaryDist)
        {
            DoAction(principal);
            Debug.Log("Comme c'est le plus important.."+principal.Curve.Value.Value);
        }
        else
        {
            if (principal.Curve.Evaluate() > principal.Curve.MaxValue)
            {
                DoAction(principal);
                Debug.Log("Comme c'est le plus important et même si l'autre est plus proche.."+" Value: "+principal.Curve.Value.Value + " EvaluatedValue: "+ principal.Curve.Evaluate());
            }
            else
            {
                DoAction(secondary);
                Debug.Log("Bon comme c'est plus proche.."+secondary.Curve.Value.Value);
            }
        }
    }
    
    
    private void DoAction(Action action)
    {
        _navMesh.isStopped = false;
        _navMesh.SetDestination(action.Transform.position);

        //Log the action
        if (action == _eatAction)
        {
            Debug.Log("Je vais manger"+action.Curve.Value.Value);
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

        float distToAction = Vector3.Distance(transform.position, action.Transform.position);
        if(distToAction <= 1)
        {
            action.Curve.Value.Value = 0f;
            Debug.Log(action.Curve.Value.Value);
            _navMesh.isStopped = true;
            IsDoingAction = false;
        }
    }
}
