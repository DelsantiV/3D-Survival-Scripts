using UnityEngine;
using GoTF.Config;
using UnityEngine.Events;

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
        [HideInInspector]
        public bool CanAction
        {
            get 
            {
                return player.isReadyForAction && cc.isGrounded && !cc.isSprinting && !cc.isHoldingBoth;
            }
        }
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

        protected KeyCode GetInputKey(Controls control)
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
            return cc.isGrounded && cc.GroundAngle() < cc.slopeLimit && !cc.stopMove && player.isReadyForAction;
        }

        /// <summary>
        /// Input to trigger the Jump 
        /// </summary>
        protected virtual void JumpInput()
        {
            if (Input.GetKeyDown(GetInputKey(Controls.Jump)) && JumpConditions())
                cc.Jump();
        }
        /// <summary>
        /// Input to trigger resource forage
        /// </summary>
        protected virtual void ForageResourcesInput()
        {
            if (Input.GetKeyDown(GetInputKey(Controls.Collect)))
            {
                cc.Forage();
            }
        }

        protected virtual void OtherHandInput() 
        { 
            if (Input.GetKey(GetInputKey(Controls.OtherHandAction)) && !cc.isAnyHandAction)
            {
                cc.HandleHandAction(player.otherHand);
            }
        }

        protected virtual void CheckStopOtherHandAction()
        {
            if (cc.isOtherHandAction)
            {
                if (!CanAction || Input.GetKeyUp(GetInputKey(Controls.OtherHandAction))) { cc.HandleHandAction(player.otherHand, false); }
            }
        }

        protected virtual void PrefHandInput()
        {
            if (Input.GetKey(GetInputKey(Controls.PrefHandAction)) && !cc.isAnyHandAction)
            {
                cc.HandleHandAction(player.prefHand);
            }
        }

        protected virtual void CheckStopPrefHandAction()
        {
            if (cc.isPrefHandAction)
            {
                if (!CanAction || Input.GetKeyUp(GetInputKey(Controls.PrefHandAction))) { cc.HandleHandAction(player.prefHand, false); }
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
                if (player.TrySwitchHandMode()) { cc.SwitchHandMode(); }
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
            Debug.Log("CanAction :" + CanAction);
            Debug.Log("IsAction : " + cc.isAnyHandAction);
            if (CanAction)
            {
                OtherHandInput();
                PrefHandInput();
                ForageResourcesInput();
            }
            CheckStopPrefHandAction();
            CheckStopOtherHandAction();
            ChangeHandModeInput();
        }

        public virtual void TryStopAllActions()
        {
            cc.StopActionsAndMovement();
        }
        #endregion
    }
}