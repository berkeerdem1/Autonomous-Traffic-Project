using UnityEngine;

public class Object_Pool : MonoBehaviour
{
    public static Object_Pool Instance;

    private GameObject[] _rightLineWithTag;
    private GameObject[] _leftLineWithTag;
    private GameObject[] _targets;
    private GameObject[] _roadSegments;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Finds objects in the scene by tag and adds them to arrays.
        InitializePool("rightLane", "leftLane");
        InitializeTargetsPool("Target");
        InitializeRoadSegmentsPool("RoadSegment");
    }

    public void InitializeRoadSegmentsPool(string tag)
    {
        _roadSegments = GameObject.FindGameObjectsWithTag(tag);
    }

    public void InitializeTargetsPool(string tag)
    {
        _targets = GameObject.FindGameObjectsWithTag(tag);
    }

    public void InitializePool(string tag1, string tag2)
    {
        _rightLineWithTag = GameObject.FindGameObjectsWithTag(tag1);
        _leftLineWithTag = GameObject.FindGameObjectsWithTag(tag2);
    }

    public GameObject[] GetRightLinesWithTag()
    {
        if (_rightLineWithTag == null)
        {
            Debug.LogError("Right lane objects are not initialized!");
            return null;
        }
        return _rightLineWithTag;
    }

    public GameObject[] GeteftLinesWithTag()
    {
        if (_leftLineWithTag == null)
        {
            Debug.LogError("Left lane objects are not initialized!");
            return null;
        }
        return _leftLineWithTag;
    }

    public GameObject[] GetTargets()
    {
        if (_targets == null)
        {
            Debug.LogError("targets objects are not initialized!");
            return null;
        }
        return _targets;
    }

    public GameObject[] GetRoadSegments()
    {
        if (_roadSegments == null)
        {
            Debug.LogError("roadSegments objects are not initialized!");
            return null;
        }
        return _roadSegments;
    }
}
