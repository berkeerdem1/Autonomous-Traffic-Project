using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public enum Lane { Right, Left }
public class Car_State_Machine : MonoBehaviour
{
    [Header("STATES")]
    public Car_Base currentstate; // Current State
    public Wait_State waitState = new Wait_State();
    public Movement_State movementState = new Movement_State();
    public Stop_State stopState = new Stop_State();
    public Overtaking_State overTakingState = new Overtaking_State();

    [Header("ENUMS")]
    public States currentShowState = States.movement;
    public DriverType currentDrivertype = DriverType.normal;
    private Lane currentLane = Lane.Right;

    private Dictionary<DriverType, int> weights = new Dictionary<DriverType, int>
    {
        { DriverType.angry, 1 },  
        { DriverType.normal, 5 },  
        { DriverType.chill, 10 }      
    };

    [Header("LISTS")]
    private List<Transform> _rightLanePoints = new List<Transform>();
    private List<Transform> _leftLanePoints = new List<Transform>();  
    private List<Transform> _targets = new List<Transform>();


    [Header("MOVEMENT SETTINGS")]
    public float detectionRange = 10f; // Car detection distance //
    public float currentSpeed = 6f;
    public float slowDownSpeed = 0.1f;
    [SerializeField] private float _angrySpeed = 12f;
    [SerializeField] private float _normalSpeed = 6f;
    [SerializeField] private float _chillSpeed = 3f;
    [SerializeField] private float _slowDownDuration = 2f;
    public Transform _overTakingPos;
    [SerializeField] private Transform _endOverTakingPos;
    private float _slowDownTimer = 0f;
    private float _initialSpeed;


    [Header("BOOLS")]
    private bool _isOnRightLane = true;
    private float _distanceToCar;
    private bool _isAngryDriver = false;


    [Header("COMPONENTS")]
    public Transform currentTarget;
    private AIPath _aiPath;
    private Transform _overtakePoint;
    private AIDestinationSetter _destinationSetter;
    private Traffic_AI _trafficAI;

    private void Awake()
    {
        _destinationSetter = GetComponent<AIDestinationSetter>();
        _aiPath = GetComponent<AIPath>();
        _trafficAI = GetComponent<Traffic_AI>();
    }
    private void OnEnable()
    {
        
    }
    void Start()
    {
        StartCoroutine(DelayedFind());
        currentDrivertype = GetWeightedRandomDriverType();
        InvokeRepeating("IsCarInFront()", 0f, 0.2f);

        switchState(waitState);
    }

    void Update()
    {
        currentstate.updateState(this);
    }

    private void FixedUpdate()
    {
        currentstate.fixedUpdateState(this);
        _aiPath.maxSpeed = currentSpeed;
    }

    private DriverType GetWeightedRandomDriverType()
    {
        int totalWeight = 0;
        foreach (var weight in weights.Values)
        {
            totalWeight += weight;
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var pair in weights)
        {
            currentWeight += pair.Value;
            if (randomValue < currentWeight)
            {
                return pair.Key;
            }
        }

        return default;
    }

    public void DriverStateControl()
    {
        switch (currentDrivertype)
        {
            case DriverType.angry:
                if(!IsCarInFront()) currentSpeed = _angrySpeed;
                _isAngryDriver = true;
                break;

            case DriverType.normal:
                if (!IsCarInFront()) currentSpeed = _normalSpeed;
                _isAngryDriver = false;
                break;

            case DriverType.chill:
                if (!IsCarInFront()) currentSpeed = _chillSpeed;
                _isAngryDriver = false;
                break;
        }
    }

    public DriverType Angry()
    {
        currentDrivertype = DriverType.angry;
        return DriverType.angry;
    }
    public DriverType Normal()
    {
        currentDrivertype = DriverType.normal;
        return DriverType.normal;
    }
    public DriverType Chill()
    {
        currentDrivertype = DriverType.chill;
        return DriverType.chill;
    }

    public void Movement()
    {
        if (currentTarget != null)
        {
            _aiPath.destination = currentTarget.position;
        }
    }

    IEnumerator DelayedFind()
    {
        GameObject[] targetWithTag = Object_Pool.Instance.GetTargets();
        GameObject[] rightLaneWithTag = Object_Pool.Instance.GetRightLinesWithTag();
        GameObject[] lefttLaneWithTag = Object_Pool.Instance.GeteftLinesWithTag();

        yield return new WaitForSeconds(0.5f);

        foreach (GameObject obj in targetWithTag)
        {
            _targets.Add(obj.transform);
        }

        int randomTargetIndex = UnityEngine.Random.Range(0, _targets.Count);
        currentTarget = _targets[randomTargetIndex];
        _aiPath.destination = currentTarget.position;

        try
        {
            foreach (GameObject obj in rightLaneWithTag)
            {
                _rightLanePoints.Add(obj.transform);
            }
        }
        catch (NullReferenceException ex)
        {
            Debug.LogError($"NullReferenceException in right lane: {ex.Message}");
        }

        try
        {
            foreach (GameObject obj in lefttLaneWithTag)
            {
                _leftLanePoints.Add(obj.transform);
            }
        }
        catch (NullReferenceException ex)
        {
            Debug.LogError($"NullReferenceException in left lane: {ex.Message}");
        }

       // _aiPath.destination = FindClosestPoint(_rightLanePoints).position;
        _aiPath.canSearch = true; 

        if (_trafficAI != null) _trafficAI.SetTarget(currentTarget);
    } // It pulls the necessary references from the ready Object Pool into its lists

    public void ChangeTarget()
    {
        //List<Transform> currentLane = _isOnRightLane ? _rightLanePoints : _leftLanePoints;
        List<Transform> currentLane = _rightLanePoints;
        Transform closestPoint = FindClosestPoint(currentLane);

        int currentIndex = currentLane.IndexOf(closestPoint);
        float disToTarget = Vector2.Distance(currentTarget.position, transform.position);

        if (disToTarget <= 1)
        {
            int newIndex = currentIndex - 2;

            if (newIndex < 0)
            {
                newIndex = currentLane.Count - 1; // Loop
            }

            currentTarget.position = currentLane[newIndex].position;
            _isOnRightLane = !_isOnRightLane; 
        }

        //var node = AstarPath.active.GetNearest(transform.position).node;
        //node.Walkable = false; 
    } // If it reaches the target, move the target back 2 points.

    public void CarInFrontControl()
    {
        if (IsCarInFront())
        {
            if (_distanceToCar < 6f) 
            {
                switchState(stopState);

                if(_isAngryDriver)
                    switchState(overTakingState);
            }
            else
            {
                if (currentSpeed > 0.51f)
                {
                    _initialSpeed = currentSpeed;
                    currentSpeed -= 0.05f;
                }
            }
        }
        else
        {
            if(currentSpeed <= _initialSpeed)
                currentSpeed += 0.05f;

            _aiPath.maxSpeed = currentSpeed;
            //switchState(movementState);
        }
    }

    private Transform FindClosestPoint(List<Transform> points)
    {
        // Since the transform position information is in the Unity main Thread,
        // copy the positions to the array beforehand
        // Prefetches the positions of all points
        Vector3[] positions = points.Select(p => p.position).ToArray();
        Vector3 currentPosition = transform.position;

        int closestIndex = -1;
        float minDistance = Mathf.Infinity;

        // Splits processes independently and runs
        // these parts simultaneously on multiple processors
        // Use parallel processing just for distance calculation
        System.Threading.Tasks.Parallel.For(0, positions.Length, i =>
        {
            float distance = Vector3.Distance(currentPosition, positions[i]);
            lock (this)
            {
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }
        });

        return closestIndex >= 0 ? points[closestIndex] : null;
    }  // Finds the closest point from the Points array in the parameter with parallel processing


    public bool IsCarInFront()
    {
        // It sends a raycast from the front point and if 
        // it touches an object with the 'car' tag, 
        // it returns true and calculates the distance to the object.

        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, transform.forward, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Car"))
            {
                Debug.DrawRay(rayOrigin, transform.forward * detectionRange, Color.red);
                _distanceToCar = Vector3.Distance(transform.position, hit.point);
                return true;
            }
        }

        Debug.DrawRay(rayOrigin, transform.forward * detectionRange, Color.green);
        return false;
    } 


    public IEnumerator StartOvertaking()
    {
        float timer = 0f;

        while (timer <= 2f)
        {
            timer += Time.deltaTime;
            transform.position = transform.position + (transform.right * -0.02f);
            transform.position = transform.position + (transform.forward * + 0.1f);
            yield return null;
        }

        float elapsedTime = 0f;

        while (elapsedTime <= 1f)
        {
            elapsedTime += Time.deltaTime;
            transform.position = transform.position + (transform.forward * + 0.25f);
            yield return null;
        }

    } // Start Overtaking using FindClosestPoint(LeftLanePoints)
    public void EndOvertaking()
    {
        Vector3 overTakingPos = transform.position + (transform.right * 5);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    } // End Overtaking using FindClosestPoint(RightLanePoints)

    public Transform GetTarget()
    {
        return currentTarget;
    }
    public AIPath GetAIPath()
    {
        return _aiPath;
    }
    public List<Transform> GetRightLinePoints()
    {
        return _rightLanePoints;
    }
    public void switchState(Car_Base state) //State change func
    {
        currentstate = state;
        state.enterState(this);
    }
    public enum States
    {
        wait,
        movement,
        stop,
        overtaking
    } // (States show on inspector)
    public enum DriverType
    {
        angry,
        normal,
        chill
    } // To select and change drive types

    private void OnCollisionEnter2D(Collision2D collision)
    {
        currentstate.onCollisionEnter(this, collision);
    }
    private void OnCollisionExit2D(Collision2D collisionexit)
    {
        currentstate.onCollisionExit(this, collisionexit);
    }
    private void OnTriggerEnter2D(Collider2D trigger)
    {
        currentstate.onTriggerEnter(this, trigger);
    }
    private void OnTriggerStay2D(Collider2D trigger)
    {
        currentstate.onTriggerEnter(this, trigger);
    }
    private void OnTriggerExit2D(Collider2D triggerExit)
    {
        currentstate.onTriggerExit(this, triggerExit);
    }
}
