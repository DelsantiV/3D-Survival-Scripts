
using UnityEngine;

namespace GoTF.Content
{
    public class UpgradedThirdPersonAnimator : UpgradedThirdPersonMotor
    {
        #region Variables                

        public const float walkSpeed = 0.5f;
        public const float runningSpeed = 1f;
        public const float sprintSpeed = 1.5f;

        #endregion  

        public virtual void UpdateAnimator()
        {
            if (animator == null || !animator.enabled) return;

            animator.SetBool(UpgradedAnimatorParameters.IsStrafing, isStrafing); ;
            animator.SetBool(UpgradedAnimatorParameters.IsSprinting, isSprinting);
            animator.SetBool(UpgradedAnimatorParameters.IsGrounded, isGrounded);
            animator.SetFloat(UpgradedAnimatorParameters.GroundDistance, groundDistance);

            if (isStrafing)
            {
                animator.SetFloat(UpgradedAnimatorParameters.InputHorizontal, stopMove ? 0 : horizontalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
                animator.SetFloat(UpgradedAnimatorParameters.InputVertical, stopMove ? 0 : verticalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
            }
            else
            {
                animator.SetFloat(UpgradedAnimatorParameters.InputVertical, stopMove ? 0 : verticalSpeed, freeSpeed.animationSmooth, Time.deltaTime);
            }

            animator.SetFloat(UpgradedAnimatorParameters.InputMagnitude, stopMove ? 0f : inputMagnitude, isStrafing ? strafeSpeed.animationSmooth : freeSpeed.animationSmooth, Time.deltaTime);
        }

        public virtual void SetAnimatorMoveSpeed(vMovementSpeed speed)
        {
            Vector3 relativeInput = transform.InverseTransformDirection(moveDirection);
            verticalSpeed = relativeInput.z;
            horizontalSpeed = relativeInput.x;

            var newInput = new Vector2(verticalSpeed, horizontalSpeed);

            if (speed.walkByDefault)
                inputMagnitude = Mathf.Clamp(newInput.magnitude, 0, isSprinting ? runningSpeed : walkSpeed);
            else
                inputMagnitude = Mathf.Clamp(isSprinting ? newInput.magnitude + 0.5f : newInput.magnitude, 0, isSprinting ? sprintSpeed : runningSpeed);
        }

        public virtual void UpdateAnimatorActions()
        {
            UpdateHandsAnimationID();
            animator.SetBool(UpgradedAnimatorParameters.IsAction, isAnyHandAction);
            animator.SetBool(UpgradedAnimatorParameters.IsActionLeft, isOtherHandAction);
            animator.SetBool(UpgradedAnimatorParameters.IsActionRight, isPrefHandAction);
            animator.SetBool(UpgradedAnimatorParameters.IsHoldingBoth, isHoldingBoth);

            animator.SetLayerWeight(upperBodyOverrideLayerIndex, (isAnyHandAction || isHoldingBoth) ? 1f : 0f);
        }

        public virtual void UpdateHandsAnimationID()
        {
            Debug.Log("Animation ID : " + playerManager.GetItemAnimationID(HandsManager.Hand.right));
            animator.SetInteger(UpgradedAnimatorParameters.ItemActionID, playerManager.GetItemAnimationID(HandsManager.Hand.right));
        }

        public virtual void Die()
        {
            animator.SetTrigger(UpgradedAnimatorParameters.DeathTrigger);
        }
    }

    public static partial class UpgradedAnimatorParameters
    {
        public static int InputHorizontal = Animator.StringToHash("InputHorizontal");
        public static int InputVertical = Animator.StringToHash("InputVertical");
        public static int InputMagnitude = Animator.StringToHash("InputMagnitude");
        public static int IsGrounded = Animator.StringToHash("IsGrounded");
        public static int IsStrafing = Animator.StringToHash("IsStrafing");
        public static int IsSprinting = Animator.StringToHash("IsSprinting");
        public static int GroundDistance = Animator.StringToHash("GroundDistance");
        public static int IsHoldingBoth = Animator.StringToHash("isHoldingBoth");
        public static int IsActionLeft = Animator.StringToHash("isActionLeft");
        public static int IsActionRight = Animator.StringToHash("isActionRight");
        public static int IsAction = Animator.StringToHash("isAction");
        public static int ItemActionID = Animator.StringToHash("ItemActionID");
        public static int DeathTrigger = Animator.StringToHash("Death");
    }
}