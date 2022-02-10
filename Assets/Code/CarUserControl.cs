using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Car
{
    [RequireComponent(typeof(CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController _carController;

        void Awake()
        {
            _carController = GetComponent<CarController>();
        }


        void FixedUpdate()
        {
            float h, v, handbrake = 0f;
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            if (Input.GetKey(KeyCode.Space))
                handbrake = 1f;

            _carController.Move(h, v, v, handbrake);
        }
    }
}