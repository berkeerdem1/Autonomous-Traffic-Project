using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class Car_State_Machine : MonoBehaviour
{
    [Header("STATES")]
    public Car_Base currentstate; // Current State
    public Movement_State movementState = new Movement_State();
    public Slowdown_State slowdownState = new Slowdown_State();
    public Overtaking_State overTakingState = new Overtaking_State();

    [Header("STATE CONTROL")]
    public States currentShowState = States.movement; // Eklendi (State'leri inpsectorde gormek icin)

    public List<Transform> rightLanePoints; // Sa� �eritteki noktalar
    public List<Transform> leftLanePoints;  // Sol �eritteki noktalar
    public Transform target; // Hedef pozisyon
    public float detectionRange = 10f; // �n�ndeki arac� alg�lama mesafesi //

    public float currentSpeed;

    private bool isChangingLane = false; // �erit de�i�tirme i�lemi devam ediyor mu?w
    private bool isOnRightLane = true; // Arac�n sa� �eritte olup olmad���n� takip edin

    [Header("COMPONENTS")]
    private AIPath aiPath;
    private Transform currentClosestPoint;
    private Transform _currentTarget;
    private Transform _newTargetPosition;
    private AIDestinationSetter _destinationSetter;


    private void Awake()
    {
        _destinationSetter = GetComponent<AIDestinationSetter>();
    }
    void Start()
    {
        aiPath = GetComponent<AIPath>();
        aiPath.destination = target.position; // AIPath hedefini ayarla
        aiPath.canSearch = true; // Yol aramay� etkinle�tir

        aiPath.destination = FindClosestPoint(rightLanePoints).position; // Sa� �eritten ba�lat

        switchState(movementState);
    }

    void Update()
    {
        currentstate.updateState(this);
    }

    private void FixedUpdate()
    {
        currentstate.fixedUpdateState(this);
    }

    public void CarInFrontControl()
    {
        var seeker = GetComponent<Seeker>();
        // Mevcut �eritteki en yak�n noktay� bul
        List<Transform> currentLane = isOnRightLane ? rightLanePoints : leftLanePoints;
        Transform closestPoint = FindClosestPoint(currentLane);

        // En yak�n noktan�n indeksini bul
        int currentIndex = currentLane.IndexOf(closestPoint);

        // Hedef noktas�na yak�n olup olmad���n�z� kontrol edin
        float disToTarget = Vector2.Distance(target.position, transform.position);

        if (disToTarget <= 1)
        {
            int newIndex = currentIndex - 2;

            // E�er yeni indeks, listedeki son noktay� a�arsa, ba�a d�n
            if (newIndex < 0)
            {
                newIndex = currentLane.Count - 1; // Ba�lang�ca sar
            }

            target.position = currentLane[newIndex].position;
            isOnRightLane = !isOnRightLane; // �erit de�i�tir
        }

        //var node = AstarPath.active.GetNearest(transform.position).node;
        //node.Walkable = false; // Arac�n ge�ti�i d���m� "y�r�mez" yap

        if (target != null)
        {
            aiPath.destination = target.position;
        }

        // �n�ndeki arac� kontrol et
        if (IsCarInFront())
        {
            isChangingLane = true;
            isOnRightLane = false;

            //switchState(slowdownState);
            switchState(overTakingState);
        }
        else
        {
            isChangingLane = false;
            aiPath.maxSpeed = currentSpeed;
        }
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
        // Sollama hedefi olu�turma
        Vector3 overtakePosition = transform.position + (transform.up * 2f); // Sola kayd�rma
        aiPath.destination = overtakePosition;
    }

    public AIPath AIPath()
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
