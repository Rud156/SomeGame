using System;
using System.Collections.Generic;
using Godot;
using SomeGame.Behaviors.Abilities.Base;
using SomeGame.Behaviors.HealthSystem;
using SomeGame.Camera;
using SomeGame.UI.Player;

namespace SomeGame.Player
{
    public partial class PlayerController : DamageHitStopPropagator
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

        // Movement
        private bool _fallStartHasMovement;
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

            CustomInputController.Instance.OnJumpPressed += _HandleJumpPressed;
            _abilityProcessor.OnAbilityStarted += _HandleAbilityStarted;
            _abilityProcessor.OnAbilityEnded += _HandleAbilityEnded;

            _playerAnimationController.SetPlayerController(this);

            // Maybe move this to a GameManager or something...
            CameraDeadZoneFollower.Instance.SetTarget(this);
            PlayerInfoDisplay.Instance.SetPlayerInfo(_playerInfoDataDisplay.playerName, _playerInfoDataDisplay.playerIcon);
            PlayerHealthDisplay.Instance.RegisterHealthAndDamage(HealthAndDamage);
        }

        public override void _ExitTree()
        {
            CustomInputController.Instance.OnJumpPressed -= _HandleJumpPressed;
            _abilityProcessor.OnAbilityStarted -= _HandleAbilityStarted;
            _abilityProcessor.OnAbilityEnded -= _HandleAbilityEnded;
        }

        public override void _Process(double delta)
        {
            var deltaTime = (float)delta;

            // (hitStopBehavior != null && !hitStopBehavior.IsActive)
            if (HitStopBehavior is { IsActive: false })
            {
                _UpdateGroundedState();
                _HandleMovement(deltaTime);

                if (TopMovementState != PlayerMovementState.CustomMovement)
                {
                    _ProcessGravity(deltaTime);
                    _ApplyJump();
                }

                _ApplyMovement();
                if (TopMovementState != PlayerMovementState.CustomMovement)
                {
                    _UpdateMeshRotation();
                }
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
                _fallStartHasMovement = !CustomInputController.Instance.HasNoDirectionalInput();
                _PushMovementState(PlayerMovementState.Falling);
            }
        }

        private void _HandleMovement(float delta)
        {
            switch (TopMovementState)
            {
                case PlayerMovementState.Normal:
                    _UpdateNormalState();
                    break;

                case PlayerMovementState.Falling:
                    _UpdateFallingState(delta);
                    break;

                case PlayerMovementState.CustomMovement:
                    _UpdateCustomMovement();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void _UpdateNormalState()
        {
            _currentMovementSpeed = _maxGroundedSpeed;
            var playerInput = CustomInputController.Instance.MovementInput;
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
            if (!CustomInputController.Instance.HasNoDirectionalInput())
            {
                _currentMovementSpeed += _airAcceleration * delta;
            }

            _currentMovementSpeed = Mathf.Clamp(_currentMovementSpeed, 0, _maxAirSpeed);

            var lastPlayerInput = CustomInputController.Instance.MovementInput;
            if (!CustomInputController.Instance.HasNoDirectionalInput())
            {
                _fallStartHasMovement = true;
            }

            if (_fallStartHasMovement)
            {
                lastPlayerInput = CustomInputController.Instance.LastNonZeroMovementInput;
            }

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

        private void _ResetFallingStateData() => _currentJumpCount = 0;

        private void _UpdateCustomMovement()
        {
            var currentAbilityVelocity = Vector3.Zero;
            var hasMovementAbility = false;

            foreach (var abilityBase in _abilityProcessor.ActiveAbilities)
            {
                if (abilityBase is MovementAbilityBase movementAbilityBase)
                {
                    currentAbilityVelocity += movementAbilityBase.MovementData;
                    hasMovementAbility = true;
                }
            }

            _movementVelocity = currentAbilityVelocity;

            if (!hasMovementAbility)
            {
                _PopMovementState();
            }
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
            switch (CustomInputController.Instance.LastUsedInputDeviceType)
            {
                case InputDeviceType.Keyboard:
                    _UpdateMeshRotationKeyboardMouse();
                    break;

                case InputDeviceType.Gamepad:
                    _UpdateMeshRotationGamepad();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void _UpdateMeshRotationKeyboardMouse()
        {
            var mousePosition = CustomInputController.Instance.MousePosition;
            var playerPosition = GlobalPosition;
            var targetPosition = new Vector3(mousePosition.X, playerPosition.Y, mousePosition.Z);
            if (!playerPosition.IsEqualApprox(targetPosition))
            {
                LookAt(targetPosition, Vector3.Up);
            }
        }

        private void _UpdateMeshRotationGamepad()
        {
            var lookGamepadInput = CustomInputController.Instance.LastLookGamepadInput;
            var rotationAngle = Mathf.Atan2(lookGamepadInput.Y, lookGamepadInput.X);
            RotationDegrees = Vector3.Up * Mathf.RadToDeg(rotationAngle);
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