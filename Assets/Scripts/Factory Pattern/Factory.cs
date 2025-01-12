using UnityEngine;

public abstract class Factory : MonoBehaviour
{
    public abstract ICar ApllyCarColorandCreate(Transform pos);
}
