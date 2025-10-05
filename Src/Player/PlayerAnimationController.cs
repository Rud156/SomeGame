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
        private const string JumpTriggerMain = "parameters/MovementBlend/blend_amount";
        private const string JumpTriggerSecondary = "parameters/JumpSM/conditions/jump_triggered";
        private const string JumpIsFalling = "parameters/JumpSM/conditions/is_falling";
        private const string JumpIsGrounded = "parameters/JumpSM/conditions/is_grounded";

        // ================================
        // Export
        // ================================

        [Export] private AnimationTree _animationTree;

        // Data
        private PlayerController _playerController;

        // ================================
        // Override Functions
        // ================================

        public override void _Ready() => _animationTree.SetActive(true);

        public override void _ExitTree() => _ResetPlayerControllerSignals();

        public override void _Process(double delta)
        {
            switch (_playerController.TopMovementState)
            {
                case PlayerMovementState.Normal:
                    _SetGroundedAnimation();
                    break;

                case PlayerMovementState.CustomMovement:
                case PlayerMovementState.Falling:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // ================================
        // Public Functions
        // ================================

        public void SetPlayerController(PlayerController playerController)
        {
            _ResetPlayerControllerSignals();

            _playerController = playerController;
            _playerController.OnJumpTriggered += _HandleOnJumpTriggered;
            _playerController.OnPlayerStateChanged += _HandleOnPlayerStateChanged;
        }

        // ================================
        // Private Functions
        // ================================

        private void _SetGroundedAnimation()
        {
            var (moveX, moveZ) = CustomInputController.Instance.MovementInput;
            moveX = -moveX;
            var yRotation = _playerController.GlobalRotation.Y;

            var hMovement = moveX * Mathf.Cos(yRotation) - moveZ * Mathf.Sin(yRotation);
            var vMovement = moveX * Mathf.Sin(yRotation) + moveZ * Mathf.Cos(yRotation);

            _animationTree.Set(GroundedAnimParam, new Vector2(hMovement, vMovement));
        }

        private void _HandleOnJumpTriggered()
        {
            if (_animationTree == null) return;

            _animationTree.Set(JumpTriggerMain, 1);
            _animationTree.Set(JumpTriggerSecondary, true);
            _animationTree.Set(JumpIsGrounded, false);
        }

        private void _HandleOnPlayerStateChanged(PlayerMovementState currentState)
        {
            if (_animationTree == null) return;

            switch (currentState)
            {
                case PlayerMovementState.Normal:
                case PlayerMovementState.CustomMovement:
                    _ResetJumpAnimation();
                    break;

                case PlayerMovementState.Falling:
                    _animationTree.Set(JumpIsFalling, true);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null);
            }
        }

        private void _ResetJumpAnimation()
        {
            if (_animationTree == null) return;

            _animationTree.Set(JumpTriggerMain, 0);
            _animationTree.Set(JumpTriggerSecondary, false);
            _animationTree.Set(JumpIsFalling, false);
            _animationTree.Set(JumpIsGrounded, true);
        }

        private void _ResetPlayerControllerSignals()
        {
            if (_playerController != null)
            {
                _playerController.OnJumpTriggered -= _HandleOnJumpTriggered;
                _playerController.OnPlayerStateChanged -= _HandleOnPlayerStateChanged;
            }
        }
    }
}