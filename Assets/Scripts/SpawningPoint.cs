using System;
using UnityEngine;

public class SpawningPoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}