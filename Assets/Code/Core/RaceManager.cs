using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    [SerializeField] private AutoCam _camera;
    [SerializeField] private GameObject[] _trackConfigs;
    
    private List<GameObject> _startPoints;
    private GameObject _car;

    private void Start()
    {
        if (GameCore.instance == null)
        {
            Debug.LogError("GameCore is not correct!\r\nUnable to load car information.");
        }
        else
        {
            LoadTrackConfing();
            FindSpawntPoints();
            InstantiateCar();
            InstantiateUI();
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(1);
        }
    }

    private void LoadTrackConfing()
    {
        Instantiate(_trackConfigs[GameCore.instance.trackSetup.trackConfigNum]);
    }

    private void FindSpawntPoints()
    {
        _startPoints = FindObjectOfType<SpawnPoints>().spawnPoints;
    }

    private void InstantiateCar()
    {
        _car = Instantiate(Resources.Load<GameObject>(GameCore.instance.carSetup.currentCarId));
        _car.transform.position = _startPoints[0].transform.position;
        _car.transform.rotation = _startPoints[0].transform.rotation;
        _camera.FindAndTargetPlayer();
    }
    private void InstantiateUI()
    {
        GameObject gameObject = Instantiate(Resources.Load<GameObject>("UI/GameUI"));
        GameUI gameUi = gameObject.GetComponent<GameUI>();
        gameUi.InitializeUI(_car.GetComponent<Car.CarController>());
    }
}
