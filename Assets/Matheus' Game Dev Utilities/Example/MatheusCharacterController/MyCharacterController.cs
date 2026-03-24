// Made By Matheus Boscariol
// Licensed under GNU AGPL License
// matheusbosc.com

using System;
using com.matheusbosc.utilities;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

namespace com.matheusbosc.charactercontroller
{ 
    [RequireComponent(typeof(Rigidbody))]
    public class MyCharacterController : MonoBehaviour
    {
        #region Variables
        
        [Header("Features")]
        [Tooltip("Is sprinting feature enabled")]  public bool sprintingEnabled = true;
        [Tooltip("Is crouching feature enabled")]  public bool crouchingEnabled = true;
        [Tooltip("Is jumping feature enabled")]    public bool jumpingEnabled = true;
        
        
        [Header("Movement")]
        [Tooltip("Speed without multipliers")]     public float baseSpeed = 5;
        [Tooltip("Speed while sprinting")]         public float sprintSpeed = 7;
        [Tooltip("Speed while crouching")]         public float crouchSpeed = 3;


        [Header("Crouch")] 
        [Tooltip("Regular camera parent position")]     public Vector3 baseCameraPosition;
        [Tooltip("Crouching camera parent position")]   public Vector3 crouchCameraPosition;
        [Tooltip("Regular Capsule Collider center")]    public Vector3 baseColliderCenter;
        [Tooltip("Crouching Capsule Collider center")]  public Vector3 crouchColliderCenter;
        [Tooltip("Regular Capsule Collider height")]    public float baseColliderHeight;
        [Tooltip("Crouching Capsule Collider height")]  public float crouchColliderHeight;
        [Tooltip("Time to crouch and uncrouch")]        public float crouchTime = 1;
        
        
        [Header("Look")]
        [Tooltip("Sensitivity of camera rotation")]             public float lookSensitivity = 10;
        [Tooltip("Minimum camera rotation")][Range(-120, -10)]  public float minClamp = -80;
        [Tooltip("Maximum camera rotation")][Range(120, 10)]    public float maxClamp = 80;
        
        
        [Header("Jump")]
        [Tooltip("The force applied on the positions y axis")]                   public float jumpForce = 4.5f;
        [Tooltip("The layers where the player is allowed to jump")]              public LayerMask groundLayer;
        [Tooltip("How far is the player allowed to be off the ground to jump")]  public float groundCheckRadius = 0.2f;


        [Header("Camera")] 
        [Tooltip("Normal FOV (Standing, Walking)")] [Range(1, 179)]  public float baseFov = 60;
        [Tooltip("FOV while sprinting")] [Range(1, 179)]             public float sprintFov = 75;
        [Tooltip("FOV while crouching")] [Range(1, 179)]             public float crouchFov = 60;
        [Tooltip("How much time to change FOV")]                     public float fovChangeTime = 1;
        
        
        [Header("References")]
        [Tooltip("The parent game object which the camera is attached to (cannot be the base player game object)")]  public Transform cameraParent;
        [Tooltip("The bottom most point on the player")]                                                             public GameObject groundCheck;
        public GameObject menu;
        
        // --------- PRIVATE -------------------------
        
        // References
        private Rigidbody _rb;
        private CharacterControllerInput _input;
        private Camera _camera;
        private CapsuleCollider _characterCollider;
        
        // Movement
        private Vector2 _movement, _look;
        private Vector3 _rbVelocity = Vector3.zero;
        private float _speed;

        // Status
        private bool _isPaused = false;
        private bool _isSprinting = false, _isCrouching = false;

        private float _fovTimer = 0, _crouchTimer = 0;
        

        #endregion
        
        #region Methods
        
        #region Startup

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();

            _characterCollider = GetComponent<CapsuleCollider>();
            
            _camera = Camera.main;
            
            _input = new CharacterControllerInput();

            // Actions
            _input.Game.Jump.started += (e) => Jump();
            
            _input.Game.Menu.started += (e) => SetPauseState(!_isPaused);
            
            _input.Game.Sprint.started += (e) => ToggleSprint(true);
            _input.Game.Sprint.canceled += (e) => ToggleSprint(false);
            
            _input.Game.Crouch.started += (e) => ToggleCrouch(true);
            _input.Game.Crouch.canceled += (e) => ToggleCrouch(false);
            
        }

        private void Start()
        {
            SetPauseState(false);
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
        }

        #endregion
        
        private void Update()
        {
            if (_isPaused) return;
            
            #region Crouch Camera Position
            
            // Crouch Camera Position
            if (_isCrouching)
            {
                if (Vector3.Distance(cameraParent.transform.localPosition, crouchCameraPosition) < 0.1f)
                {
                    cameraParent.transform.localPosition = crouchCameraPosition;
                    _crouchTimer = 0;
                }
                else
                {
                    _crouchTimer += Time.deltaTime;
                    cameraParent.transform.localPosition = Vector3.Lerp(cameraParent.transform.localPosition, crouchCameraPosition, _crouchTimer / crouchTime);
                }
            } else
            {
                if (Vector3.Distance(cameraParent.transform.localPosition, baseCameraPosition) < 0.1f)
                {
                    cameraParent.transform.localPosition = baseCameraPosition;
                    _crouchTimer = 0;
                }
                else
                {
                    _crouchTimer += Time.deltaTime;
                    cameraParent.transform.localPosition = Vector3.Lerp(cameraParent.transform.localPosition, baseCameraPosition, _crouchTimer / crouchTime);
                }
            }
            
            #endregion

            #region FOV
            
            // FOV
            if (_isSprinting)
            {
                if (Mathf.Abs(_camera.fieldOfView - sprintFov) < 0.1f)
                {
                    _camera.fieldOfView = sprintFov;
                    _fovTimer = 0;
                }
                else
                {
                    _fovTimer += Time.deltaTime;
                    _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, sprintFov, _fovTimer /  fovChangeTime);
                }
            } else if (_isCrouching)
            {
                if (Mathf.Abs(_camera.fieldOfView - crouchFov) < 0.1f)
                {
                    _camera.fieldOfView = crouchFov;
                    _fovTimer = 0;
                }
                else
                {
                    _fovTimer += Time.deltaTime;
                    _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, crouchFov, _fovTimer /  fovChangeTime);
                }
            }
            else 
            {
                if (Mathf.Abs(_camera.fieldOfView - baseFov) < 0.1f)
                {
                    _camera.fieldOfView = baseFov;
                    _fovTimer = 0;
                }
                else
                {
                    _fovTimer += Time.deltaTime;
                    _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, baseFov, _fovTimer /  fovChangeTime);
                }
            }
            
            #endregion
            
            // Look
            _look = _input.Game.Look.ReadValue<Vector2>();
            
            cameraParent.Rotate(Vector3.left, _look.y * lookSensitivity * Time.deltaTime);
            transform.Rotate(Vector3.up, _look.x * lookSensitivity * Time.deltaTime);

            
            // Clamp Camera (x)
            float x = cameraParent.localEulerAngles.x;

            if (x > 180f)
                x -= 360f;

            x = Mathf.Clamp(x, minClamp, maxClamp);

            cameraParent.localEulerAngles = new Vector3(x, 0, 0);
            
        }

        public void MainMenu()
        {
            GameSceneManager.instance.LoadMainMenu();
        }

        private void FixedUpdate()
        {
            if (_isPaused) return;
            
            // Speed
            if (_isSprinting)
            {
                _speed = sprintSpeed;
            }
            else if (_isCrouching)
            {
                _speed = crouchSpeed;
            }
            else
            {
                _speed = baseSpeed;
            }
            
            // Move
            _movement = _input.Game.Move.ReadValue<Vector2>();
            
            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.Normalize();
            
            Vector3 right = transform.right;
            right.y = 0;
            right.Normalize();
            
            Vector3 moveDir = forward * _movement.y + right * _movement.x;

            Vector3 velocity = new Vector3(moveDir.x * _speed, _rb.linearVelocity.y,  moveDir.z * _speed);

            _rb.linearVelocity = velocity;
        }

        private void Jump()
        {
            if (!jumpingEnabled) return;
            
            // Is grounded?
            var results = new Collider[1];
            var size = Physics.OverlapSphereNonAlloc(groundCheck.transform.position, groundCheckRadius, results, groundLayer);
            
            if (size > 0)
            {
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }

        private void SetPauseState(bool paused)
        {
            if (paused)
            {
                // Unlock Mouse
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                
                // Disable Gravity
                _rb.useGravity = false;

                // Save velocity
                _rbVelocity = _rb.linearVelocity;
                _rb.linearVelocity = Vector3.zero;
                
                _isPaused = true;
                menu.SetActive(true);
            }
            else
            {
                // Lock mouse
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
                // Enable gravity
                _rb.useGravity = true;
                
                // Reset velocity to velocity before pause
                _rb.linearVelocity = _rbVelocity;
                
                _isPaused = false;
                menu.SetActive(false);
            }
        }

        private void ToggleCrouch(bool toggle)
        {
            if (!crouchingEnabled) return;
            
            if (toggle)
            {
                _isCrouching = true;
                ToggleSprint(false);

                _characterCollider.center = crouchColliderCenter;
                _characterCollider.height = crouchColliderHeight;
            }
            else
            {
                _isCrouching = false;
                
                _characterCollider.center = baseColliderCenter;
                _characterCollider.height = baseColliderHeight;
            }
        }
        
        private void ToggleSprint(bool toggle)
        {
            if (!sprintingEnabled) return;
            
            if (toggle)
            {
                _isSprinting = true;
                ToggleCrouch(false);
            }
            else
            {
                _isSprinting = false;
            }
        }

        #endregion
    }
}
