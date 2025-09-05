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
            // playerController.Jumped += _HandleOnJumpTriggered;
            // playerController.PlayerStateChanged += _HandleOnPlayerStateChanged;
        }

        public override void _ExitTree()
        {
            // playerController.Jumped -= _HandleOnJumpTriggered;
            // playerController.PlayerStateChanged -= _HandleOnPlayerStateChanged;
        }

        // This method is called every frame.
        public override void _Process(double delta)
        {
            // Handle grounded motion based on the current movement state.
            if (playerController == null) return;

            switch (playerController.TopMovementState)
            {
                case PlayerMovementState.Normal:
                    SetGroundedAnimation();
                    break;

                case PlayerMovementState.CustomMovement:
                    break;

                case PlayerMovementState.Falling:
                    break;
            }
        }

        // ================================
        // Private Functions
        // ================================

        // Calculates and sets the blend position for grounded movement animations.
        private void SetGroundedAnimation()
        {
            // Ensure controllers are valid before proceeding.
            if (PlayerInputController.Instance == null || animationTree == null) return;

            // Get movement input from the input controller.
            var movementInput = PlayerInputController.Instance.MovementInput;
            float moveZ = -movementInput.Y;
            float moveX = movementInput.X;

            // Get the player's global rotation on the Y-axis.
            float yRotation = playerController.GlobalRotation.Y;

            // Calculate vertical and horizontal movement relative to the character's facing direction.
            float vAxisVMovement = moveZ * Mathf.Cos(yRotation);
            float vAxisHMovement = moveZ * Mathf.Sin(yRotation);

            float hAxisVMovement = moveX * Mathf.Sin(yRotation);
            float hAxisHMovement = moveX * Mathf.Cos(yRotation);

            float vMovement = vAxisVMovement + hAxisVMovement;
            float hMovement = vAxisHMovement + hAxisHMovement;

            // Set the blend position on the animation tree.
            animationTree.Set(GroundedAnimParam, new Vector2(hMovement, vMovement));
        }

        // Event handler for when a jump is triggered.
        private void _HandleOnJumpTriggered()
        {
            if (animationTree == null) return;

            animationTree.Set(JumpTriggerMain, 1);
            animationTree.Set(JumpTriggerSecondary, true);
            animationTree.Set(JumpIsGrounded, false);
        }

        // Event handler for when the player's movement state changes.
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

        // Resets jump-related animation parameters.
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