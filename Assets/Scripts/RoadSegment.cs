using System;
using System.Collections.Generic;
using UnityEngine;

public class RoadSegment : MonoBehaviour
{
    public Transform startPoint; 
    public Transform endPoint;
    public List<Transform> intermediatePoints;  
    public Junction junction;            
    public bool oneWay = true;

    // Kav�a�� ayarlamak i�in
    public void SetJunction(Junction junction)
    {
        this.junction = junction;
    }

    public Vector3 GetRoadDirection()
    {
        return (endPoint.position - startPoint.position).normalized;
    }

    public bool IsDirectionValid(Vector3 currentPosition, Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - currentPosition).normalized;
        Vector3 roadDirection = GetRoadDirection();

        // E�er ara� hedefin ters y�n�ndeyse, ge�erli yolda ilerlememesi gerekir
        return Vector3.Dot(directionToTarget, roadDirection) > 0;
    }

    internal void SetColor(Color color)
    {
        throw new NotImplementedException();
    }
}
