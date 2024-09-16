using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Car
{
    [RequireComponent(typeof(CarController))]
    public class CarSound : MonoBehaviour
    {
        // This script reads some of the car's current properties and plays sounds accordingly.
        // The engine sound must be a crossfaded blend of four clips which represent the timbre of the engine
        // lowAccelClip : The engine at low revs, with throttle open (i.e. begining acceleration at very low speed)
        // highAccelClip : Thenengine at high revs, with throttle open (i.e. accelerating, but almost at max speed)
        // lowDecelClip : The engine at low revs, with throttle at minimum (i.e. idling or engine-braking at very low speed)
        // highDecelClip : Thenengine at high revs, with throttle at minimum (i.e. engine-braking at very high speed)

        // For proper crossfading, the clips pitches should all match, with an octave offset between low and high.

        public AudioClip lowAccelClip;
        public AudioClip lowDecelClip;
        public AudioClip highAccelClip;
        public AudioClip highDecelClip;
        public float lowPitchMin = 1f;                                              // The lowest possible pitch for the low sounds
        public float lowPitchMax = 6f;                                              // The highest possible pitch for the low sounds
        public float maxRolloffDistance = 500;                                      // The maximum distance where rollof starts to take place

        private AudioSource m_LowAccel; // Source for the low acceleration sounds
        private AudioSource m_LowDecel; // Source for the low deceleration sounds
        private AudioSource m_HighAccel; // Source for the high acceleration sounds
        private AudioSource m_HighDecel; // Source for the high deceleration sounds
        private bool m_StartedSound; // flag for knowing if we have started sounds
        private CarController m_CarController; // Reference to car we are controlling

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // get the distance to main camera
            float camDist = (Camera.main.transform.position - transform.position).sqrMagnitude;

            // stop sound if the object is beyond the maximum roll off distance
            if (m_StartedSound && camDist > maxRolloffDistance * maxRolloffDistance)
            {
                StopSound();
            }

            // start the sound if not playing and it is nearer than the maximum distance
            if (!m_StartedSound && camDist < maxRolloffDistance * maxRolloffDistance)
            {
                StartSound();
            }
            if (m_StartedSound)
            {
                float pitch = (m_CarController.CurrentEngineRevs - m_CarController.minEngineRevs) /
                    (m_CarController.maxEngineRevs - m_CarController.minEngineRevs) * (lowPitchMax - lowPitchMin) + lowPitchMin;

                pitch = Mathf.Min(lowPitchMax, pitch);

                m_LowAccel.pitch = pitch;
                m_LowDecel.pitch = pitch;
                m_HighAccel.pitch = pitch * 0.25f;
                m_HighDecel.pitch = pitch * 0.25f;

                // get values for fading the sounds based on the acceleration
                float accFade = Mathf.Abs(m_CarController.InputAcceleration);
                float decFade = 1 - accFade;

                float normalizedRevs = (m_CarController.CurrentEngineRevs - m_CarController.minEngineRevs) / (m_CarController.maxEngineRevs - m_CarController.minEngineRevs);
                // get the high fade value based on the cars revs
                float highFade = Mathf.InverseLerp(0.2f, 0.8f, normalizedRevs);
                float lowFade = 1 - highFade;

                // adjust the values to be more realistic
                highFade = 1 - ((1 - highFade) * (1 - highFade));
                lowFade = 1 - ((1 - lowFade) * (1 - lowFade));
                accFade = 1 - ((1 - accFade) * (1 - accFade));
                decFade = 1 - ((1 - decFade) * (1 - decFade));

                // adjust the source volumes based on the fade values
                m_LowAccel.volume = lowFade * accFade;
                m_LowDecel.volume = lowFade * decFade;
                m_HighAccel.volume = highFade * accFade;
                m_HighDecel.volume = highFade * decFade;
            }
        }

        private void StartSound()
        {
            // get the carcontroller ( this will not be null as we have require component)
            m_CarController = GetComponent<CarController>();

            // setup audio sources
            m_HighAccel = SetUpEngineAudioSource(highAccelClip);
            m_LowAccel = SetUpEngineAudioSource(lowAccelClip);
            m_LowDecel = SetUpEngineAudioSource(lowDecelClip);
            m_HighDecel = SetUpEngineAudioSource(highDecelClip);

            // flag that we have started the sounds playing
            m_StartedSound = true;
        }

        private void StopSound()
        {
            //Destroy all audio sources on this object:
            foreach (var source in GetComponents<AudioSource>())
            {
                Destroy(source);
            }

            m_StartedSound = false;
        }

        // sets up and adds new audio source to the game object
        private AudioSource SetUpEngineAudioSource(AudioClip clip)
        {
            // create the new audio source component on the game object and set up its properties
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = 0;
            source.loop = true;

            // start the clip from a random point
            source.time = Random.Range(0f, clip.length);
            source.Play();
            source.minDistance = 5;
            source.maxDistance = maxRolloffDistance;
            source.dopplerLevel = 0;
            return source;
        }
    }
}