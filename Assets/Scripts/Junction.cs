using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Junction : MonoBehaviour
{
    public List<RoadSegment> connectedRoads;  
    public bool isClockwise = false;

    public void InitializeJunction()
    {
        if (isClockwise)
        {
            connectedRoads = connectedRoads.OrderBy(road => road.transform.position.x).ToList();
        }
        else
        {
            connectedRoads = connectedRoads.OrderByDescending(road => road.transform.position.x).ToList();
        }

        foreach (RoadSegment road in connectedRoads)
        {
            road.SetJunction(this);
        }
    }

    public RoadSegment GetNextRoad(RoadSegment currentRoad)
    {
        int currentIndex = connectedRoads.IndexOf(currentRoad);
        int nextIndex = (currentIndex + (isClockwise ? 1 : -1)) % connectedRoads.Count;

        return connectedRoads[nextIndex];
    }
}
