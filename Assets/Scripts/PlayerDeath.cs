using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : PlayerState
{
    // Start is called before the first frame update


    protected override void FixedUpdate()
    {
        Debug.Log("Dead");
    }

}
