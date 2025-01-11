using Pathfinding;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Car_State_Machine : MonoBehaviour
{
    [Header("STATES")]
    public Car_Base currentstate; // Current State
    public Movement_State movementState = new Movement_State();
    public Slowdown_State slowdownState = new Slowdown_State();
    public Overtaking_State overTakingState = new Overtaking_State();

    [Header("STATE CONTROL")]
    public States currentShowState = States.movement; // Eklendi (State'leri inpsectorde gormek icin)

    [SerializeField] private string _rightLaneTag = "rightLane";
    [SerializeField] private string _leftLaneTag = "leftLane";

    private List<Transform> rightLanePoints; // Sa� �eritteki noktalar
    private List<Transform> leftLanePoints;  // Sol �eritteki noktalar

    public Transform currentTransform;
    [SerializeField]  private List<Transform> targets; // Hedef pozisyon
    public float detectionRange = 10f; // �n�ndeki arac� alg�lama mesafesi //
    public List<RoadSegment> roadSegments; // T�m yol segmentleri

    public float currentSpeed;

    private bool isChangingLane = false; // �erit de�i�tirme i�lemi devam ediyor mu?w
    private bool isOnRightLane = true; // Arac�n sa� �eritte olup olmad���n� takip edin
    public bool isOvertaking = false; // Sollama durumunu kontrol etmek i�in bayrak
    private Transform overtakePoint;  // Sollama hedef noktas�

    [Header("COMPONENTS")]
    [SerializeField] private AIPath aiPath;
    private Transform currentClosestPoint;
    public Transform _currentTarget;
    private Transform _newTargetPosition;
    private AIDestinationSetter _destinationSetter;


    private void Awake()
    {
        UpdateTransformList();
        _destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();

        GameObject[] targetWithTag = GameObject.FindGameObjectsWithTag("Target");

        foreach (GameObject obj in targetWithTag)
        {
            targets.Add(obj.transform);
        }
    }
    void Start()
    {
        //int randomTargetIndex = Random.Range(0, targets.Count);
        //currentTransform = targets[randomTargetIndex];
        aiPath.destination = currentTransform.position;
        aiPath.canSearch = true; // Yol aramay� etkinle�tir

        //aiPath.orientation = OrientationMode.YAxisForward;
        //aiPath.destination = AdjustPathToFlow(target).position; // Sa� �eritten ba�lat

        switchState(movementState);
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
    }

    public void Movement()
    {
        if (_currentTarget != null)
        {
            aiPath.destination = _currentTarget.position;
        }

    }

    public void UpdateTransformList()
    {
        GameObject[] rightLaneWithTag = GameObject.FindGameObjectsWithTag(_rightLaneTag);
        GameObject[] lefttLaneWithTag = GameObject.FindGameObjectsWithTag(_leftLaneTag);

        foreach (GameObject obj in rightLaneWithTag)
        {
            rightLanePoints.Add(obj.transform);
        }

        foreach (GameObject obj in lefttLaneWithTag)
        {
            leftLanePoints.Add(obj.transform);
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
        float disToTarget = Vector2.Distance(_currentTarget.position, transform.position);

        if (disToTarget <= 1)
        {
            int newIndex = currentIndex - 2;

            // E�er yeni indeks, listedeki son noktay� a�arsa, ba�a d�n
            if (newIndex < 0)
            {
                newIndex = currentLane.Count - 1; // Ba�lang�ca sar
            }

            _currentTarget.position = currentLane[newIndex].position;
            isOnRightLane = !isOnRightLane; // �erit de�i�tir
        }

        //var node = AstarPath.active.GetNearest(transform.position).node;
        //node.Walkable = false; // Arac�n ge�ti�i d���m� "y�r�mez" yap

        if (_currentTarget != null)
        {
            aiPath.destination = _currentTarget.position;
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
        Vector3 targetDirection = (_currentTarget.position - transform.position).normalized;
        Vector3 currentDirection = transform.forward;

        // E�er hedefin y�n�, arac�n y�n�ne tersse
        return Vector3.Dot(targetDirection, currentDirection) < 0;
    }

    void SetCorrectTargetDirection()
    {
        Vector3 targetDirection = (_currentTarget.position - transform.position).normalized;
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
        return _currentTarget;
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
        movement,
        slowdown,
        overtaking
    } // (States show on inspector)




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
