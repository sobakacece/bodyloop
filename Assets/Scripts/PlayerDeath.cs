using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerDeath : PlayerState
{
    // Start is called before the first frame update
    public GameObject gluePrefab;
    public GameObject usualPrefab;
    public Vector3 ragdollSpawnPoint = Vector3.zero;
    public bool shouldGlue = false;
    public override void OnEnter()
    {
        //        Debug.Log(player.spawnPoint);
        // player.enabled = false;
        // player.headCamera.GetComponent<AudioListener>().enabled = false;
        // player.headCamera.GetComponent<Camera>().enabled = false;
        // Rigidbody rb = player.GetComponent<Rigidbody>();
        // rb.useGravity = true;
        // Collider playerCollider = player.GetComponent<Collider>();
        // playerCollider.enabled = false;
        //rb.constraints = RigidbodyConstraints.FreezeAll;

        //GameObject newPlayerObject = Instantiate(playerPrefab, transform.position, transform.rotation);
        if (ragdollSpawnPoint == Vector3.zero)
        {
            ragdollSpawnPoint = player.transform.position;
            shouldGlue = false;
        }
        SpawnRagdoll();
        player.Respawn();
        // PlayerController newPlayer = newPlayerObject.GetComponent<PlayerController>();
        // newPlayer.enabled = true;
        // newPlayer.headCamera.GetComponent<Camera>().enabled = true;
        // newPlayer.headCamera.GetComponent<AudioListener>().enabled = true;
        // rb = newPlayerObject.GetComponent<Rigidbody>();
        // rb.constraints = RigidbodyConstraints.FreezeRotation;
        // newPlayer.GetComponent<Collider>().enabled = true;
        //Debug.Log("Dead");
    }
    protected override void FixedUpdate()
    {
        stateMachine.ChangeState(StateMachine.StateEnum.Normal);
    }

    public override void OnExit()
    {
        shouldGlue = false;
        ragdollSpawnPoint = Vector3.zero;
    }

    public void SpawnRagdoll()
    {
        GameObject playerPrefab = shouldGlue ? gluePrefab : usualPrefab;
        Instantiate(playerPrefab, ragdollSpawnPoint, transform.rotation);
    }



}
