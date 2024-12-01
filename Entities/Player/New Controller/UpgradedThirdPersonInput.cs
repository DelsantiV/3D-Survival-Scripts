using UnityEngine;
using GoTF.Config;

namespace GoTF.Content
{
    public class UpgradedThirdPersonInput : MonoBehaviour
    {
        #region Variables       

        [Header("Controller Input")]
        public string horizontalInput = "Horizontal";
        public string verticallInput = "Vertical";

        public KeyCode strafeInput;

        [Header("Camera Input")]
        public string rotateCameraXInput = "Mouse X";
        public string rotateCameraYInput = "Mouse Y";

        [HideInInspector] public UpgradedThirdPersonController cc;
        [HideInInspector] public vThirdPersonCameraUpgraded tpCamera;
        [HideInInspector] public Camera cameraMain;
        [HideInInspector] public bool cameraLocked = false;
        [HideInInspector] public bool canMove;
        [HideInInspector] public bool canAction;
        private PlayerManager player;
        private PlayerInputConfig playerInputConfig;


        #endregion

        protected virtual void Start()
        {
            InitilizeController();
            InitializeTpCamera();
            player = GetComponent<PlayerManager>();
            playerInputConfig = player.PlayerInputConfig;
        }

        protected virtual void FixedUpdate()
        {
            cc.UpdateMotor();               // updates the ThirdPersonMotor methods
            cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
            cc.ControlRotationType();       // handle the controller rotation type
        }

        protected virtual void Update()
        {
            InputHandle();                  // update the input methods
            cc.UpdateAnimator();            // updates the Animator Parameters
        }

        public virtual void OnAnimatorMove()
        {
            cc.ControlAnimatorRootMotion(); // handle root motion animations 
        }

        #region Basic Locomotion Inputs

        protected virtual void InitilizeController()
        {
            cc = GetComponent<UpgradedThirdPersonController>();

            if (cc != null)
                cc.Init();
        }

        protected virtual void InitializeTpCamera()
        {
            if (tpCamera == null)
            {
                tpCamera = FindFirstObjectByType<vThirdPersonCameraUpgraded>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }
        }

        protected virtual void InputHandle()
        {
            LocomotionInputs();  // Handles inputs related to locomotion
            ActionInputs();      // Handles inputs related to action
        }

        private KeyCode GetInputKey(Controls control)
        {
            return playerInputConfig.GetKeyCodeForControl(control);
        }

        public virtual void MoveInput()
        {
            if (canMove)
            {
                cc.input.x = Input.GetAxis(horizontalInput);
                cc.input.z = Input.GetAxis(verticallInput);
                cc.GetGridCellAfterMovement();
                Debug.Log("Current cell : " + cc.currentGridCell.index.ToString() + ", at position " + cc.currentGridCell.worldPostion.ToString());
            }
            else
            {
                cc.input.x = 0;
                cc.input.z = 0;
            }
        }

        protected virtual void CameraInput()
        {
            if (!cameraLocked)
            {
                if (!cameraMain)
                {
                    if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                    else
                    {
                        cameraMain = Camera.main;
                        cc.rotateTarget = cameraMain.transform;
                    }
                }

                if (cameraMain)
                {
                    cc.UpdateMoveDirection(cameraMain.transform);
                }

                if (tpCamera == null)
                    return;

                var Y = Input.GetAxis(rotateCameraYInput);
                var X = Input.GetAxis(rotateCameraXInput);

                tpCamera.RotateCamera(X, Y);
            }
        }

        protected virtual void StrafeInput()
        {
            if (Input.GetKeyDown(strafeInput))
                cc.Strafe();
        }

        protected virtual void SprintInput()
        {
            if (Input.GetKeyDown(GetInputKey(Controls.Sprint)))
                cc.Sprint(true);
            else if (Input.GetKeyUp(GetInputKey(Controls.Sprint)))
                cc.Sprint(false);
        }

        /// <summary>
        /// Conditions to trigger the Jump animation & behavior
        /// </summary>
        /// <returns></returns>
        protected virtual bool JumpConditions()
        {
            return cc.isGrounded && cc.GroundAngle() < cc.slopeLimit && !cc.isJumping && !cc.stopMove && canAction;
        }

        /// <summary>
        /// Input to trigger the Jump 
        /// </summary>
        protected virtual void JumpInput()
        {
            if (Input.GetKeyDown(GetInputKey(Controls.Jump)) && JumpConditions())
                cc.Jump();
        }

        protected virtual void ForageResourcesInput()
        {
            if (Input.GetKeyDown(GetInputKey(Controls.Collect)) && !cc.isJumping)
            {
                cc.Forage();
            }
        }

        protected virtual void OtherHandInput() 
        { 
            if (Input.GetKeyDown(GetInputKey(Controls.OtherHandAction)) && !cc.isJumping && cc.inputMagnitude < 0.01f && canAction)
            {
                cc.OtherHandActionStart();
            }

            if ((Input.GetKeyUp(GetInputKey(Controls.OtherHandAction)) || cc.inputMagnitude > 0.01f) && cc.isOtherHandAction && canAction)
            {
                cc.OtherHandActionStop();
            }
        }

        protected virtual void PrefHandInput()
        {
            if (Input.GetKeyDown(GetInputKey(Controls.PrefHandAction)) && !cc.isJumping && cc.inputMagnitude < 0.01f && canAction)
            {
                cc.PrefHandActionStart();
            }

            if ((Input.GetKeyUp(GetInputKey(Controls.PrefHandAction)) || cc.inputMagnitude > 0.01f) && cc.isPrefHandAction && canAction)
            {
                cc.PrefHandActionStop();
            }
        }

        protected virtual void WalkModeInput()
        {
            if (Input.GetKeyDown(GetInputKey(Controls.ChangeWalkMode)))
            {
                if (!player.PlayerStatus.CanSprint && cc.freeSpeed.walkByDefault)
                {
                    Debug.Log("Carriyng heavy weight ! Cannot sprint");
                    return;
                }
                cc.freeSpeed.walkByDefault = !cc.freeSpeed.walkByDefault;
            }
        }

        protected virtual void ChangeHandModeInput()
        {
            if (Input.GetKeyDown(GetInputKey(Controls.SwitchHandMode)))
            {
                player.SwitchHandMode();
            }
        }

        protected virtual void LocomotionInputs()
        {
            MoveInput();
            CameraInput();
            SprintInput();
            StrafeInput();
            WalkModeInput();
        }

        protected virtual void ActionInputs()
        {
            JumpInput();
            ForageResourcesInput();
            OtherHandInput();
            PrefHandInput();
            ChangeHandModeInput();
        }

        public virtual void TryStopAllActions()
        {
            if (cc.isPrefHandAction) { cc.PrefHandActionStop(); }
            if (cc.isOtherHandAction) { cc.OtherHandActionStop(); }
            cc.StopMoving();
        }
        #endregion
    }
}