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

    public List<Transform> rightLanePoints; // Sað þeritteki noktalar
    public List<Transform> leftLanePoints;  // Sol þeritteki noktalar
    public Transform target; // Hedef pozisyon
    public float detectionRange = 10f; // Önündeki aracý algýlama mesafesi //

    public float currentSpeed;

    private bool isChangingLane = false; // Þerit deðiþtirme iþlemi devam ediyor mu?w
    private bool isOnRightLane = true; // Aracýn sað þeritte olup olmadýðýný takip edin

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
        aiPath.canSearch = true; // Yol aramayý etkinleþtir

        aiPath.destination = FindClosestPoint(rightLanePoints).position; // Sað þeritten baþlat

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
        // Mevcut þeritteki en yakýn noktayý bul
        List<Transform> currentLane = isOnRightLane ? rightLanePoints : leftLanePoints;
        Transform closestPoint = FindClosestPoint(currentLane);

        // En yakýn noktanýn indeksini bul
        int currentIndex = currentLane.IndexOf(closestPoint);

        // Hedef noktasýna yakýn olup olmadýðýnýzý kontrol edin
        float disToTarget = Vector2.Distance(target.position, transform.position);

        if (disToTarget <= 1)
        {
            int newIndex = currentIndex - 2;

            // Eðer yeni indeks, listedeki son noktayý aþarsa, baþa dön
            if (newIndex < 0)
            {
                newIndex = currentLane.Count - 1; // Baþlangýca sar
            }

            target.position = currentLane[newIndex].position;
            isOnRightLane = !isOnRightLane; // Þerit deðiþtir
        }

        //var node = AstarPath.active.GetNearest(transform.position).node;
        //node.Walkable = false; // Aracýn geçtiði düðümü "yürümez" yap

        if (target != null)
        {
            aiPath.destination = target.position;
        }

        // Önündeki aracý kontrol et
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

    // Önünde araba var mý?
    public bool IsCarInFront()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f; // Ray'in baþladýðý nokta

        if (Physics.Raycast(rayOrigin, transform.forward, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Car")) // Önündeki araba mý?
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
        // Sollama hedefi oluþturma
        Vector3 overtakePosition = transform.position + (transform.up * 2f); // Sola kaydýrma
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
