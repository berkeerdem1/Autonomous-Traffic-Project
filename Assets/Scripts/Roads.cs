using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class Roads : MonoBehaviour
{
    [SerializeField] private List<Transform> pointLists;

    public List<Transform> GetPoints()
    {
        return pointLists;
    }
}
