using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public GameObject playerPrefab;
    private Collision coll;

    public LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
    }

    void OnCollisionEnter(Collision col)
    {
        PlayerStateMachine stateMachine = col.collider.GetComponent<PlayerStateMachine>();
        Debug.Log(col.gameObject.name);
        if (stateMachine != null)
        {
            stateMachine.ChangeState(PlayerStateMachine.StateEnum.Death);
            coll = col;
            StartCoroutine(AttachRagdoll());
        }

        // On collision we simply create a glue object at any contact point.
        //CreateGlue(col.contacts[0].point, col.collider.gameObject);
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

    }

    private IEnumerator AttachRagdoll()
    {
        yield return new WaitForEndOfFrame();
        //Vector3 freeSpot = FindEmptySpawnPoint(coll.contacts[0].point, coll.contacts[0].normal, playerPrefab.GetComponentInChildren<SkinnedMeshRenderer>().bounds.size, 6, layerMask);
        Instantiate(playerPrefab, coll.contacts[0].point, coll.body.gameObject.transform.rotation);

    }

    // Vector3 FindEmptySpawnPoint(Vector3 origin, Vector3 surfaceNormal, Vector3 size, float radius, LayerMask checkMask)
    // {
    //     const int resolution = 25;
    //     float step = radius / resolution;
    //     Vector3 up = surfaceNormal;
    //     Vector3 right = Vector3.Cross(up, Vector3.forward).normalized;
    //     if (right.sqrMagnitude < 0.01f) right = Vector3.Cross(up, Vector3.right).normalized;
    //     Vector3 forward = Vector3.Cross(right, up);

    //     for (int x = -resolution; x <= resolution; x++)
    //         for (int y = 0; y <= resolution; y++) // Prefer space above
    //             for (int z = -resolution; z <= resolution; z++)
    //             {
    //                 Vector3 offset = right * x * step + up * y * step + forward * z * step;
    //                 Vector3 candidate = origin + offset;

    //                 if (!Physics.CheckBox(candidate, size * 0.5f, Quaternion.identity, checkMask))
    //                     return candidate;
    //             }

    //     return origin; // fallback if nothing found
    // }

}
