using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 4.0f;   // Speed when walking forward
            public float BackwardSpeed = 2.0f;  // Speed when walking backwards
            public float StrafeSpeed = 2.0f;    // Speed when walking sideways
            public float RunMultiplier = 2.0f;   // Speed when sprinting
            public KeyCode RunKey = KeyCode.LeftShift;
            public float JumpForce = 60f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;


#if !MOBILE_INPUT
            private bool m_Running;
#endif

            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
                if (input == Vector2.zero) return;
                if (input.x > 0 || input.x < 0)
                {
                    //strafe
                    CurrentTargetSpeed = StrafeSpeed;
                }
                if (input.y < 0)
                {
                    //backwards
                    CurrentTargetSpeed = BackwardSpeed;
                }
                if (input.y > 0)
                {
                    //forwards
                    //handled last as if strafing and moving forward at the same time forwards speed should take precedence
                    CurrentTargetSpeed = ForwardSpeed;
                }
#if !MOBILE_INPUT
                if (Input.GetKey(RunKey))
                {
                    CurrentTargetSpeed *= RunMultiplier;
                    m_Running = true;
                }
                else
                {
                    m_Running = false;
                }
#endif
            }

#if !MOBILE_INPUT
            public bool Running
            {
                get { return m_Running; }
            }
#endif
        }


        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }


        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public static MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();


        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;

        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
        private AudioSource m_AudioSource;
        private Vector2 previousStepPosition;                     // This is so we only play a step sound when we are of specified distance away
        public float distanceForStepNoise;
        public float equipDistance;                               // This is the distance needed to equip something
        private Equippable[] equippables;                         // This is all of the objects with the tag 'equippable'
        public Equippable[] equipped;                            // These are all of the equippable objects that we have equipped
        private DoorScript[] doors;                               // These are all of the doors in the game
        private int equippedIndex;                                // This is basically the number of items we have equipped
        private int score;                                        // This is the players score
        public Text scoreText;
        public Text errorText;
        public Text goodJobText;
        public int errorMsgFrameLimit;
        private int errorMsgFrameCount;
        public int goodJobFrameLimit;
        private int goodJobFrameCount;

        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public bool Jumping
        {
            get { return m_Jumping; }
        }

        public bool Running
        {
            get
            {
#if !MOBILE_INPUT
                return movementSettings.Running;
#else
	            return false;
#endif
            }
        }


        private void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            mouseLook.Init(transform, cam.transform);

            m_AudioSource = GetComponent<AudioSource>();

            previousStepPosition = new Vector2(transform.position.x, transform.position.z);

            equippables = FindObjectsOfType<Equippable>();

            equipped = new Equippable[equippables.Length];

            Debug.Log("Number of equippables is: " + equippables.Length);

            doors = FindObjectsOfType<DoorScript>();

            equippedIndex = 0;

            score = 0;

            errorMsgFrameCount = 0;

            errorText.text = "";

            goodJobFrameCount = 0;

            goodJobText.text = "";

        }


        private void Update()
        {
            RotateView();

            if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump)
            {
                m_Jump = true;
            }
        }


        private void FixedUpdate()
        {

            // We want to update the score constantly
            scoreText.text = "Score: " + score.ToString();

            // This checks if we right click the mouse
            if (Input.GetMouseButtonDown(1))
            {
                Equip();
            }


            if (errorText.text == "" || errorMsgFrameCount >= errorMsgFrameLimit)
            {
                errorMsgFrameCount = 0;
                errorText.text = "";
                errorText.enabled = false;
            }
            else
            {
                errorMsgFrameCount++;
            }

            if (goodJobText.text == "" || goodJobFrameCount >= goodJobFrameLimit)
            {
                goodJobFrameCount = 0;
                goodJobText.text = "";
                goodJobText.enabled = false;
            }
            else
            {
                goodJobFrameCount++;
            }


            GroundCheck();
            Vector2 input = GetInput();

            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || m_IsGrounded))
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

                desiredMove.x = desiredMove.x * movementSettings.CurrentTargetSpeed;
                desiredMove.z = desiredMove.z * movementSettings.CurrentTargetSpeed;
                desiredMove.y = desiredMove.y * movementSettings.CurrentTargetSpeed;
                if (m_RigidBody.velocity.sqrMagnitude <
                    (movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed))
                {
                    m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                    PlayFootStepAudio();
                }
            }

            if (m_IsGrounded)
            {
                m_RigidBody.drag = 5f;

                if (m_Jump)
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;

                    PlayJumpSound();
                }

                if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
                {
                    m_RigidBody.Sleep();
                }

                // We will jump if we are grounded not and we not previously
                if (!m_PreviouslyGrounded)
                {
                    PlayLandingSound();
                }
            }
            else
            {
                m_RigidBody.drag = 0f;
                if (m_PreviouslyGrounded && !m_Jumping)
                {
                    StickToGroundHelper();
                }
            }
            m_Jump = false;

        }

        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            Debug.Log("Landing sound");
        }

        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
            Debug.Log("Jump sound");
        }

        private void PlayFootStepAudio()
        {
            // Creating this variable and adding it to our if statement condition allows us to only make a noise when we have gone a certain distance from the last noise
            Vector2 currLocation = new Vector2(transform.position.x, transform.position.z);
            float distanceFromLastFootstep = Vector2.Distance(previousStepPosition, currLocation);
            if (!m_IsGrounded || distanceFromLastFootstep < distanceForStepNoise)
            {
                // Do nothing
            }
            else
            {
                // pick & play a random footstep sound from the array,
                // excluding sound at index 0
                int n = UnityEngine.Random.Range(1, m_FootstepSounds.Length);
                m_AudioSource.clip = m_FootstepSounds[n];
                m_AudioSource.PlayOneShot(m_AudioSource.clip);
                // move picked sound to index 0 so it's not picked next time
                m_FootstepSounds[n] = m_FootstepSounds[0];
                m_FootstepSounds[0] = m_AudioSource.clip;
                previousStepPosition = currLocation;
                Debug.Log("Footstep");
            }
        }

        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) +
                                   advancedSettings.stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }


        private Vector2 GetInput()
        {

            Vector2 input = new Vector2
            {
                x = CrossPlatformInputManager.GetAxis("Horizontal"),
                y = CrossPlatformInputManager.GetAxis("Vertical")
            };
            movementSettings.UpdateDesiredTargetSpeed(input);
            return input;
        }


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation(transform, cam.transform);

            if (m_IsGrounded || advancedSettings.airControl)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation * m_RigidBody.velocity;
            }
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
            }
        }


        /**
         * This is a method to try and equip a nearby item.
         */
        private void Equip()
        {

            // We will use this variable so that we can only equip one item per click
            bool done = false;

            for (int i = 0; i < equippables.Length && done == false; i++)
            {
                Equippable current = equippables[i];

                if (current.getDistanceToPlayer() < current.distanceNeeded && !current.equipped)
                {
                    // This if statement is so that everything dissapears when we select it, besides the flashlight
                    if (current.name == "Flashlight")
                    {
                        Transform camera = gameObject.transform.GetChild(0);
                        current.transform.parent = camera;
                        current.transform.position = camera.position;
                        current.transform.rotation = camera.rotation;
                    }
                    else
                    {
                        current.gameObject.SetActive(false);
                    }
                    equipped[equippedIndex++] = current;
                    current.equipped = true;
                    score += current.points;
                    string text = current.message;

                    // JUst check if they player didn't win
                    // Weird to say how many points the item gave them if they won
                    if (current.name != "FinalCrystal")
                    {
                        text += " This got you " + current.points + " points!";
                    }
                    goodJobText.text = text;
                    goodJobText.enabled = true;
                    done = true;
                    Debug.Log("Just equipped " + current.name);
                }

            }
        }

    }
}
