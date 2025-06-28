using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerDeath : PlayerState
{
    // Start is called before the first frame update
    public GameObject playerPrefab;

    public override void OnEnter()
    {
        Debug.Log(player.spawnPoint);
        player.enabled = false;
        player.headCamera.GetComponent<AudioListener>().enabled = false;
        player.headCamera.GetComponent<Camera>().enabled = false;
        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;

        GameObject newPlayerObject = Instantiate(playerPrefab, player.spawnPoint, player.spawnRotation);
        PlayerController newPlayer = newPlayerObject.GetComponent<PlayerController>();
        newPlayer.enabled = true;
        newPlayer.headCamera.GetComponent<Camera>().enabled = true;
        newPlayer.headCamera.GetComponent<AudioListener>().enabled = true;
        rb = newPlayerObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        //Debug.Log("Dead");
    }
    protected override void FixedUpdate()
    {

    }

}
