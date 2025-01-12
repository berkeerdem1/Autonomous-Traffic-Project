using UnityEngine;

public class Object_Pool : MonoBehaviour
{
    public static Object_Pool Instance;

    private GameObject[] rightLineWithTag;
    private GameObject[] leftLineWithTag;
    private GameObject[] targets;
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

        InitializePool("rightLane", "leftLane");
        InitializeTargetsPool("Target");
    }

    private void Start()
    {
    }

    public void InitializeTargetsPool(string tag)
    {
        targets = GameObject.FindGameObjectsWithTag(tag);

    }

    public void InitializePool(string tag1, string tag2)
    {
        rightLineWithTag = GameObject.FindGameObjectsWithTag(tag1);
        leftLineWithTag = GameObject.FindGameObjectsWithTag(tag2);

        Debug.Log($"Initialized pool with {rightLineWithTag.Length} right lane objects and {leftLineWithTag.Length} left lane objects.");
    }

    public GameObject[] GetRightLinesWithTag()
    {
        if (rightLineWithTag == null)
        {
            Debug.LogError("Right lane objects are not initialized!");
            return null;
        }
        return rightLineWithTag;
    }

    public GameObject[] GeteftLinesWithTag()
    {
        if (leftLineWithTag == null)
        {
            Debug.LogError("Left lane objects are not initialized!");
            return null;
        }
        return leftLineWithTag;
    }

    public GameObject[] GetTargets()
    {
        if (targets == null)
        {
            Debug.LogError("Left lane objects are not initialized!");
            return null;
        }
        return targets;
    }
}
