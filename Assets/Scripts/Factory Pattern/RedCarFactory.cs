using UnityEngine;

public class RedCarFactory : Factory
{
    [SerializeField] private RedCar _redCarPrefab;
    [SerializeField] private Transform _mySpawnPosition;
    public override ICar ApllyCarColor()
    {
        GameObject redCarInstane = Instantiate(_redCarPrefab.gameObject, _mySpawnPosition.position, Quaternion.identity);
        RedCar newCar =  GetComponent<RedCar>();

        newCar.Color();

        return newCar;
    }
}
