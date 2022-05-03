using UnityEngine;

namespace Car { 

    public class CarController : MonoBehaviour
    {
        public WheelCollider[] frontLeftWheelColliders;
        public WheelCollider[] frontRightWheelColliders;
        public WheelCollider[] rearLeftWheelColliders;
        public WheelCollider[] rearRightWheelColliders;

        public GameObject[] frontLeftWheelMesh;
        public GameObject[] frontRightWheelMesh;
        public GameObject[] backLeftWheelMesh;
        public GameObject[] backRightWheelMesh;
        public float[] gearFactor = { 0f, 3.5f, 2.273f, 1.531f, 1.122f, 1.076f, 0.951f, 0.795f, -4.17f };
        public float mainFactor = 4.37f;
        public float maxSteerAngle = 25f;
        public float maxEngineTorque = 250f;
        public float brakeTorque = 2500f;
        public float minEngineRevs = 1000;
        public float maxEngineRevs = 6500;
        public float maxEngineAngularIncreaseVel = 15000; // revs per second
        public float maxEngineAngularDecreaseVel = 10000; // revs per second
        public AnimationCurve engineCharacteristic;
        public float m_Downforce = 100f;
        [Range(0, 1)] [SerializeField] private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing

        private float currentWheelsTorque;
        private float[] gearFactorOnWheels;
        private Quaternion[] frontWheelMeshLocalRotation;
        private Quaternion[] rearWheelMeshLocalRotation;
        private float maxEngineRevsIncreasePerTimeStep;
        private float maxEngineRevsDecreasePerTimeStep;
        private sbyte revsExceed; // 0 - revs weren't exceeded, 1 - revs increased too fast, -1 - revs fell too fast
        private float m_Topspeed = 230f;
        private Rigidbody m_Rigidbody;
        [SerializeField] private float m_SlipLimit = 0.2f;
        [SerializeField] private float _steeringSensetivity = 1.0f;
        private float _currentSteerAngle;
        private float _steerTarget;
        private float m_OldRotation;

        public float CurrentEngineRevs { get; private set; }
        public float InputAcceleration { get; private set; }
        public float InputBrake { get; private set; }
        public int GearNumber { get; private set; }
        public float MaxSpeed { get { return m_Topspeed; } }
        public float CurrentSpeed { get { return m_Rigidbody.velocity.magnitude * 3.6f; } }//2.23693629f; } }

        public float FwdSlipFL { get; private set; }
        public float FwdSlipFR { get; private set; }
        public float FwdSlipRL { get; private set; }
        public float FwdSlipRR { get; private set; }

        // Start is called before the first frame update
        void Awake()
        {
            // calculate gear factors, applied to wheels. Do it once at the start of the scene
            gearFactorOnWheels = new float[gearFactor.Length];
            for (int i = 0; i < gearFactor.Length; i++)
            {
                gearFactorOnWheels[i] = mainFactor * gearFactor[i];
            }
            CurrentEngineRevs = minEngineRevs;
            GearNumber = 0;
            maxEngineRevsIncreasePerTimeStep = maxEngineAngularIncreaseVel/(1/Time.fixedDeltaTime);
            maxEngineRevsDecreasePerTimeStep = maxEngineAngularDecreaseVel / (1 / Time.fixedDeltaTime);
            revsExceed = 0;
            m_Rigidbody = GetComponent<Rigidbody>();
        }
        
        public void Move(float steerAngle, float acceleration, float brake, float handbrake)
        {
            revsExceed = 0;
            if (acceleration > 0 && GearNumber == 0) GearNumber = 1;
            SwitchGear();

            CalculateEngineRevs(GearNumber);
            CalculateWheelsTorque(GearNumber);

            // set meshes position
            for (int i = 0; i < frontLeftWheelColliders.Length; i++)
            {
                Quaternion quat;
                Vector3 position;
                frontLeftWheelColliders[i].GetWorldPose(out position, out quat);
                frontLeftWheelMesh[i].transform.position = position;
                frontLeftWheelMesh[i].transform.rotation = quat;
            }
            for (int i = 0; i < frontRightWheelColliders.Length; i++)
            {
                Quaternion quat;
                Vector3 position;
                frontRightWheelColliders[i].GetWorldPose(out position, out quat);
                frontRightWheelMesh[i].transform.position = position;
                frontRightWheelMesh[i].transform.rotation = quat;
            }
            for (int i = 0; i < rearLeftWheelColliders.Length; i++)
            {
                Quaternion quat;
                Vector3 position;
                rearLeftWheelColliders[i].GetWorldPose(out position, out quat);
                backLeftWheelMesh[i].transform.position = position;
                backLeftWheelMesh[i].transform.rotation = quat;
            }
            for (int i = 0; i < frontRightWheelColliders.Length; i++)
            {
                Quaternion quat;
                Vector3 position;
                rearRightWheelColliders[i].GetWorldPose(out position, out quat);
                backRightWheelMesh[i].transform.position = position;
                backRightWheelMesh[i].transform.rotation = quat;
            }

            if (transform.GetComponent<Rigidbody>().velocity.magnitude < 1 && acceleration < 0)
            {
                TurnOnBackwardGear();
            } else if (transform.GetComponent<Rigidbody>().velocity.magnitude < 1 && acceleration > 0)
            {
                ShiftGearDown();
            }
            if(GearNumber == gearFactor.Length - 1) // backward gear
            {
                float temp = acceleration;
                acceleration = -brake; ;
                brake = -temp;
            }

            InputAcceleration = acceleration = Mathf.Clamp(acceleration, 0, 1);
            InputBrake = brake = -1f * Mathf.Clamp(brake, -1, 0);

            SteerHelper();
            ApplyAcceleration(acceleration, brake, handbrake);
            ApplySteering(steerAngle);
            AddDownForce();
        }

        private void CalculateEngineRevs(int gearNumber)
        {
            float averageWheelRevs = 0;
            for (int i = 0; i < rearLeftWheelColliders.Length; i++)
            {
                averageWheelRevs += rearLeftWheelColliders[i].rpm;
            }
            for (int i = 0; i < rearRightWheelColliders.Length; i++)
            {
                averageWheelRevs += rearRightWheelColliders[i].rpm;
            }
            averageWheelRevs /= (rearLeftWheelColliders.Length + rearRightWheelColliders.Length);
            float engineRevs = averageWheelRevs * gearFactorOnWheels[gearNumber];
            //check if revs changed too quickly
            if ((engineRevs - CurrentEngineRevs) > maxEngineRevsIncreasePerTimeStep)
            {
                engineRevs = CurrentEngineRevs + maxEngineRevsIncreasePerTimeStep;
                revsExceed = 1;
            }
            if ((engineRevs - CurrentEngineRevs) < -maxEngineAngularDecreaseVel)
            {
                engineRevs = CurrentEngineRevs - maxEngineAngularIncreaseVel;
                revsExceed = -1;
            }
            if (engineRevs < minEngineRevs)
                engineRevs = minEngineRevs;
            if (engineRevs > maxEngineRevs)
                engineRevs = maxEngineRevs;
            CurrentEngineRevs = engineRevs;
        }

        private void ApplyAcceleration(float acceleration, float brake, float handbrake)
        {
            // apply acceleration
            for (int i = 0; i < rearLeftWheelColliders.Length; i++)
                rearLeftWheelColliders[i].motorTorque = acceleration * currentWheelsTorque / (float)rearLeftWheelColliders.Length / 2;
            for (int i = 0; i < frontRightWheelColliders.Length; i++)
                rearRightWheelColliders[i].motorTorque = acceleration * currentWheelsTorque / (float)rearRightWheelColliders.Length / 2;
            // apply brakes
            for (int i = 0; i < rearLeftWheelColliders.Length; i++)
                if (handbrake > 0)
                {
                    rearLeftWheelColliders[i].brakeTorque = float.MaxValue * brakeTorque / (float)rearLeftWheelColliders.Length / 2;
                }
                else
                {
                    rearLeftWheelColliders[i].brakeTorque = brake * brakeTorque / (float)rearLeftWheelColliders.Length / 2;
                }
            for (int i = 0; i < rearRightWheelColliders.Length; i++)
                if (handbrake > 0)
                {
                    rearRightWheelColliders[i].brakeTorque = float.MaxValue * brakeTorque / (float)rearRightWheelColliders.Length / 2;
                }
                else
                {
                    rearRightWheelColliders[i].brakeTorque = brake * brakeTorque / (float)rearRightWheelColliders.Length / 2;
                }
            for (int i = 0; i < frontLeftWheelColliders.Length; i++)
                frontLeftWheelColliders[i].brakeTorque = brake * brakeTorque / (float)frontLeftWheelColliders.Length / 2;
            for (int i = 0; i < frontRightWheelColliders.Length; i++)
                frontRightWheelColliders[i].brakeTorque = brake * brakeTorque / (float)frontRightWheelColliders.Length / 2;
            // apply steering
            // calculate steering coorrection, if the car moves sideways
            //float sideSpeed = Vector3.Dot(m_Rigidbody.velocity, transform.right); // calculate side speed
            //float steerCorrection = 0f;
            //if (sideSpeed > 0.2f)
            //    steerCorrection = Mathf.Clamp(sideSpeed / m_Rigidbody.velocity.magnitude, 0, 1);
            // if front wheel is sleeping, reduce steerAngle
            //frontWheelCollider[0].GetGroundHit(out WheelHit frontWheelHit1);
            //frontWheelCollider[1].GetGroundHit(out WheelHit frontWheelHit2);
            //float sideWaysSleep = Mathf.Max(frontWheelHit1.sidewaysSlip, frontWheelHit2.sidewaysSlip);
            //if (Mathf.Abs(sideWaysSleep) > m_SlipLimit)
            //{
            //    if (sideWaysSleep > 0)
            //        steerAngleCorrection += 0.04f;
            //    else
            //        steerAngleCorrection -= 0.04f;
            //} else
            //{
            //    if (steerAngleCorrection > -0.06f && steerAngleCorrection < 0.06f)
            //        steerAngleCorrection = 0;
            //    if (steerAngleCorrection > 0)
            //        steerAngleCorrection -= 0.01f;
            //    else if (steerAngleCorrection < 0)
            //        steerAngleCorrection += 0.01f;
            //}
            //steerAngleCorrection = Mathf.Clamp(steerAngleCorrection, -1f, 1f);
        }

        private void ApplySteering(float steerInput)
        {
            if (steerInput == 0)
            {
                if (m_Rigidbody.velocity == Vector3.zero)
                {
                    _steerTarget = 0.0f;
                } 
                else
                {
                    _steerTarget = Vector3.SignedAngle(transform.forward, m_Rigidbody.velocity, transform.up);
                }
            }
            _steerTarget += steerInput * maxSteerAngle;

            

            _currentSteerAngle += _steerTarget > _currentSteerAngle 
                ? _steeringSensetivity * Time.fixedDeltaTime 
                : -_steeringSensetivity * Time.fixedDeltaTime;
            _currentSteerAngle = Mathf.Clamp(_currentSteerAngle, -maxSteerAngle, maxSteerAngle);
            
            for (int i = 0; i < frontLeftWheelColliders.Length; i++)
                frontLeftWheelColliders[i].steerAngle = Mathf.Clamp(_currentSteerAngle, -maxSteerAngle, maxSteerAngle);
            for (int i = 0; i < frontRightWheelColliders.Length; i++)
                frontRightWheelColliders[i].steerAngle = Mathf.Clamp(_currentSteerAngle, -maxSteerAngle, maxSteerAngle);
        }

        private void CalculateWheelsTorque(int gearNumber)
        {
            // normalize revs and torque (set value between 0 and 1)
            //if (revsExceed == 0)
            //{
                float normalizedRevs = (CurrentEngineRevs - minEngineRevs) / (maxEngineRevs - minEngineRevs);
                float normalizedTorque = engineCharacteristic.Evaluate(normalizedRevs);
                currentWheelsTorque = gearFactorOnWheels[gearNumber] * maxEngineTorque * normalizedTorque;
            //} else if (revsExceed == 1) // if engine revs has been increased very quickly we simulate resistance
            //{
            //    currentWheelsTorque = -1;
            //} else
            //{
            //    currentWheelsTorque = 1;
            //}
        }

        private void SwitchGear()
        {
            if (GearNumber < gearFactor.Length - 1)
            {
                if (CurrentEngineRevs > (maxEngineRevs - 200))
                {
                    ShiftGearUp();
                }
                if (CurrentEngineRevs < (minEngineRevs + 1600))
                {
                    ShiftGearDown();
                }
            }
        }

        private void ShiftGearUp()
        {
            if (GearNumber < gearFactor.Length - 2)
            {
                GearNumber++;
            }
        }

        private void ShiftGearDown()
        {
            if (GearNumber > 1)
                GearNumber--;
        }

        private void TurnOnBackwardGear()
        {
            GearNumber = gearFactor.Length - 1;
        }

        // this is used to add more grip in relation to speed
        private void AddDownForce()
        {
            m_Rigidbody.AddForce(-transform.up * m_Downforce *
                                                         m_Rigidbody.velocity.magnitude);
        }

        private void SteerHelper()
        {
            //for (int i = 0; i < frontWheelCollider.Length; i++)
            //{
            //    WheelHit wheelhit;
            //    frontWheelCollider[i].GetGroundHit(out wheelhit);
            //    if (wheelhit.normal == Vector3.zero)
            //        return; // wheels arent on the ground so dont realign the rigidbody velocity
            //}

            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
            {
                var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
                Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
                m_Rigidbody.velocity = velRotation * (m_Rigidbody.velocity );
            }
            m_OldRotation = transform.eulerAngles.y;
        }
    }
}