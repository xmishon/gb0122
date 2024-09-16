using UnityEngine;
using UnityEngine.UI;

public class Tachometer
{
    private GameObject _arrow;
    private float _revsMax;
    private float _arrowAngleMin;
    private float _arrowAngleRange;
    private Car.CarController _carController;

    public Tachometer(GameObject arrow, float revsMin, float revsMax, float arrowAngleMin, float arrowAngleMax, Car.CarController carController)
    {
        _arrow = arrow;
        _revsMax = revsMax;
        _arrowAngleMin = arrowAngleMin;
        _arrowAngleRange = arrowAngleMax - arrowAngleMin;
        _carController = carController;
    }

    public void SetArrowPosition()
    {
        float targetAngle = _arrowAngleMin + _arrowAngleRange * _carController.CurrentEngineRevs / _revsMax;
        _arrow.transform.rotation = Quaternion.Euler(0.0f, 0.0f, targetAngle);
    }
}
