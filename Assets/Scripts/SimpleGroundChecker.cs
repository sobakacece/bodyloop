using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGroundChecker : MonoBehaviour
{
 [SerializeField] private BoxCollider legsCollider;
    public bool Grounded { get; private set; }

    private bool isChanged;
    private Vector3 size;
    private void Start()
    {
        Grounded = true;
        size = legsCollider.bounds.size * 1.5f;
        size.y /= 1.5f;
    }

    private void Update()
    {
        var colliders = Physics.OverlapBox(transform.position - Vector3.up * 0.5f, legsCollider.bounds.size, Quaternion.identity, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        

        if (colliders.Length > 0)
        {
            if (isChanged != Grounded)
            {
                isChanged = Grounded;
			}

            Grounded = true;
        }
        else
        {
			if (isChanged != Grounded)
			{
				isChanged = Grounded;
			}

			Grounded = false;
        }
    }

    private void OnDrawGizmos()
    {
//        Gizmos.DrawCube(transform.position - Vector3.up * 0.5f, legsCollider.bounds.size);
    }

    private IEnumerator Check()
    {
        while (true)
        {
            yield return null;
            var colliders = Physics.OverlapBox(transform.position - Vector3.up * 0.5f, legsCollider.bounds.size, Quaternion.identity, Physics.AllLayers, QueryTriggerInteraction.Ignore);


            if (colliders.Length > 0)
            {
                Grounded = true;
            }
            else
            {
                yield return new WaitForSecondsRealtime(0.1f);
                Grounded = false;
            }
        }
    }
}
