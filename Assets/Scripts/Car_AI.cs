using Pathfinding;
using Pathfinding.Util;
using UnityEngine;

public class Car_AI : MonoBehaviour
{
    private Transform _currentTarget;
    private Transform _newTargetPosition;
    private AIDestinationSetter _destinationSetter;

    private void Awake()
    {
        _destinationSetter = GetComponent<AIDestinationSetter>();
    }

    void Start()
    {
    }

    void FixedUpdate()
    {
        var seeker = GetComponent<Seeker>();
        float disToTarget = Vector2.Distance(_destinationSetter.target.position, transform.position);

        Vector3 currentTarget = _destinationSetter.target.position;
        Vector3 backwardDirection = -transform.forward; // Ters yöndeki vektör

        if (disToTarget <= 1)
        {
            Vector3 newTargetPosition = currentTarget + (backwardDirection * 20f);

            _destinationSetter.target.position = newTargetPosition;
        }

        //var node = AstarPath.active.GetNearest(transform.position).node;
        //node.Walkable = false; // Aracýn geçtiði düðümü "yürümez" yap
    }
}
