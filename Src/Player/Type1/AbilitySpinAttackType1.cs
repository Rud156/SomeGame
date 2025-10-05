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
        // Constants
        // ================================

        private const string AbilityActiveParam = "parameters/AbilityActive/blend_amount";
        private const string AbilitySelectorParam = "parameters/AbilitySelector/blend_amount";
        private const string SpinAttackStartParam = "parameters/Type1SpinAttack/conditions/spin_attack_start";
        private const string SpinAttackStopParam = "parameters/Type1SpinAttack/conditions/spin_attack_stop";

        // ================================
        // Export
        // ================================

        [Export] private float _spinAttackMaxDuration;
        [Export] private float _spinMovementSpeed;
        [Export] private PackedScene _weaponRotatingDamage;
        [Export] private PackedScene _endingAoeDamage;

        // Data
        private float _currentSpinTime;
        private TickDamageInRange _tickDamageInstance;

        // ================================
        // Ability Functions
        // ================================

        public override void Start()
        {
            base.Start();
            _currentSpinTime = _spinAttackMaxDuration;

            if (_tickDamageInstance == null)
            {
                var tickDamageInstance = (TickDamageInRange)_weaponRotatingDamage.Instantiate();
                AddChild(tickDamageInstance);
                _tickDamageInstance = tickDamageInstance;
            }

            _tickDamageInstance.Enable(abilityProcessor.Character.Position, [abilityProcessor.Character.GetRid()]);

            abilityProcessor.AnimationTree.Set(AbilityActiveParam, 1);
            abilityProcessor.AnimationTree.Set(AbilitySelectorParam, (int)AbilityDisplay.abilityType);
            abilityProcessor.AnimationTree.Set(SpinAttackStopParam, false);
            abilityProcessor.AnimationTree.Set(SpinAttackStartParam, true);
        }

        public override void End()
        {
            base.End();

            var burstDamage = (BurstDamageInRange)_endingAoeDamage.Instantiate();
            AddChild(burstDamage);
            burstDamage.ApplyDamage(abilityProcessor.Character.Position, [abilityProcessor.Character.GetRid()]);
            burstDamage.QueueFree();

            _tickDamageInstance.Disable();
            _tickDamageInstance.QueueFree();
            _tickDamageInstance = null;

            abilityProcessor.AnimationTree.Set(AbilityActiveParam, 0);
            abilityProcessor.AnimationTree.Set(SpinAttackStopParam, true);
            abilityProcessor.AnimationTree.Set(SpinAttackStartParam, false);
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            // Update the Rotating Damage Location based on the character position...
            _tickDamageInstance.UpdatePosition(abilityProcessor.Character.Position);

            if (IsAbilityTriggerPressed(AbilityDisplay.abilityType))
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
            else
            {
                markedForEnd = true;
            }

            _currentSpinTime -= delta;
            if (_currentSpinTime <= 0)
            {
                markedForEnd = true;
            }

            // When the character falls end the ability...
            if (!abilityProcessor.Character.IsOnFloor())
            {
                markedForEnd = true;
            }
        }
    }
}