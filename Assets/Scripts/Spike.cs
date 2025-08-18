using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public GameObject playerPrefab;

    public LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
    }

    void OnCollisionEnter(Collision col)
    {
        StateMachine stateMachine = col.collider.GetComponent<StateMachine>();
        //        Debug.Log(col.gameObject.name);
        if (stateMachine != null && col.contacts.Length > 0)
        {
            ContactPoint contact = col.GetContact(0);
            StartCoroutine(AttachRagdoll(contact, stateMachine));
        }

        // On collision we simply create a glue object at any contact point.
        //CreateGlue(col.contacts[0].point, col.collider.gameObject);
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

    }

    private IEnumerator AttachRagdoll(ContactPoint col, StateMachine stateMachine)
    {

        yield return new WaitForEndOfFrame();
        PlayerDeath state = stateMachine.GetComponent<PlayerDeath>();
        state.ragdollSpawnPoint = col.point;
        state.shouldGlue = true;
        stateMachine.ChangeState(StateMachine.StateEnum.Death);


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
