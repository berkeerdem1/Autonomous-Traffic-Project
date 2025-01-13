using Pathfinding.Examples;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Unity.VisualScripting;
using UnityEngine;

public class Car_UI_Manager : MonoBehaviour
{
    public static Car_UI_Manager Instance;

    [SerializeField] private GameObject _buttonsPanel;
    [SerializeField] private SmoothCameraFollow _smoothCamera;
    Car_State_Machine _carManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _smoothCamera = FindFirstObjectByType<SmoothCameraFollow>();
    }
    void Start()
    {
        _buttonsPanel.SetActive(false);
    }

    private void FixedUpdate()
    {
        // If smooth camera has a target and is not in free mode,
        // open the buttons panel and set target
        // If in free mode, close the panel

        if (_smoothCamera.isFreeMode) _buttonsPanel.SetActive(false);

        else if(!_smoothCamera.isFreeMode && _smoothCamera.target != null)
        {
            _buttonsPanel.SetActive(true);
            SetTargetCar(_smoothCamera.GetTarget());
        }

        else _buttonsPanel.SetActive(false);
    }

    public void SetTargetCar(Transform car) // Changes the target object that the UI will use
    {
        if (car == null)
        {
            _carManager = null;
            return;
        }

        _carManager = car.GetComponentInParent<Car_State_Machine>();
    }

    public void AngryDriverButton()
    {
        _carManager.currentDrivertype = _carManager.Angry();
    }
    public void NormalDriverButton()
    {
        _carManager.currentDrivertype = _carManager.Normal();
    }
    public void ChillDriverButton()
    {
        _carManager.currentDrivertype = _carManager.Chill();
    }
}
