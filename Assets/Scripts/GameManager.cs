using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Road
{
    public Transform target; // Yolun hedefi //
    public List<Transform> cars; // Yoldaki arabalar�n listesi
}

public class GameManager : MonoBehaviour
{
    public List<Road> roads; // 4 yolun hedefleri ve arabalar�n� tutar
    public float targetChangeInterval = 30f;

    private void Start()
    {
        StartCoroutine(ChangeTargetsPeriodically());
    }

    private IEnumerator ChangeTargetsPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(targetChangeInterval);

            // 4 yoldan 2 rastgele yol se� (Bu yollardaki arabalar y�nlenecek)
            List<int> selectedRoads = GetTwoRandomRoadIndices();

            // Geriye kalan yollar� bul (Y�nlenecek hedefler)
            List<int> remainingRoads = GetRemainingRoadIndices(selectedRoads);

            // Her turda se�ilen ve kalan yollar� g�ncelle ve hedefleri y�nlendir
            RedirectCarsToOtherRoads(selectedRoads, remainingRoads);
        }
    }


    private List<int> GetTwoRandomRoadIndices()
    {

        List<int> indices = new List<int>();

        int road1 = Random.Range(0, roads.Count);
        indices.Add(road1);

        int road2;

        do
        {
            road2 = Random.Range(0, roads.Count);
        } 
        while (road2 == road1);

        indices.Add(road2);

        return indices;
    }

    private List<int> GetRemainingRoadIndices(List<int> selectedRoads)
    {
        // Geriye kalan yollar� bul
        return roads
            .Select((road, index) => index)
            .Where(index => !selectedRoads.Contains(index))
            .ToList();
    }

    private void RedirectCarsToOtherRoads(List<int> selectedRoads, List<int> remainingRoads)
    {
        // �lk se�ilen yoldaki arabalar ilk kalan yola y�nlendir
        RedirectCars(selectedRoads[0], remainingRoads[0]);

        // �kinci se�ilen yoldaki arabalar ikinci kalan yola y�nlendir
        RedirectCars(selectedRoads[1], remainingRoads[1]);
    }

    private void RedirectCars(int fromRoad, int toRoad)
    {
        List<Transform> carsOnRoad = roads[fromRoad].cars;

        int carsToRedirect = Mathf.Min(2, carsOnRoad.Count); // Maksimum 2 
        List<Transform> randomCars = new List<Transform>();

        while (randomCars.Count < carsToRedirect)
        {
            int randomIndex = Random.Range(0, carsOnRoad.Count);
            if (!randomCars.Contains(carsOnRoad[randomIndex]))
            {
                randomCars.Add(carsOnRoad[randomIndex]);
            }
        }

        Transform newTarget = roads[toRoad].target;

        foreach (var car in randomCars)
        {
            car.GetComponent<Car_State_Machine>().currentTarget = newTarget;
        }
    }
}
