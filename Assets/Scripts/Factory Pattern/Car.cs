using UnityEngine;

public class Car : MonoBehaviour, ICar
{
    [SerializeField] private Color _color;

    [SerializeField] private Material _material;

    public void Color()
    {
        _material.color = _color;
    }
}
