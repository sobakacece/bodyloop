using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    // Start is called before the first frame update

[NonSerialized]
    public PlayerController player;
[NonSerialized]

    public PlayerStateMachine stateMachine;

    void Start()
    {

    }
    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {

    }
}
