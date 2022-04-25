using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireSlipIndecatorPanel : MonoBehaviour
{
    private Car.CarController _carController;
    private List<WheelCollider> _frontLeftWheels;
    private List<WheelCollider> _frontRightWheels;
    private List<WheelCollider> _backLeftWheels;
    private List<WheelCollider> _backRightWheels;

    public void Initialize(List<WheelCollider> frontLeft,
        List<WheelCollider> frontRight,
        List<WheelCollider> backLeft,
        List<WheelCollider> backRight)
    {
        _frontLeftWheels = frontLeft;
        _frontRightWheels = frontRight;
        _backLeftWheels = backLeft;
        _backRightWheels = backRight;
    }
}
