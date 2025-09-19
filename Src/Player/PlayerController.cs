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
        [Export] private float _maxGroundedSpeed;
        [Export] private float _airAcceleration;
        [Export] private float _maxAirSpeed;
        [Export] private float _jumpVelocity;
        [Export] private int _maxJumpCount;
        [Export] private float _gravityMultiplier;

        [ExportGroup("Display")]
        [Export] private PlayerInfoDataDisplay _playerInfoDataDisplay;

        [ExportGroup("Components")]
        [Export] private AbilityProcessor _abilityProcessor;
        [Export] private HealthAndDamage _healthAndDamage;
        [Export] private HitStopBehavior _hitStopBehavior;
        [Export] private PlayerAnimationController _playerAnimationController;

        // ================================
        // Signals
        // ================================

        [Signal]
        public delegate void OnJumpTriggeredEventHandler();

        [Signal]
        public delegate void OnPlayerStateChangedEventHandler(PlayerMovementState movementState);


        // Movement Modifiers
        private Vector3 _movementVelocity;

        // Player State
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
            _abilityProcessor.OnAbilityStarted += _HandleAbilityStarted;
            _abilityProcessor.OnAbilityEnded += _HandleAbilityEnded;

            _playerAnimationController.SetPlayerController(this);

            CameraController.Instance.SetTargetObject(this);
            PlayerInfoDisplay.Instance.SetPlayerInfo(_playerInfoDataDisplay.playerName, _playerInfoDataDisplay.playerIcon);
            PlayerHealthDisplay.Instance.RegisterHealthAndDamage(_healthAndDamage);
        }

        public override void _ExitTree()
        {
            PlayerInputController.Instance.OnJumpPressed -= _HandleJumpPressed;
            _abilityProcessor.OnAbilityStarted -= _HandleAbilityStarted;
            _abilityProcessor.OnAbilityEnded -= _HandleAbilityEnded;
        }

        public override void _Process(double delta)
        {
            var deltaTime = (float)delta;

            // (hitStopBehavior != null && !hitStopBehavior.IsActive)
            if (_hitStopBehavior is { IsActive: false })
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
            var abilityDisplay = ability.AbilityDisplay;
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
            _currentMovementSpeed = _maxGroundedSpeed;
            var playerInput = PlayerInputController.Instance.MovementInput;
            var forward = Vector3.Forward;
            var right = Vector3.Right;

            var mappedMovement = forward * playerInput.Y + right * playerInput.X;
            mappedMovement.Y = 0;
            mappedMovement = mappedMovement.Normalized() * _currentMovementSpeed;

            _movementVelocity.X = mappedMovement.X;
            _movementVelocity.Z = mappedMovement.Z;
        }

        private void _UpdateFallingState(float delta)
        {
            if (!PlayerInputController.Instance.HasNoDirectionalInput())
            {
                _currentMovementSpeed += _airAcceleration * delta;
            }

            _currentMovementSpeed = Mathf.Clamp(_currentMovementSpeed, 0, _maxAirSpeed);

            var lastPlayerInput = PlayerInputController.Instance.LastNonZeroMovementInput;
            var forward = Vector3.Forward;
            var right = Vector3.Right;

            var mappedMovement = forward * lastPlayerInput.Y + right * lastPlayerInput.X;
            mappedMovement.Y = 0;
            mappedMovement = mappedMovement.Normalized() * _currentMovementSpeed;

            _movementVelocity.X = Mathf.Clamp(_movementVelocity.X + mappedMovement.X, -_maxAirSpeed, _maxAirSpeed);
            _movementVelocity.Z = Mathf.Clamp(_movementVelocity.Z + mappedMovement.Z, -_maxAirSpeed, _maxAirSpeed);

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
                _movementVelocity.Y += Gravity * _gravityMultiplier * delta;
            else
                _movementVelocity.Y = Gravity * _gravityMultiplier * delta;
        }

        private void _ApplyJump()
        {
            if (_jumpPressed)
            {
                _movementVelocity.Y = _jumpVelocity;
                _jumpPressed = false;
                EmitSignal(SignalName.OnJumpTriggered);
            }
        }

        private void _HandleJumpPressed()
        {
            if (_currentJumpCount < _maxJumpCount)
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