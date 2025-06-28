using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spike : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    void OnTriggerEnter(Collider otherObject)
    {
        PlayerStateMachine stateMachine = otherObject.GetComponent<PlayerStateMachine>();
        //Debug.Log(otherObject.name);
        if (stateMachine != null)
        {
            stateMachine.ChangeState(PlayerStateMachine.StateEnum.Death);
        }
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
