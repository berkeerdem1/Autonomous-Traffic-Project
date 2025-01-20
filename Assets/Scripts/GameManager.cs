using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Road
{
    public Transform target;
    public Transform NewTarget;
    public List<Transform> cars; 
}

public class GameManager : MonoBehaviour
{
    public List<Road> roads; // targets and cars of 4 roads
    public float targetChangeInterval = 120f;

    private void Start()
    {
        StartCoroutine(ChangeTargetsPeriodically());
    }

    private IEnumerator ChangeTargetsPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(targetChangeInterval);
            
            List<int> selectedRoads = GetTwoRandomRoadIndices();
            List<int> remainingRoads = GetRemainingRoadIndices(selectedRoads);
            
            RedirectCarsToOtherRoads(selectedRoads, remainingRoads);
        }
    }


    private List<int> GetTwoRandomRoadIndices() // Select 2 random roads with 4 paths (Cars on these roads will be directed)
    {
        List<int> indices = new List<int>();

        int road1 = Random.Range(0, roads.Count);
        indices.Add(road1);

        int road2;

        // If path1 and path2 are not the same, add to list
        do
        {
            road2 = Random.Range(0, roads.Count);
        } 
        while (road2 == road1);

        indices.Add(road2);

        return indices;
    }

    private List<int> GetRemainingRoadIndices(List<int> selectedRoads) // Find the remaining paths (Destinations to navigate to)
    {
        // Find the remaining paths
        return roads
            .Select((road, index) => index)
            .Where(index => !selectedRoads.Contains(index))
            .ToList();
    }

    private void RedirectCarsToOtherRoads(List<int> selectedRoads, List<int> remainingRoads) // Update selected and remaining paths and route targets each round
    {
        // Cars on the first selected path will be directed to the first remaining path.
        RedirectCars(selectedRoads[0], remainingRoads[0]);

        // Cars on the second selected path will be directed to the second remaining path.
        RedirectCars(selectedRoads[1], remainingRoads[1]);
    }

    private void RedirectCars(int fromRoad, int toRoad)
    {
        List<Transform> carsOnRoad = roads[fromRoad].cars;
        int carsToRedirect = Mathf.Min(2, carsOnRoad.Count); 
        List<Transform> randomCars = new List<Transform>();

        while (randomCars.Count < carsToRedirect)
        {
            int randomIndex = Random.Range(0, carsOnRoad.Count);

            if (!randomCars.Contains(carsOnRoad[randomIndex]))
            {
                randomCars.Add(carsOnRoad[randomIndex]);
            }
        }

        //Transform newTarget = roads[toRoad].target;
        Transform newTarget = roads[toRoad].NewTarget;

        foreach (var car in randomCars)
        {
            car.GetComponent<Car_State_Machine>().currentTarget = newTarget;
        }
    }
}
