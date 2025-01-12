using UnityEngine;

public class Car : MonoBehaviour, ICar
{
    [SerializeField] private Color _color;
    [SerializeField] private Material _material;

    private void Start()
    {
        tag = "Untagged";

        if(transform.childCount > 0)
        {
            Transform firstChild = transform.GetChild(0);
            firstChild.tag = "Car";
        }
    }
    public void Color()
    {
        _material.color = _color;
    }
}
