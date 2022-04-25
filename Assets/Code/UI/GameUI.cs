using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    private float _revsMin = 0;
    [SerializeField]
    private float _revsMax = 9000;
    [SerializeField]
    private float _arrowAngleMin = 221;
    [SerializeField]
    private float _arrowAngleMax = -38.0f;
    [SerializeField]
    private GameObject _arrow;
    [SerializeField]
    private TextMeshProUGUI _gearNumberText;
    [SerializeField]
    private TextMeshProUGUI _speedometerText;

    private GearNumberIndicator _gearNumberIndicator;
    private Tachometer _tachometer;
    private Speedometer _speedometer;
    private bool _initialized = false;

    public void InitializeUI(Car.CarController carController)
    {
        _gearNumberIndicator = new GearNumberIndicator(_gearNumberText, carController);
        _tachometer = new Tachometer(_arrow, _revsMin, _revsMax, _arrowAngleMin, _arrowAngleMax, carController);
        _speedometer = new Speedometer(_speedometerText, carController);
        _initialized = true;
    }

    private void Update()
    {
        if (!_initialized)
        {
            return;
        }
        _gearNumberIndicator.UpdateGearNumber();
        _tachometer.SetArrowPosition();
        _speedometer.UpdateSpeed();
    }
}