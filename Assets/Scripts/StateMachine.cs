using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{

    public enum StateEnum
    {
        Normal,
        Climb,
        Death,
        Dash

    }

    [System.Serializable]
    public class PlayerScriptRelation
    {
        public StateEnum state;
        public PlayerState action;
    }
    public PlayerScriptRelation[] actions;
    // Start is called before the first frame update
    public StateEnum currentState = StateEnum.Normal;

    public PlayerController player;

    public event Action<StateEnum> StateEnterEvent;
    public event Action<StateEnum> StateExitEvent;

    void Start()
    {
        foreach (PlayerScriptRelation relation in actions)
        {
            relation.action.stateMachine = this;
            relation.action.player = player;
        }
        FindAction(currentState).enabled = true;
    }

    void Update()
    {
        // Debug.Log(currentState);
    }
    public void ChangeState(StateEnum nextState)
    {
        if (FindAction(nextState) != null && nextState != currentState)
        {
            FindAction(currentState).OnExit();
            FindAction(currentState).enabled = false;
            StateExitEvent?.Invoke(currentState);

            FindAction(nextState).OnEnter();
            FindAction(nextState).enabled = true;
            StateEnterEvent?.Invoke(nextState);

            currentState = nextState;
        }
    }

    PlayerState FindAction(StateEnum requiredState)
    {
        foreach (PlayerScriptRelation relation in actions)
        {
            if (relation.state == requiredState)
            {
                return relation.action;
            }
        }
        return null;
    }

    public void ResetStates()
    {
        foreach (PlayerScriptRelation relation in actions)
        {
            relation.action.enabled = false;
            FindAction(currentState).enabled = true;
        }
    }
}


