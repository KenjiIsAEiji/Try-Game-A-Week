using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBeacon : MonoBehaviour
{
    public float GetDistance(Vector3 target)
    {
        return (transform.position - target).magnitude;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(1, 2, 1));
    }
}
