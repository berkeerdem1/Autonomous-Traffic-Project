using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traffic_AI : MonoBehaviour
{
    private AIPath aiPath;
    public List<RoadSegment> roadSegments;
    private Transform currentTarget;

    void Start()
    {
        aiPath = GetComponent<AIPath>();

        StartCoroutine(Awaken());
    }

    IEnumerator Awaken()
    {
        GameObject[] roadSegmentsWithTag = Object_Pool.Instance.GetRoadSegments();

        yield return new WaitForSeconds(1f);

        foreach (GameObject obj in roadSegmentsWithTag)
        {
            roadSegments.Add(obj.GetComponent<RoadSegment>());
        }
    }

    void Update()
    {
        if (roadSegments == null)
            return;

        RoadSegment closestSegment = FindClosestSegment(aiPath.transform.position);

        if (closestSegment != null)
        {
            aiPath.destination = closestSegment.endPoint.position;
            currentTarget = closestSegment.endPoint;
        }

        if (currentTarget == null || Vector3.Distance(aiPath.transform.position, currentTarget.position) < 1f)
        {
            RoadSegment nextSegment = FindNextSegment();

            if (nextSegment != null)
            {
                aiPath.destination = nextSegment.endPoint.position;
                currentTarget = nextSegment.endPoint;
            }
        }
    }

    RoadSegment FindClosestSegment(Vector3 position)
    {
        RoadSegment closestSegment = null;
        float closestDistance = Mathf.Infinity;

        foreach (RoadSegment segment in roadSegments)
        {
            float distanceToStart = Vector3.Distance(position, segment.startPoint.position);
            float distanceToEnd = Vector3.Distance(position, segment.endPoint.position);

            if (distanceToStart < closestDistance)
            {
                closestDistance = distanceToStart;
                closestSegment = segment;
            }

            if (distanceToEnd < closestDistance)
            {
                closestDistance = distanceToEnd;
                closestSegment = segment;
            }
        }

        return closestSegment;
    }

    RoadSegment FindNextSegment()
    {
        RoadSegment nextSegment = null;
        float closestDistance = Mathf.Infinity;

        foreach (RoadSegment segment in roadSegments)
        {
            float distanceToStart = Vector3.Distance(aiPath.transform.position, segment.startPoint.position);

            if (distanceToStart < closestDistance)
            {
                closestDistance = distanceToStart;
                nextSegment = segment;
            }
        }

        return nextSegment;
    }

    public Transform GetTarget()
    {
        return currentTarget;
    }
    public void SetTarget(Transform newTarget)
    {
        //currentTarget = newTarget;
    }
}
