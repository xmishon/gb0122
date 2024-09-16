using TMPro;
using UnityEngine;


public class GearNumberIndicator
{
    private TextMeshProUGUI _gearNumberText;
    private Car.CarController _carController;

    public GearNumberIndicator(TextMeshProUGUI text, Car.CarController carController)
    {
        _gearNumberText = text;
        _carController = carController;
    }

    public void UpdateGearNumber()
    {
        if (_carController.GearNumber == _carController.gearFactor.Length - 1)
        {
            _gearNumberText.text = "R";
        }
        else if (_carController.GearNumber == 0)
        {
            _gearNumberText.text = "N";
        }
        else
        {
            _gearNumberText.text = _carController.GearNumber.ToString();
        }
    }
}

