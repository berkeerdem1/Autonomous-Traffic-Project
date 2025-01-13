using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class QLearningSystem : MonoBehaviour
{
    float epsilon = 0.1f;
    float epsilonDecay = 0.99f;

    public float learningRate = 0.1f;
    public float discountFactor = 0.9f;

    public RoadSegment currentSegment;
    public Dictionary<RoadSegment, float> qValues = new Dictionary<RoadSegment, float>();

    private List<RoadSegment> allSegments;
    private AIPath aiPath;

    [System.Obsolete]
    void Start()
    {
        aiPath = GetComponent<AIPath>();

        allSegments = new List<RoadSegment>(FindObjectsOfType<RoadSegment>());
        foreach (RoadSegment segment in allSegments)
        {
            qValues[segment] = 0f;
        }

        InvokeRepeating("Repeat", 0.2f, 1f);
    }
    private void FixedUpdate()
    {
        if (currentSegment == null) return;
        epsilon = Mathf.Max(0.01f, epsilon * epsilonDecay); NextSegmetnChoose();
    }

    void Repeat()
    {
        UpdateCurrentSegment();
        LearnSegment();
        ChooseNextSegment();

        Debug.Log($"Epsilon: {epsilon}");
    }
    void UpdateCurrentSegment()
    {
        if (allSegments == null || allSegments.Count == 0) return;

        float closestDistance = float.MaxValue;
        RoadSegment closestSegment = null;

        foreach (var segment in allSegments)
        {
            float distanceToSegment = (Vector3.Distance(transform.position, segment.startPoint.position) + Vector3.Distance(transform.position, segment.endPoint.position)) / 2f;

            if (distanceToSegment < closestDistance)
            {
                closestDistance = distanceToSegment;
                closestSegment = segment;
            }
        }

        if (closestSegment != null)
        {
            currentSegment = closestSegment;
        }
    }

    void NextSegmetnChoose()
    {
        if (Vector3.Distance(transform.position, currentSegment.endPoint.position) < 1f)
        {
            RoadSegment nextSegment = ChooseNextSegment();
            if (nextSegment != null)
            {
                currentSegment = nextSegment;
                aiPath.destination = currentSegment.endPoint.position;
            }
        }
    }

    void LearnSegment()
    {
        if (currentSegment == null) return;

        float reward = CalculateReward();

        print("Reward" + reward);

        float oldQValue = qValues[currentSegment];
        float maxNextQValue = FindMaxQValueForNextSegment();

        qValues[currentSegment] = oldQValue + learningRate * (reward + discountFactor * maxNextQValue - oldQValue);

        Debug.Log($"Segment: {currentSegment.name}, Q-Value: {qValues[currentSegment]}");
    
    }

    float CalculateReward()
    {
        Vector3 directionToEnd = (currentSegment.endPoint.position - aiPath.transform.position).normalized;
        Vector3 directionOfSegment = (currentSegment.endPoint.position - currentSegment.startPoint.position).normalized;

        float reward = Vector3.Dot(directionToEnd, directionOfSegment);
        if (reward < 0) 
        {
            reward = -1f;  
        }

        return reward;
    }

    float FindMaxQValueForNextSegment()
    {
        float maxQValue = float.MinValue;

        foreach (RoadSegment segment in FindConnectedSegments(currentSegment.endPoint))
        {
            if (qValues.ContainsKey(segment))
            {
                maxQValue = Mathf.Max(maxQValue, qValues[segment]);
            }
        }

        return maxQValue;
    }

    List<RoadSegment> FindConnectedSegments(Transform point)
    {
        List<RoadSegment> connectedSegments = new List<RoadSegment>();

        foreach (RoadSegment segment in FindObjectsOfType<RoadSegment>())
        {
            if (segment.startPoint == point)
            {
                connectedSegments.Add(segment);
            }
        }

        return connectedSegments;
    }

    RoadSegment ChooseNextSegment()
    {
        List<RoadSegment> connectedSegments = FindConnectedSegments(currentSegment.endPoint);

        if (connectedSegments.Count == 0) return null;

        // ε-greedy strategy
        if (Random.value < epsilon)
        {
            return connectedSegments[Random.Range(0, connectedSegments.Count)];
        }
        else
        {
            RoadSegment bestSegment = null;
            float maxQValue = float.MinValue;

            foreach (var segment in connectedSegments)
            {
                if (qValues.TryGetValue(segment, out float qValue) && qValue > maxQValue)
                {
                    maxQValue = qValue;
                    bestSegment = segment;
                }
            }

            return bestSegment;
        }
    }
}
