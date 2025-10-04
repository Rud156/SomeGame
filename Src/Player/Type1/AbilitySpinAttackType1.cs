using System.Collections.Generic;
using Godot;
using SomeGame.Behaviors.Abilities.Base;
using SomeGame.Behaviors.HealthSystem;
using SomeGame.Player.Abilities;

namespace SomeGame.Player.Type1
{
    public partial class AbilitySpinAttackType1 : PlayerMovementAbilityBase
    {
        // ================================
        // Export
        // ================================

        [Export] private float _spinAttackMaxDuration;
        [Export] private float _spinRate;
        [Export] private float _spinMovementSpeed;
        [Export] private PackedScene _weaponRotatingDamage;
        [Export] private PackedScene _endingAoeDamage;

        // Data
        private float _currentSpinTime;
        private TickDamageInRange _tickDamageInstance;

        // ================================
        // Override Functions
        // ================================

        public override void Start()
        {
            base.Start();
            _currentSpinTime = _spinAttackMaxDuration;

            if (_tickDamageInstance == null)
            {
                var tickDamageInstance = (TickDamageInRange)_weaponRotatingDamage.Instantiate();
                _tickDamageInstance = tickDamageInstance;
            }

            _tickDamageInstance.Enable(abilityProcessor.Character.Position, [abilityProcessor.Character]);
        }

        public override void End()
        {
            base.End();

            var burstDamage = (BurstDamageInRange)_endingAoeDamage.Instantiate();
            burstDamage.ApplyDamage(abilityProcessor.Character.Position, [abilityProcessor.Character]);

            _tickDamageInstance.Disable();
            _tickDamageInstance.QueueFree();
            _tickDamageInstance = null;
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            // Update the Rotating Damage Location based on the character position...
            _tickDamageInstance.UpdatePosition(abilityProcessor.Character.Position);

            if (IsAbilityTriggerPressed(AbilityDisplay.abilityType))
            {
                {
                    // Handle Spin...
                    var currentRotation = abilityProcessor.CharacterMesh.RotationDegrees;
                    currentRotation.Y += _spinRate * delta;
                    abilityProcessor.CharacterMesh.RotationDegrees = currentRotation;
                }

                {
                    // Handle Movement...
                    var movementInput = GetMovementInput();
                    var forward = Vector3.Forward;
                    var right = Vector3.Right;

                    var mappedMovement = forward * movementInput.Y + right * movementInput.X;
                    mappedMovement.Y = 0;
                    mappedMovement = mappedMovement.Normalized() * _spinMovementSpeed;

                    MovementData = mappedMovement;
                }
            }
            else
            {
                markedForEnd = true;
            }

            _currentSpinTime -= delta;
            if (_currentSpinTime <= 0)
            {
                markedForEnd = true;
            }
        }

        public override bool CanStart(List<AbilityBase> activeAbilities)
        {
            var canStart = base.CanStart(activeAbilities);
            if (!IsAbilityTriggerPressed(AbilityDisplay.abilityType))
            {
                canStart = false;
            }

            return canStart;
        }
    }
}