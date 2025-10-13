using System;
using Godot;
using SomeGame.Behaviors.HealthSystem;
using SomeGame.Player.Abilities;

namespace SomeGame.Player.Type1
{
    public partial class AbilitySequenceSlashType1 : PlayerMovementAbilityBase
    {
        // ================================
        // Constants
        // ================================

        private const string AbilityActiveParam = "parameters/AbilityActive/blend_amount";
        private const string AbilitySelectorParam = "parameters/AbilitySelector/blend_amount";
        private const string ChopAnimParam = "parameters/Type1SequenceSlash/conditions/chop";
        private const string DualSliceAnimParam = "parameters/Type1SequenceSlash/conditions/dual_slice";
        private const string SliceAnimParam = "parameters/Type1SequenceSlash/conditions/slice";

        // ================================
        // Export
        // ================================

        [ExportGroup("Generic Data")]
        [Export] private float _resetStateDuration;
        [Export] private float _animationResetDuration;

        [ExportGroup("Attack Data")]
        [Export] private float _chopDuration;
        [Export] private float _chopDamageTriggerTime;
        [Export] private PackedScene _chopDamage;
        [Export] private float _dualSliceDuration;
        [Export] private float _dualSliceDamageTriggerTime;
        [Export] private PackedScene _dualSliceDamage;
        [Export] private float _sliceDuration;
        [Export] private float _sliceDamageTriggerTime;
        [Export] private PackedScene _sliceDamage;

        // Sequence Data
        private SequenceState _sequenceState;

        // Attack Data
        private float _currentResetTime; // Once this hits 0, the sequence is reset to SequenceState.Chop
        private float _currentAttackDuration;
        private float _currentDamageTriggerTime;
        private BurstDamageController _currentDamageController;

        // ================================
        // Ability Functions
        // ================================

        public override void Start()
        {
            base.Start();

            _currentAttackDuration = 0;

            abilityProcessor.AnimationTree.Set(AbilityActiveParam, 1);
            abilityProcessor.AnimationTree.Set(AbilitySelectorParam, (int)AbilityDisplay.abilityType);

            _ValidateAndUpdateNextState();
        }

        public override void End()
        {
            base.End();

            _currentResetTime = _resetStateDuration;
            abilityProcessor.AnimationTree.Set(AbilityActiveParam, 0);

            _ResetAnimations();
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            _currentAttackDuration -= delta;
            if (_currentAttackDuration <= 0)
            {
                _ValidateAndUpdateNextState();
            }

            if (_currentDamageTriggerTime > 0)
            {
                _currentDamageTriggerTime -= delta;
                if (_currentDamageTriggerTime <= 0)
                {
                    AddChild(_currentDamageController);
                    _currentDamageController.ApplyDamage([abilityProcessor.Character.GetRid()]);
                    _currentDamageController.QueueFree();
                }
            }
        }

        // ================================
        // Override Functions
        // ================================

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (AbilityActive || _currentResetTime <= 0)
            {
                return;
            }

            var deltaTime = (float)delta;

            _currentResetTime -= deltaTime;
            if (_currentResetTime <= 0)
            {
                _sequenceState = SequenceState.Chop;
            }
        }

        // ================================
        // Private Functions
        // ================================

        private void _ValidateAndUpdateNextState()
        {
            if (!IsAbilityTriggerPressed(AbilityDisplay.abilityType))
            {
                markedForEnd = true;
                return;
            }

            _ResetAnimations();

            // Set the duration for the animation
            _currentAttackDuration = _sequenceState switch
            {
                SequenceState.Chop => _chopDuration,
                SequenceState.DualSlice => _dualSliceDuration,
                SequenceState.Slice => _sliceDuration,
                _ => _currentAttackDuration
            };

            // Set the duration for when to trigger the damage
            _currentDamageTriggerTime = _sequenceState switch
            {
                SequenceState.Chop => _chopDamageTriggerTime,
                SequenceState.DualSlice => _dualSliceDamageTriggerTime,
                SequenceState.Slice => _sliceDamageTriggerTime,
                _ => _currentDamageTriggerTime
            };

            // Play the animation
            switch (_sequenceState)
            {
                case SequenceState.Chop:
                    abilityProcessor.AnimationTree.Set(ChopAnimParam, true);
                    _currentDamageController = (BurstDamageController)_chopDamage.Instantiate();
                    break;

                case SequenceState.DualSlice:
                    abilityProcessor.AnimationTree.Set(DualSliceAnimParam, true);
                    _currentDamageController = (BurstDamageController)_dualSliceDamage.Instantiate();
                    break;

                case SequenceState.Slice:
                    abilityProcessor.AnimationTree.Set(SliceAnimParam, true);
                    _currentDamageController = (BurstDamageController)_sliceDamage.Instantiate();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Go to the next state
            _sequenceState = _sequenceState switch
            {
                SequenceState.Chop => SequenceState.DualSlice,
                SequenceState.DualSlice => SequenceState.Slice,
                SequenceState.Slice => SequenceState.Chop,
                _ => _sequenceState
            };
        }

        private void _ResetAnimations()
        {
            abilityProcessor.AnimationTree.Set(ChopAnimParam, false);
            abilityProcessor.AnimationTree.Set(DualSliceAnimParam, false);
            abilityProcessor.AnimationTree.Set(SliceAnimParam, false);
        }

        // ================================
        // Enums
        // ================================

        private enum SequenceState
        {
            Chop,
            DualSlice,
            Slice
        }
    }
}