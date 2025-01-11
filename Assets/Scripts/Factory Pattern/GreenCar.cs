using UnityEngine;

public class GreenCar : MonoBehaviour, ICar
{
    [SerializeField] private Color _color;

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }
    public void Color()
    {
        _meshRenderer.material.color = _color;
    }
}
