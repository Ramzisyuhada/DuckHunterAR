using UnityEngine;

public class SpawnVisual : MonoBehaviour
{
    public float radius = 0.3f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, radius);
    }
}