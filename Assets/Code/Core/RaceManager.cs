using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class RaceManager : MonoBehaviour
{
    [SerializeField] private AutoCam _camera;
    [SerializeField] private GameObject[] _trackConfigs;
    
    private List<GameObject> _startPoints;

    private void Start()
    {
        if (GameCore.instance == null)
        {
            Debug.LogError("GameCore is not correct!\r\nUnable to load car information.");
        }
        else
        {
            InstantiateCar();
        }
    }

    private void LoadTrackConfing()
    {

    }

    private void FindSpawntPoints()
    {
        _startPoints = FindObjectOfType<SpawnPoints>().spawnPoints;
    }

    private void InstantiateCar()
    {
        GameObject car = Instantiate(Resources.Load<GameObject>(GameCore.instance.carSetup.currentCarId));
        car.transform.position = _startPoints[0].transform.position;
        car.transform.rotation = _startPoints[0].transform.rotation;
        _camera.FindAndTargetPlayer();
    }
}
