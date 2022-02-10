﻿using UnityEngine;

namespace Car { 

    public class CarController : MonoBehaviour
    {
        public WheelCollider[] frontWheelCollider;
        public WheelCollider[] rearWheelCollider;
        public GameObject[] frontWheelMesh;
        public GameObject[] rearWheelMesh;
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
        //private float steerAngleCorrection = 0f;
        private float m_OldRotation;

        public float CurrentEngineRevs { get; private set; }
        public float InputAcceleration { get; private set; }
        public float InputBrake { get; private set; }
        public int GearNumber { get; private set; }
        public float MaxSpeed { get { return m_Topspeed; } }
        public float CurrentSpeed { get { return m_Rigidbody.velocity.magnitude * 2.23693629f; } }

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
            for (int i = 0; i < frontWheelCollider.Length; i++)
            {
                Quaternion quat;
                Vector3 position;
                frontWheelCollider[i].GetWorldPose(out position, out quat);
                frontWheelMesh[i].transform.position = position;
                frontWheelMesh[i].transform.rotation = quat;
            }
            for (int i = 0; i < rearWheelCollider.Length; i++)
            {
                Quaternion quat;
                Vector3 position;
                rearWheelCollider[i].GetWorldPose(out position, out quat);
                rearWheelMesh[i].transform.position = position;
                rearWheelMesh[i].transform.rotation = quat;
            }

            if(transform.GetComponent<Rigidbody>().velocity.magnitude < 1 && acceleration < 0)
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
            ApplyAcceleration(steerAngle, acceleration, brake, handbrake);
            AddDownForce();
            FillWheelInfo();
        }

        private void CalculateEngineRevs(int gearNumber)
        {
            float averageWheelRevs = 0;
            for (int i = 0; i < rearWheelCollider.Length; i++)
            {
                averageWheelRevs += rearWheelCollider[i].rpm;
            }
            averageWheelRevs /= (float) rearWheelCollider.Length;
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

        private void ApplyAcceleration(float steerAngle, float acceleration, float brake, float handbrake)
        {
            // apply acceleration
            for (int i = 0; i < rearWheelCollider.Length; i++)
                rearWheelCollider[i].motorTorque = acceleration * currentWheelsTorque / (float)rearWheelCollider.Length;
            // apply brakes
            for (int i = 0; i < rearWheelCollider.Length; i++)
                if (handbrake > 0)
                {
                    rearWheelCollider[i].brakeTorque = float.MaxValue * brakeTorque / (float)rearWheelCollider.Length;
                }
                else
                {
                    rearWheelCollider[i].brakeTorque = brake * brakeTorque / (float)rearWheelCollider.Length;
                }
            for (int i = 0; i < rearWheelCollider.Length; i++)
                frontWheelCollider[i].brakeTorque = brake * brakeTorque / (float)rearWheelCollider.Length;
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

            float fwdSpeed = Vector3.Dot(m_Rigidbody.velocity, transform.forward);
            float temp = Mathf.Log(Mathf.Abs(0.1f * fwdSpeed) + 0.5f) * 8f; // magic...
            float correction = temp > (maxSteerAngle - 1) ? (maxSteerAngle - 1) : temp;
            for (int i = 0; i < frontWheelCollider.Length; i++)
                frontWheelCollider[i].steerAngle = steerAngle * maxSteerAngle - steerAngle * correction;
        }

        private void CalculateWheelsTorque(int gearNumber)
        {
            // normalize revs and torque (set value between 0 and 1)
            if (revsExceed == 0)
            {
                float normalizedRevs = (CurrentEngineRevs - minEngineRevs) / (maxEngineRevs - minEngineRevs);
                float normalizedTorque = engineCharacteristic.Evaluate(normalizedRevs);
                currentWheelsTorque = gearFactorOnWheels[gearNumber] * maxEngineTorque * normalizedTorque;
            } else if (revsExceed == 1) // if engine revs has been increased very quickly we simulate resistance
            {
                currentWheelsTorque = -1;
            } else
            {
                currentWheelsTorque = 1;
            }
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

        private void FillWheelInfo()
        {
            frontWheelCollider[0].GetGroundHit(out WheelHit wheelHit);
            FwdSlipFL = wheelHit.sidewaysSlip;
            frontWheelCollider[1].GetGroundHit(out wheelHit);
            FwdSlipFR = wheelHit.sidewaysSlip;
            rearWheelCollider[0].GetGroundHit(out wheelHit);
            FwdSlipRL = wheelHit.sidewaysSlip;
            rearWheelCollider[1].GetGroundHit(out wheelHit);
            FwdSlipRR = wheelHit.sidewaysSlip;
        }
    }
}