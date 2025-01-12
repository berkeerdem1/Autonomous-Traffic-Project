using Pathfinding;
using System.IO;
using UnityEngine;

public class CustomAIPath : MonoBehaviour
{
    private AIPath aiPath;
    private Transform target;
    private Vector3 lastTargetPosition;

    private void Awake()
    {
        aiPath = GetComponent<AIPath>();
    }

    void Update()
    {
        if (target != null && target.position != lastTargetPosition)
        {
            lastTargetPosition = target.position;
            UpdateTargetDirection();
        }
    }

    private void UpdateTargetDirection()
    {
        Vector3 directionToTarget = target.position - transform.position;

        float dotProduct = Vector3.Dot(transform.forward, directionToTarget.normalized);

        if (dotProduct > 0)
        {
            aiPath.canSearch = true;
            aiPath.canMove = true;
        }
        else
        {
            aiPath.canSearch = false;
            aiPath.canMove = false;
        }
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        UpdateTargetDirection();
    }
}
