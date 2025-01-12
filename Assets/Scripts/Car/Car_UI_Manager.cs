using Pathfinding.Examples;
using UnityEngine;

public class Car_UI_Manager : MonoBehaviour
{
    public static Car_UI_Manager Instance;

    [SerializeField] private GameObject _buttonsPanel;

    Car_State_Machine _carManager;

    [SerializeField] private SmoothCameraFollow _smoothCamera;

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

        if (_smoothCamera.isFreeMode)
        {
            _buttonsPanel.SetActive(false);
        }
        else if(!_smoothCamera.isFreeMode && _smoothCamera.target != null)
        {
            _buttonsPanel.SetActive(true);
            SetTargetCar(_smoothCamera.GetTarget());
        }
        else
        {
            _buttonsPanel.SetActive(false);
        }
    }


    public void SetTargetCar(Transform car)
    {
        if (car == null)
        {
            _carManager = null;
            return;
        }

        _carManager = car.GetComponentInParent<Car_State_Machine>();
    }

    void TogglePanel()
    {
        _buttonsPanel.SetActive(!_buttonsPanel.activeSelf);
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
