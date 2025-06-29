using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glue : MonoBehaviour {

    protected Transform stuckTo = null;
    protected Vector3 offset = Vector3.zero;

    public void AttachObject(GameObject other)
    {
        // Basically - same code as yours with slight modifications

        // Make rigidbody Kinematic
        // var rb = other.GetComponent<Rigidbody>();
        // rb.isKinematic = true;

        // Calculate offset - pay attention the direction of the offset is now reverse
        // since we attach glue to object and not object to glue. It can be modified to work
        // the other way, it just seems more reasonable to set all "glueing" functionality
        // at Glue object
        offset = transform.position - other.transform.position;

        stuckTo = other.transform;
    }

    public void LateUpdate()
    {
        if (stuckTo != null) {
            // If you don't want to support rotation remove this line
            stuckTo.rotation = transform.rotation;

            stuckTo.position = transform.position - transform.rotation * offset;
        }
    }

    // Just visualizing the glue point, remove if not needed
    void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}