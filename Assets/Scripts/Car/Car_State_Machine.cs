using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Car_State_Machine : MonoBehaviour
{
    [Header("STATES")]
    public Car_Base currentstate; // Current State
    public Wait_State waitState = new Wait_State();
    public Movement_State movementState = new Movement_State();
    public Slowdown_State slowdownState = new Slowdown_State();
    public Overtaking_State overTakingState = new Overtaking_State();

    [Header("STATE CONTROL")]
    public States currentShowState = States.movement;
    public DriverType currentDrivertype = DriverType.normal;

    [Header("LISTS")]
    public List<RoadSegment> roadSegments;
    private List<Transform> rightLanePoints = new List<Transform>(); // Sa� �eritteki noktalar
    private List<Transform> leftLanePoints = new List<Transform>();  // Sol �eritteki noktalar
    private List<Transform> targets = new List<Transform>();

    [Header("MOVEMENT SETTINGS")]
    public float detectionRange = 10f; // �n�ndeki arac� alg�lama mesafesi //
    public float currentSpeed;
    [SerializeField] private float _angrySpeed = 12f;
    [SerializeField] private float _normalSpeed = 6f;
    [SerializeField] private float _chillSpeed = 3f;
    public bool isOvertaking = false; 
    private bool isChangingLane = false; 
    private bool isOnRightLane = true; 
    private Transform overtakePoint;

    [Header("COMPONENTS")]
    public Transform currentTarget;
    private AIPath aiPath;
    private Transform currentClosestPoint;
    private Transform _newTargetPosition;
    private AIDestinationSetter _destinationSetter;


    private void Awake()
    {
        _destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
    }
    private void OnEnable()
    {
        
    }
    void Start()
    {
        StartCoroutine(DelayedFind());

        //aiPath.orientation = OrientationMode.YAxisForward;
        //aiPath.destination = AdjustPathToFlow(target).position; // Sa� �eritten ba�lat
        currentSpeed = _normalSpeed;
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

        //if (IsTargetInWrongDirection())
        //{
        //    SetCorrectTargetDirection();
        //}
        DriverStateControl();
    }

    void DriverStateControl()
    {
        switch (currentDrivertype)
        {
            case DriverType.angry:
                currentSpeed = _angrySpeed;
                aiPath.maxSpeed = currentSpeed;
                break;
            case DriverType.normal:
                currentSpeed = _normalSpeed;
                aiPath.maxSpeed = currentSpeed;
                break;
            case DriverType.chill:
                currentSpeed = _chillSpeed;
                aiPath.maxSpeed = currentSpeed;
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
        aiPath.canSearch = true; // Yol aramay� etkinle�tir

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
    }

    //void OnPathComplete(Path p)
    //{
    //    // Yolun her bir noktas�n� kontrol ederek geriye do�ru hareketleri engelleyin
    //    if (p.vectorPath.Count > 1)
    //    {
    //        for (int i = 0; i < p.vectorPath.Count - 1; i++)
    //        {
    //            // E�er bir sonraki nokta, agent�n mevcut konumundan geriye do�ru ise, bu noktay� yolun d���na ��kar�n
    //            if (Vector3.Dot(transform.forward, p.vectorPath[i + 1] - p.vectorPath[i]) < 0)
    //            {
    //                // Yeni bir yol hesaplay�n
    //                aiPath.SearchPath();
    //                return;
    //            }
    //        }
    //    }
    //}

    public void CarInFrontControl()
    {
        var seeker = GetComponent<Seeker>();
        // Mevcut �eritteki en yak�n noktay� bul
        List<Transform> currentLane = isOnRightLane ? rightLanePoints : leftLanePoints;
        Transform closestPoint = FindClosestPoint(currentLane);

        // En yak�n noktan�n indeksini bul
        int currentIndex = currentLane.IndexOf(closestPoint);

        // Hedef noktas�na yak�n olup olmad���n�z� kontrol edin
        float disToTarget = Vector2.Distance(currentTarget.position, transform.position);

        if (disToTarget <= 1)
        {
            int newIndex = currentIndex - 2;

            // E�er yeni indeks, listedeki son noktay� a�arsa, ba�a d�n
            if (newIndex < 0)
            {
                newIndex = currentLane.Count - 1; // Ba�lang�ca sar
            }

            currentTarget.position = currentLane[newIndex].position;
            isOnRightLane = !isOnRightLane; // �erit de�i�tir
        }

        //var node = AstarPath.active.GetNearest(transform.position).node;
        //node.Walkable = false; // Arac�n ge�ti�i d���m� "y�r�mez" yap

        if (currentTarget != null)
        {
            aiPath.destination = currentTarget.position;
        }

        // �n�ndeki arac� kontrol et
        if (IsCarInFront())
        {
            isChangingLane = true;
            isOnRightLane = false;

            //switchState(slowdownState);
            //switchState(overTakingState);
        }
        else
        {
            isChangingLane = false;
            aiPath.maxSpeed = currentSpeed;
        }
    }

    bool IsTargetInWrongDirection()
    {
        Vector3 targetDirection = (currentTarget.position - transform.position).normalized;
        Vector3 currentDirection = transform.forward;

        // E�er hedefin y�n�, arac�n y�n�ne tersse
        return Vector3.Dot(targetDirection, currentDirection) < 0;
    }

    void SetCorrectTargetDirection()
    {
        Vector3 targetDirection = (currentTarget.position - transform.position).normalized;
        float step = aiPath.maxSpeed * Time.deltaTime;  // Ad�m b�y�kl���
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirection), step);
    }
    Transform FindClosestPoint(List<Transform> points)
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

    // �n�nde araba var m�?
    public bool IsCarInFront()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f; // Ray'in ba�lad��� nokta

        if (Physics.Raycast(rayOrigin, transform.forward, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Car")) // �n�ndeki araba m�?
            {
                Debug.DrawRay(rayOrigin, transform.forward * detectionRange, Color.red);
                
                return true;
            }
        }

        Debug.DrawRay(rayOrigin, transform.forward * detectionRange, Color.green);
        return false;
    }

    public void StartOvertaking()
    {
        isOvertaking = true;

        // Sollama hedef noktas�n� bul (sol �erit �zerindeki en yak�n nokta)
        overtakePoint = FindClosestPoint(leftLanePoints);

        if (overtakePoint != null)
        {
            aiPath.destination = overtakePoint.position; // Sollama hedefine y�nel
        }
    }
    public void EndOvertaking()
    {
        isOvertaking = false;

        // Sa� �eritteki en yak�n noktaya d�n
        Transform returnPoint = FindClosestPoint(rightLanePoints);

        if (returnPoint != null)
        {
            aiPath.destination = returnPoint.position; // Sa� �eride geri d�n
        }
    }

    Transform AdjustPathToFlow(Transform target)
    {
        // Ak��a uygun en yak�n yolu bul
        RoadSegment closestSegment = null;
        float minDistance = Mathf.Infinity;

        foreach (RoadSegment segment in roadSegments)
        {
            if (!segment.oneWay || IsDirectionValid(transform.position, segment))
            {
                float distance = Vector3.Distance(transform.position, segment.startPoint.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestSegment = segment;
                }
            }
        }

        return closestSegment != null ? closestSegment.endPoint : target;
    }

    bool IsDirectionValid(Vector3 currentPosition, RoadSegment segment)
    {
        Vector3 direction = (segment.endPoint.position - segment.startPoint.position).normalized;
        Vector3 currentDirection = (segment.startPoint.position - currentPosition).normalized;
        return Vector3.Dot(direction, currentDirection) > 0; // Ak�� y�n�yle ayn� m�?
    }

    public Transform GetTarget()
    {
        return currentTarget;
    }

    public AIPath GetAIPath()
    {
        return aiPath;
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
        slowdown,
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
