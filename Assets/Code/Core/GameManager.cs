using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class GameManager : MonoBehaviour
{
    [SerializeField] AutoCam _camera;
    [SerializeField] List<GameObject> _startPoint;
    // Start is called before the first frame update
    void Start()
    {
        if (GameCore.instance == null)
        {
            Debug.LogError("GameCore is not correct!\r\nUnable to load car information.");
        }
        else
        {
            GameObject car = Instantiate(Resources.Load<GameObject>(GameCore.instance.carSetup.currentCarId));
            car.transform.position = _startPoint[0].transform.position;
            car.transform.rotation = _startPoint[0].transform.rotation;
            _camera.FindAndTargetPlayer();
        }
    }
}
