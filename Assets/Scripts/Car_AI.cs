using Pathfinding;
using Pathfinding.Util;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class Car_AI : MonoBehaviour
{
    public List<Transform> rightLanePoints; // Sað þeritteki noktalar
    public List<Transform> leftLanePoints;  // Sol þeritteki noktalar
    public Transform target; // Hedef pozisyon
    public float detectionRange = 10f; // Önündeki aracý algýlama mesafesi //

    public float currentSpeed;

    private bool isChangingLane = false; // Þerit deðiþtirme iþlemi devam ediyor mu?w
    private bool isOnRightLane = true; // Aracýn sað þeritte olup olmadýðýný takip edin

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
    }

    void FixedUpdate()  
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
            aiPath.maxSpeed = 4;
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
    bool IsCarInFront()
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
}
