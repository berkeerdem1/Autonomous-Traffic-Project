using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.ConstrainedExecution;
using Unity.Mathematics;
using UnityEngine;

public class Car_State_Machine : MonoBehaviour
{
    [Header("STATES")]
    public Car_Base currentstate; // Current State
    public Wait_State waitState = new Wait_State();
    public Movement_State movementState = new Movement_State();
    public Stop_State stopState = new Stop_State();
    public Overtaking_State overTakingState = new Overtaking_State();

    [Header("STATE CONTROL")]
    public States currentShowState = States.movement;
    public DriverType currentDrivertype = DriverType.normal;

    [Header("LISTS")]
    private List<Transform> rightLanePoints = new List<Transform>(); // Sað þeritteki noktalar
    private List<Transform> leftLanePoints = new List<Transform>();  // Sol þeritteki noktalar
    private List<Transform> targets = new List<Transform>();

    [Header("MOVEMENT SETTINGS")]
    public float detectionRange = 10f; // Önündeki aracý algýlama mesafesi //
    public float _currentSpeed = 6f;
    public float _slowDownSpeed = 0.1f;
    [SerializeField] private float _angrySpeed = 12f;
    [SerializeField] private float _normalSpeed = 6f;
    [SerializeField] private float _chillSpeed = 3f;
    public bool isOvertaking = false; 
    private bool isChangingLane = false; 
    private bool isOnRightLane = true;
    private float distanceToCar;
    [SerializeField] private float slowDownDuration = 2f;
    private float slowDownTimer = 0f;

    [Header("COMPONENTS")]
    public Transform currentTarget;
    public AIPath aiPath;
    private Transform currentClosestPoint;
    private Transform _newTargetPosition;
    private Transform overtakePoint;
    private AIDestinationSetter _destinationSetter;
    private Traffic_AI _trafficAI;

    private void Awake()
    {
        _destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        _trafficAI = GetComponent<Traffic_AI>();
    }
    private void OnEnable()
    {
        
    }
    void Start()
    {
        StartCoroutine(DelayedFind());

        //aiPath.destination = FindClosestPoint(rightLanePoints).position;
        
        currentDrivertype = DriverType.normal;
        switchState(waitState);
    }

    void Update()
    {
        currentstate.updateState(this);
    }

    private void FixedUpdate()
    {
        currentstate.fixedUpdateState(this);

        CarInFrontControl();

        aiPath.maxSpeed = _currentSpeed;
    }

    public void DriverStateControl()
    {
        switch (currentDrivertype)
        {
            case DriverType.angry:
                if(!IsCarInFront()) _currentSpeed = _angrySpeed;
                break;
            case DriverType.normal:
                if (!IsCarInFront()) _currentSpeed = _normalSpeed;
                break;
            case DriverType.chill:
                if (!IsCarInFront()) _currentSpeed = _chillSpeed;
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
            aiPath.destination = currentTarget.position;
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
            targets.Add(obj.transform);
        }

        int randomTargetIndex = UnityEngine.Random.Range(0, targets.Count);
        currentTarget = targets[randomTargetIndex];
        aiPath.destination = currentTarget.position;

        try
        {
            foreach (GameObject obj in rightLaneWithTag)
            {
                rightLanePoints.Add(obj.transform);
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
                leftLanePoints.Add(obj.transform);
            }
        }
        catch (NullReferenceException ex)
        {
            Debug.LogError($"NullReferenceException in left lane: {ex.Message}");
        }

        aiPath.destination = FindClosestPoint(rightLanePoints).position;
        aiPath.canSearch = true; 

        if (_trafficAI != null) _trafficAI.SetTarget(currentTarget);
    } // It pulls the necessary references from the ready Object Pool into its lists

    public void ChangeTarget()
    {
        List<Transform> currentLane = isOnRightLane ? rightLanePoints : leftLanePoints;
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
            isOnRightLane = !isOnRightLane; 
        }

        //var node = AstarPath.active.GetNearest(transform.position).node;
        //node.Walkable = false; 
    }

    public void CarInFrontControl()
    {
        if (currentTarget != null)
        {
            aiPath.destination = currentTarget.position;
        }
        
        if (IsCarInFront())
        {
            if (distanceToCar < 6f) 
            {
                //switchState(stopState);
            }
            else
            {
                if (_currentSpeed > 0.51f)
                {
                    _currentSpeed -= 0.05f;
                }
            }
            
            //switchState(overTakingState);
        }
        else
        {
            isChangingLane = false;
            aiPath.maxSpeed = _currentSpeed;
            //switchState(movementState);
        }
    }

    public Transform FindClosestPoint(List<Transform> points)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform point in points)
        {
            float distance = Vector3.Distance(transform.position, point.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = point;
            }
        }

        return closest;
    }

    public bool IsCarInFront()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, transform.forward, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Car"))
            {
                Debug.DrawRay(rayOrigin, transform.forward * detectionRange, Color.red);
                distanceToCar = Vector3.Distance(transform.position, hit.point);
                return true;
            }
        }

        Debug.DrawRay(rayOrigin, transform.forward * detectionRange, Color.green);
        return false;
    }

    public void StartOvertaking()
    {
        isOvertaking = true;

        overtakePoint = FindClosestPoint(leftLanePoints);

        if (overtakePoint != null)
        {
            aiPath.destination = overtakePoint.position; 
        }
    }
    public void EndOvertaking()
    {
        isOvertaking = false;

        Transform returnPoint = FindClosestPoint(rightLanePoints);

        if (returnPoint != null)
        {
            aiPath.destination = returnPoint.position; 
        }
    }

    public Transform GetTarget()
    {
        return currentTarget;
    }
    public AIPath GetAIPath()
    {
        return aiPath;
    }
    public List<Transform> GetRightLinePoints()
    {
        return rightLanePoints;
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
    }

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
