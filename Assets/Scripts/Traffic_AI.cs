using PathCreation;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class Traffic_AI : MonoBehaviour
{
    private AIPath aiPath;
    private BezierPath bezierPath;
    private List<Vector3> pathPoints; // Bezier path noktalarý
    public Transform target; // Hedef pozisyon
    private Seeker seeker;  // Seeker'ý ekledik, yol arama iþini o yapacak.


    // Bezier Path için gerekli noktalar
    public Transform startPoint;
    public Transform endPoint;
    public Transform controlPoint1;
    public Transform controlPoint2;

    private void Awake()
    {
        aiPath = GetComponent<AIPath>();
        seeker = GetComponent<Seeker>(); // Seeker bileþenini alýyoruz
        aiPath.canSearch = true;
        aiPath.canMove = true;

        pathPoints = new List<Vector3>();
        CreateBezierPath();
    }

    void Update()
    {
        // Eðer hedef deðiþirse, yeni path hesaplama
        if (target != null && target.position != aiPath.destination)
        {
            aiPath.destination = target.position;
            StartPathfinding();  // Yeni yolu hesaplamak için baþlatýyoruz
        }
    }

    // Yeni yol hesaplamasýný baþlatma fonksiyonu
    void StartPathfinding()
    {
        // Seeker.StartPath kullanarak yolu hesaplatýyoruz
        ABPath path = ABPath.Construct(transform.position, target.position, null); // Baþlangýç ve hedef arasýnda yol hesaplatýyoruz
        seeker.StartPath(path); // Yolu baþlatýyoruz

        // Yol tamamlandýðýnda AIPath'e verelim
        path.immediateCallback = OnPathComplete;
    }

    // Yol tamamlandýðýnda AIPath'e yolu verme
    void OnPathComplete(Path p)
    {
        if (p.error)
        {
            Debug.LogError("Yol hesaplama hatasý!");
            return;
        }

        // Hesaplanan yolu AIPath'e veriyoruz
        aiPath.SetPath(p);
    }

    // Bezier Path'i oluþturma fonksiyonu
    void CreateBezierPath()
    {
        for (float t = 0; t <= 1; t += 0.1f)  // t parametresi ile path üzerinde nokta alýyoruz
        {
            Vector3 point = CalculateBezierPoint(t, startPoint.position, controlPoint1.position, controlPoint2.position, endPoint.position);
            pathPoints.Add(point);
        }
    }

    // Bezier Path hesaplama fonksiyonu (3. derece)
    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0; // (1-t)^3 * P0
        p += 3 * uu * t * p1; // 3(1-t)^2 * t * P1
        p += 3 * u * tt * p2; // 3(1-t) * t^2 * P2
        p += ttt * p3; // t^3 * P3

        return p;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
