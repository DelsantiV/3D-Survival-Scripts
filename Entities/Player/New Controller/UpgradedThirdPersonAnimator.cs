
using System.Collections;
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

        protected AnimatorStateInfo AnimatorStateInfo(int layerindex)
        {
            return animator.GetCurrentAnimatorStateInfo(layerindex);
        }

        protected virtual bool CanExitLayer(int layerindex)
        {
            return AnimatorStateInfo(layerindex).IsName("Empty");
        }

        public virtual void UpdateAnimator()
        {
            if (animator == null || !animator.enabled) return;

            UpdateAnimatorLocomotion(); 
        }

        protected virtual void SetAnimatorMoveSpeed(vMovementSpeed speed)
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

        protected virtual void UpdateAnimatorLocomotion()
        {
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
            if (inputMagnitude > 0.4f) StopBodyActions();
        }

        public virtual void UpdateAnimatorActions()
        {
            UpdateHandsAnimationID();
            animator.SetBool(UpgradedAnimatorParameters.IsAction, isAnyHandAction);
            animator.SetBool(UpgradedAnimatorParameters.IsActionLeft, isOtherHandAction);
            animator.SetBool(UpgradedAnimatorParameters.IsActionRight, isPrefHandAction);
            animator.SetBool(UpgradedAnimatorParameters.IsHoldingBoth, isHoldingBoth);

            float upperBodyLayerWeight = (isAnyHandAction || isHoldingBoth) ? 1f : 0f;
            if (TrySetLayerWeight(upperBodyOverrideLayerIndex, upperBodyLayerWeight))
            {

            }
        }

        protected virtual IEnumerator WaitForCurrentActionToEnd()
        {
            yield return new WaitUntil(() => !animator.IsInTransition(upperBodyOverrideLayerIndex));
            Debug.Log("Waiting for animation to finish ...");
            yield return new WaitWhile(() => !CanExitLayer(upperBodyOverrideLayerIndex));
            Debug.Log("Animation finished !");
        }

        public virtual void StopBodyActions()
        {
            animator.SetLayerWeight(upperBodyOverrideLayerIndex, 0f);
        }

        protected virtual bool TrySetLayerWeight(int layerindex, float weight, bool forceInterrupt = false)
        {
            float currentWeight = animator.GetLayerWeight(layerindex);
            if (currentWeight == weight) return true;

            if (weight == 0 && !forceInterrupt)
            {
                if (!CanExitLayer(layerindex))
                {
                    Debug.Log("Cannot leave current animation state, as animation is still playing");
                    return false;
                }
            } 

            else
            {
                animator.SetLayerWeight(layerindex, weight);
                return true;
            }
            return false;
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