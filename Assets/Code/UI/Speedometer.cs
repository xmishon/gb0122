using TMPro;
using UnityEngine;

public class Speedometer
{
    private TextMeshProUGUI _speedText;
    private Car.CarController _carController;

    public Speedometer(TextMeshProUGUI text, Car.CarController carController)
    {
        _speedText = text;
        _carController = carController;
    }

    public void UpdateSpeed()
    {
        _speedText.text = ((int)_carController.CurrentSpeed).ToString();
    }
}

