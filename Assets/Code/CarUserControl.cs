using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Car
{
    [RequireComponent(typeof(CarController))]
    public class CarUserControl : MonoBehaviour
    {
        [SerializeField]
        private float _slowMotionFactor = 0.2f;

        private CarController _carController;
        private bool _slowMoution = false;

        void Awake()
        {
            _carController = GetComponent<CarController>();
        }


        void Update()
        {
            float h, v, handbrake = 0f;
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxis("Vertical");
            if (Input.GetKey(KeyCode.Space))
                handbrake = 1f;

            _carController.Move(h, v, v, handbrake);
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (_slowMoution)
                {
                    Time.timeScale = 1.0f;
                }
                else
                {
                    Time.timeScale = _slowMotionFactor;
                }
                _slowMoution = !_slowMoution;
            }
        }
    }
}