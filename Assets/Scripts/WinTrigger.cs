using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter(Collider otherObject)
    {

        if (otherObject.gameObject.GetComponent<PlayerController>() != null)
        {
            GameFlow.Instance.Win();
        }
        
 
    }
}
