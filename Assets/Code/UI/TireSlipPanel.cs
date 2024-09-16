using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Car;

public class TireSlipPanel : MonoBehaviour
{
    [SerializeField]
    private Transform _frontLeftTiresPanel;
    [SerializeField]
    private Transform _frontRightTiresPanel;
    [SerializeField]
    private Transform _rearLeftTiresPanel;
    [SerializeField]
    private Transform _rearRightTiresPanel;

    private WheelCollider[] _frontLeftWheelColliders;
    private WheelCollider[] _frontRightWheelColliders;
    private WheelCollider[] _rearLeftWheelColliders;
    private WheelCollider[] _rearRightWheelColliders;

    private TireSlipIndicator[] _frontLeftIndicators;
    private TireSlipIndicator[] _frontRightIndicators;
    private TireSlipIndicator[] _rearLeftIndicators;
    private TireSlipIndicator[] _rearRightIndicators;

    public void Initialize(CarController carController)
    {
        _frontLeftWheelColliders = carController.frontLeftWheelColliders;
        _frontRightWheelColliders = carController.frontRightWheelColliders;
        _rearLeftWheelColliders = carController.rearLeftWheelColliders;
        _rearRightWheelColliders = carController.rearRightWheelColliders;

        _frontLeftIndicators = new TireSlipIndicator[_frontLeftWheelColliders.Length];
        _frontLeftTiresPanel.DeleteChildren();
        for (int i = 0; i < _frontLeftWheelColliders.Length; i++)
        {
            GameObject indicatorGO = Instantiate(Resources.Load<GameObject>("UI/TireSlipIndicator"));
            indicatorGO.transform.SetParent(_frontLeftTiresPanel);
            _frontLeftIndicators[i] = indicatorGO.GetComponent<TireSlipIndicator>();
            _frontLeftIndicators[i].Initialize(_frontLeftWheelColliders[i]);
        }

        _frontRightIndicators = new TireSlipIndicator[_frontRightWheelColliders.Length];
        _frontRightTiresPanel.DeleteChildren();
        for (int i = 0; i < _frontRightWheelColliders.Length; i++)
        {
            GameObject indicatorGO = Instantiate(Resources.Load<GameObject>("UI/TireSlipIndicator"));
            indicatorGO.transform.SetParent(_frontRightTiresPanel);
            _frontRightIndicators[i] = indicatorGO.GetComponent<TireSlipIndicator>();
            _frontRightIndicators[i].Initialize(_frontRightWheelColliders[i]);
        }

        _rearLeftIndicators = new TireSlipIndicator[_rearLeftWheelColliders.Length];
        _rearLeftTiresPanel.DeleteChildren();
        for (int i = 0; i < _rearLeftWheelColliders.Length; i++)
        {
            GameObject indicatorGO = Instantiate(Resources.Load<GameObject>("UI/TireSlipIndicator"));
            indicatorGO.transform.SetParent(_rearLeftTiresPanel);
            _rearLeftIndicators[i] = indicatorGO.GetComponent<TireSlipIndicator>();
            _rearLeftIndicators[i].Initialize(_rearLeftWheelColliders[i]);
        }

        _rearRightIndicators = new TireSlipIndicator[_rearRightWheelColliders.Length];
        _rearRightTiresPanel.DeleteChildren();
        for (int i = 0; i < _rearRightWheelColliders.Length; i++)
        {
            GameObject indicatorGO = Instantiate(Resources.Load<GameObject>("UI/TireSlipIndicator"));
            indicatorGO.transform.SetParent(_rearRightTiresPanel);
            _rearRightIndicators[i] = indicatorGO.GetComponent<TireSlipIndicator>();
            _rearRightIndicators[i].Initialize(_rearRightWheelColliders[i]);
        }
    }

    public void UpdateIndicators()
    {
        for (int i = 0; i < _frontLeftIndicators.Length; i++)
        {
            _frontLeftIndicators[i].UpdateView();
        }
        for (int i = 0; i < _frontRightIndicators.Length; i++)
        {
            _frontRightIndicators[i].UpdateView();
        }
        for (int i = 0; i < _rearLeftIndicators.Length; i++)
        {
            _rearLeftIndicators[i].UpdateView();
        }
        for (int i = 0; i < _rearRightIndicators.Length; i++)
        {
            _rearRightIndicators[i].UpdateView();
        }
    }
}
