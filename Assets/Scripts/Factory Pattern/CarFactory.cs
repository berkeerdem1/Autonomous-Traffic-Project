using UnityEngine;

public class CarFactory : Factory
{
    [SerializeField] private Car _redCarPrefab;
    public override ICar ApllyCarColor(Transform pos)
    {
        GameObject redCarInstane = Instantiate(_redCarPrefab.gameObject, pos.position, Quaternion.identity);
        Car newCar = redCarInstane.GetComponent<Car>();

        newCar.Color();

        return newCar;
    }
}
