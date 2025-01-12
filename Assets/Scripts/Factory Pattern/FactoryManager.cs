using UnityEngine;

public class FactoryManager : MonoBehaviour
{
    [SerializeField] private Factory[] _factories;
    [SerializeField] private Transform[] _spawnPoints;

    private Factory _factory;
    void Start()
    {
        SpawnCars();
    }

    void SpawnCars()
    {
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            _factory = _factories[Random.Range(0, _factories.Length)];
            _factory.ApllyCarColor(_spawnPoints[i]);
        }
    }
}
