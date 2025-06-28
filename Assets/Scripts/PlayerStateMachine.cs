using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor.UI;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{

    public enum StateEnum
    {
        Normal,
        Climb,
        Death

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

    void Start()
    {
        foreach (PlayerScriptRelation relation in actions)
        {
            relation.action.stateMachine = this;
            relation.action.player = player;
        }
        FindAction(currentState).enabled = true;
}
    public void ChangeState(StateEnum nextState)
    {
        if (FindAction(nextState) != null && nextState != currentState)
        {
            FindAction(currentState).OnExit();
            FindAction(currentState).enabled = false;
            FindAction(nextState).OnEnter();
            FindAction(nextState).enabled = true;
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
}


