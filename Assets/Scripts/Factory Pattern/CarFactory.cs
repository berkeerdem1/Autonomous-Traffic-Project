using UnityEngine;

public class CarFactory : Factory
{
    [SerializeField] private Car _redCarPrefab;
    public override ICar ApllyCarColorandCreate(Transform pos)
    {
        GameObject redCarInstane = Instantiate(_redCarPrefab.gameObject, pos.position, pos.rotation);
        Car newCar = redCarInstane.GetComponent<Car>();

        newCar.Color();

        return newCar;
    }
}
