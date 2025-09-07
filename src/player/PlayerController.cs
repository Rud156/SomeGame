using System;
using System.Collections.Generic;
using Godot;
using SomeGame.Behaviors.Abilities.Base;
using SomeGame.Behaviors.HealthSystem;
using SomeGame.Behaviors.HitStop;
using SomeGame.Camera;
using SomeGame.UI.Player;

namespace SomeGame.Player
{
    public partial class PlayerController : CharacterBody3D
    {
        // ================================
        // Constants
        // ================================

        private const float Gravity = -9.8f;

        // ================================
        // Export
        // ================================

        [ExportGroup("Movement")]
        [Export] public float groundedAcceleration;
        [Export] public float groundedDeceleration;
        [Export] public float maxGroundedSpeed;
        [Export] public float airAcceleration;
        [Export] public float maxAirSpeed;
        [Export] public float jumpVelocity;
        [Export] public int maxJumpCount;
        [Export] public float gravityMultiplier;

        [ExportGroup("Display")]
        [Export] public PlayerInfoDataDisplay playerInfoDataDisplay;

        [ExportGroup("Components")]
        [Export] public AbilityProcessor abilityProcessor;
        [Export] public HealthAndDamage healthAndDamage;
        [Export] public HitStopBehavior hitStopBehavior;
        [Export] public PlayerAnimationController playerAnimationController;
        // ================================
        // Signals
        // ================================

        [Signal]
        public delegate void OnJumpTriggeredEventHandler();

        [Signal]
        public delegate void OnPlayerStateChangedEventHandler(PlayerMovementState movementState);

        // ================================
        // Movement Modifiers
        // ================================

        private Vector3 _movementVelocity;

        // ================================
        // State
        // ================================

        private List<PlayerMovementState> _movementStack;
        private bool _jumpPressed;
        private int _currentJumpCount;
        private float _currentMovementSpeed;

        // ================================
        // Override Functions
        // ================================

        public override void _Ready()
        {
            _movementStack = [];
            _movementVelocity = Vector3.Zero;

            _PushMovementState(PlayerMovementState.Normal);
            _ResetFallingStateData();

            PlayerInputController.Instance.OnJumpPressed += _HandleJumpPressed;
            abilityProcessor.OnAbilityStarted += _HandleAbilityStarted;
            abilityProcessor.OnAbilityEnded += _HandleAbilityEnded;

            playerAnimationController.SetPlayerController(this);

            CameraController.Instance.SetTargetObject(this);
            PlayerInfoDisplay.Instance.SetPlayerInfo(playerInfoDataDisplay.playerName, playerInfoDataDisplay.playerIcon);
            PlayerHealthDisplay.Instance.RegisterHealthAndDamage(healthAndDamage);
        }

        public override void _ExitTree()
        {
            PlayerInputController.Instance.OnJumpPressed -= _HandleJumpPressed;
            abilityProcessor.OnAbilityStarted -= _HandleAbilityStarted;
            abilityProcessor.OnAbilityEnded -= _HandleAbilityEnded;
        }

        public override void _Process(double delta)
        {
            var deltaTime = (float)delta;

            // (hitStopBehavior != null && !hitStopBehavior.IsActive)
            if (hitStopBehavior is { IsActive: false })
            {
                _UpdateGroundedState();

                _HandleMovement(deltaTime);
                _ProcessGravity(deltaTime);

                _ApplyJump();

                _ApplyMovement();
                _UpdateMeshRotation();
            }
        }

        // ================================
        // Public Functions
        // ================================

        public PlayerMovementState TopMovementState => _movementStack[^1];

        // ================================
        // Private Functions
        // ================================

        private void _HandleAbilityStarted(AbilityBase ability)
        {
            var abilityDisplay = ability.abilityDisplay;
            if (abilityDisplay is { isMovementAbility: true })
            {
                if (TopMovementState != PlayerMovementState.CustomMovement)
                {
                    _PushMovementState(PlayerMovementState.CustomMovement);
                }
            }
        }

        private void _HandleAbilityEnded(AbilityBase ability)
        {
            // Nothing to do for now...
        }

        private void _UpdateGroundedState()
        {
            var movementState = TopMovementState;
            if (!IsOnFloor() && movementState != PlayerMovementState.Falling && movementState != PlayerMovementState.CustomMovement)
            {
                _PushMovementState(PlayerMovementState.Falling);
            }
        }

        private void _HandleMovement(float delta)
        {
            switch (TopMovementState)
            {
                case PlayerMovementState.Normal:
                    _UpdateNormalState(delta);
                    break;

                case PlayerMovementState.Falling:
                    _UpdateFallingState(delta);
                    break;

                case PlayerMovementState.CustomMovement:
                    _UpdateCustomMovement(delta);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void _UpdateNormalState(float delta)
        {
            if (PlayerInputController.Instance.HasNoDirectionalInput())
            {
                _currentMovementSpeed -= groundedDeceleration * delta;
            }
            else
            {
                _currentMovementSpeed += groundedAcceleration * delta;
            }

            _currentMovementSpeed = Mathf.Clamp(_currentMovementSpeed, 0, maxGroundedSpeed);

            var lastPlayerInput = PlayerInputController.Instance.LastNonZeroMovementInput;
            var forward = Vector3.Forward;
            var right = Vector3.Right;

            var mappedMovement = forward * lastPlayerInput.Y + right * lastPlayerInput.X;
            mappedMovement.Y = 0;
            mappedMovement = mappedMovement.Normalized() * _currentMovementSpeed;

            _movementVelocity.X = mappedMovement.X;
            _movementVelocity.Z = mappedMovement.Z;
        }

        private void _UpdateFallingState(float delta)
        {
            if (!PlayerInputController.Instance.HasNoDirectionalInput())
            {
                _currentMovementSpeed += airAcceleration * delta;
            }

            _currentMovementSpeed = Mathf.Clamp(_currentMovementSpeed, 0, maxAirSpeed);

            var lastPlayerInput = PlayerInputController.Instance.LastNonZeroMovementInput;
            var forward = Vector3.Forward;
            var right = Vector3.Right;

            var mappedMovement = forward * lastPlayerInput.Y + right * lastPlayerInput.X;
            mappedMovement.Y = 0;
            mappedMovement = mappedMovement.Normalized() * _currentMovementSpeed;

            _movementVelocity.X = Mathf.Clamp(_movementVelocity.X + mappedMovement.X, -maxAirSpeed, maxAirSpeed);
            _movementVelocity.Z = Mathf.Clamp(_movementVelocity.Z + mappedMovement.Z, -maxAirSpeed, maxAirSpeed);

            if (IsOnFloor())
            {
                _ResetFallingStateData();
                _PopMovementState();
            }
        }

        private void _ResetFallingStateData()
        {
            _currentJumpCount = 0;
        }

        private void _UpdateCustomMovement(float delta)
        {
        }

        private void _ProcessGravity(float delta)
        {
            if (!IsOnFloor())
                _movementVelocity.Y += Gravity * gravityMultiplier * delta;
            else
                _movementVelocity.Y = Gravity * gravityMultiplier * delta;
        }

        private void _ApplyJump()
        {
            if (_jumpPressed)
            {
                _movementVelocity.Y = jumpVelocity;
                _jumpPressed = false;
                EmitSignal(SignalName.OnJumpTriggered);
            }
        }

        private void _HandleJumpPressed()
        {
            if (_currentJumpCount < maxJumpCount)
            {
                _jumpPressed = true;
                _currentJumpCount += 1;
            }
        }

        private void _ApplyMovement()
        {
            Velocity = _movementVelocity;
            MoveAndSlide();
        }

        private void _UpdateMeshRotation()
        {
            var mousePosition = PlayerInputController.Instance.MousePosition;
            var playerPosition = GlobalPosition;
            var targetPosition = new Vector3(mousePosition.X, playerPosition.Y, mousePosition.Z);
            if (!playerPosition.IsEqualApprox(targetPosition))
            {
                LookAt(targetPosition, Vector3.Up);
            }
        }

        private void _PushMovementState(PlayerMovementState movementState)
        {
            _movementStack.Add(movementState);
            EmitSignal(SignalName.OnPlayerStateChanged, (int)movementState);
        }

        private void _PopMovementState()
        {
            _movementStack.RemoveAt(_movementStack.Count - 1);
            EmitSignal(SignalName.OnPlayerStateChanged, (int)TopMovementState);
        }
    }


    public enum PlayerMovementState
    {
        Normal,
        Falling,
        CustomMovement,
    }
}