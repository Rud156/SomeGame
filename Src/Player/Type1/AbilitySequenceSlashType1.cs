using System.Collections.Generic;
using Godot;
using SomeGame.Behaviors.Abilities.Base;
using SomeGame.Player.Abilities;

namespace SomeGame.Player.Type1
{
    public partial class AbilitySequenceSlashType1 : PlayerMovementAbilityBase
    {
        // ================================
        // Constants
        // ================================

        private const string AbilitySelectorParam = "parameters/AbilitySelector/blend_amount";
        private const string ChopAnimParam = "parameters/Type1Slash/conditions/chop";
        private const string DualSliceAnimParam = "parameters/Type1Slash/conditions/dual_slice";
        private const string SliceAnimParam = "parameters/Type1Slash/conditions/slice";

        // ================================
        // Export
        // ================================

        [ExportGroup("Generic Data")]
        [Export] private float _attackRate;
        [Export] private float _resetStateDuration;

        [ExportGroup("Attack Data")]
        [Export] private float _chopDamage;
        [Export] private CollisionShape3D _chopDamageArea;
        [Export] private Node3D _dualSlicePrefab;
        [Export] private float _slashDamage;
        [Export] private CollisionShape3D _slashDamageArea;

        private SequenceState _sequenceState;
        private float _currentResetTime; // Once this hits 0, the sequence is reset to SequenceState.Chop

        // ================================
        // Override Functions
        // ================================

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (PlayerInputController.Instance.IsAbilityTriggerPressed((int)AbilityDisplay.abilityType))
            {
            }
        }

        public override bool CanStart(List<AbilityBase> activeAbilities)
        {
            return false;
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