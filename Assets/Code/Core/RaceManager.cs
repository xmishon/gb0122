using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using UnityEngine.SceneManagement;
using Photon.Pun;

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
            InstantiatePlayers();
            InstantiateUI();
        }
    }

    private void InstantiatePlayers()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            string carId = (string)PhotonNetwork.PlayerList[i].CustomProperties["carId"];
            if (PhotonNetwork.PlayerList[i].IsLocal)
                InstantiateCar(carId, i, PhotonNetwork.PlayerList[i].IsLocal);
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

    private void InstantiateCar(string carId, int carPosition, bool isMine)
    {
        GameObject car = PhotonNetwork.Instantiate(carId,
            _startPoints[carPosition].transform.position,
            _startPoints[carPosition].transform.rotation);
        if (isMine)
        {
            _car = car;
            _car.tag = "Player";
            _camera.FindAndTargetPlayer();
            _car.GetComponent<Car.CarUserControl>().enabled = true;
        }
        else
        {
            car.tag = "Enemy";
            car.GetComponent<Car.CarUserControl>().enabled = false;
        }
    }
    private void InstantiateUI()
    {
        GameObject gameObject = Instantiate(Resources.Load<GameObject>("UI/GameUI"));
        GameUI gameUi = gameObject.GetComponent<GameUI>();
        gameUi.InitializeUI(_car.GetComponent<Car.CarController>());
    }
}
