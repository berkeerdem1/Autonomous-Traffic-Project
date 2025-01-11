using UnityEngine;

public class GreenCarFactory : Factory
{
    [SerializeField] private GreenCar _greenCarPrefab;  //
    [SerializeField] private Transform _mySpawnPosition;  //
    public override ICar ApllyCarColor()
    {
        GameObject greenCarInstane = Instantiate(_greenCarPrefab.gameObject, _mySpawnPosition.position, Quaternion.identity);
        GreenCar newCar = greenCarInstane.GetComponent<GreenCar>();
        newCar.Color();

        return newCar;
    }
}
