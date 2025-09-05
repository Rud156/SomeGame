using System;
using Godot;

namespace SomeGame.Player
{
    public partial class PlayerAnimationController : Node3D
    {
        // ================================
        // Constants
        // ================================
        private const string GroundedAnimParam = "parameters/GroundedBSP/blend_position";
        private const string JumpTriggerMain = "parameters/Lowerbody/blend_amount";
        private const string JumpTriggerSecondary = "parameters/Lowerbody/blend_amount"; // This appears to be a duplicate constant, kept for direct conversion.
        private const string JumpIsFalling = "parameters/JumpSM/conditions/is_falling";
        private const string JumpIsGrounded = "parameters/JumpSM/conditions/is_grounded";

        // ================================
        // Export
        // ================================
        [Export] public PlayerController playerController;
        [Export] public AnimationTree animationTree;

        // ================================
        // Override Functions
        // ================================

        public override void _Ready()
        {
            playerController.OnJumpTriggered += _HandleOnJumpTriggered;
            playerController.OnPlayerStateChanged += _HandleOnPlayerStateChanged;
        }

        public override void _ExitTree()
        {
            playerController.OnJumpTriggered -= _HandleOnJumpTriggered;
            playerController.OnPlayerStateChanged -= _HandleOnPlayerStateChanged;
        }

        public override void _Process(double delta)
        {
            switch (playerController.TopMovementState)
            {
                case PlayerMovementState.Normal:
                    SetGroundedAnimation();
                    break;

                case PlayerMovementState.CustomMovement:
                case PlayerMovementState.Falling:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // ================================
        // Private Functions
        // ================================

        private void SetGroundedAnimation()
        {
            var (moveX, moveZ) = PlayerInputController.Instance.MovementInput;
            var yRotation = playerController.GlobalRotation.Y;

            var vAxisVMovement = moveZ * Mathf.Cos(yRotation);
            var vAxisHMovement = moveZ * Mathf.Sin(yRotation);

            var hAxisVMovement = moveX * Mathf.Sin(yRotation);
            var hAxisHMovement = moveX * Mathf.Cos(yRotation);

            var vMovement = vAxisVMovement + hAxisVMovement;
            var hMovement = vAxisHMovement + hAxisHMovement;

            animationTree.Set(GroundedAnimParam, new Vector2(hMovement, vMovement));
        }

        private void _HandleOnJumpTriggered()
        {
            if (animationTree == null) return;

            animationTree.Set(JumpTriggerMain, 1);
            animationTree.Set(JumpTriggerSecondary, true);
            animationTree.Set(JumpIsGrounded, false);
        }

        private void _HandleOnPlayerStateChanged(PlayerMovementState currentState)
        {
            if (animationTree == null) return;

            switch (currentState)
            {
                case PlayerMovementState.Normal:
                case PlayerMovementState.CustomMovement:
                    ResetJumpAnimation();
                    break;

                case PlayerMovementState.Falling:
                    animationTree.Set(JumpIsFalling, true);
                    break;
            }
        }

        private void ResetJumpAnimation()
        {
            if (animationTree == null) return;

            animationTree.Set(JumpTriggerMain, 0);
            animationTree.Set(JumpTriggerSecondary, false);
            animationTree.Set(JumpIsFalling, false);
            animationTree.Set(JumpIsGrounded, true);
        }
    }
}