using UnityEngine;
using UnityEngine.UI;

public class TireSlipIndicator : MonoBehaviour
{
    [SerializeField]
    private Image _verticalIndicator;
    [SerializeField]
    private Image _horizontalIndicator;

    private WheelCollider _wheelCollider;
    private float _maxForwardSlip;
    private float _extremumForwardSlip;
    private float _maxSidewaysSlip;
    private float _extremumSidewaysSlip;

    public void Initialize(WheelCollider wheelCollider)
    {
        _wheelCollider = wheelCollider;
        _maxForwardSlip = wheelCollider.forwardFriction.asymptoteSlip;
        _extremumForwardSlip = wheelCollider.forwardFriction.extremumSlip;
        _maxSidewaysSlip = wheelCollider.sidewaysFriction.asymptoteSlip;
        _extremumSidewaysSlip = wheelCollider.sidewaysFriction.extremumSlip;
    }

    public void UpdateView()
    {
        if(_wheelCollider.GetGroundHit(out WheelHit hit))
        {
            // set vertical indicator
            if (Mathf.Abs(hit.forwardSlip) < _extremumForwardSlip)
                _verticalIndicator.color = Color.green;
            else if (Mathf.Abs(hit.forwardSlip) < _maxForwardSlip)
                _verticalIndicator.color = Color.yellow;
            else
                _verticalIndicator.color = Color.red;

            _verticalIndicator.transform.localScale = 
                new Vector3(1.0f, Mathf.Clamp(hit.forwardSlip / _maxForwardSlip, -1.0f, 1.0f), 1.0f);

            // set horizontal indicator
            if (Mathf.Abs(hit.sidewaysSlip) < _extremumSidewaysSlip)
                _horizontalIndicator.color = Color.green;
            else if (Mathf.Abs(hit.forwardSlip) < _maxSidewaysSlip)
                _horizontalIndicator.color = Color.yellow;
            else
                _horizontalIndicator.color = Color.red;

            _horizontalIndicator.transform.localScale =
                new Vector3(Mathf.Clamp(hit.sidewaysSlip / _maxSidewaysSlip, -1.0f, 1.0f), 1.0f, 1.0f);
        }

    }
}
