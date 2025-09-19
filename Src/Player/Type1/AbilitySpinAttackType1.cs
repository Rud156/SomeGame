using System.Collections.Generic;
using Godot;
using SomeGame.Behaviors.Abilities.Base;
using SomeGame.Player.Abilities;

namespace SomeGame.Player.Type1
{
    public partial class AbilitySpinAttackType1 : PlayerMovementAbilityBase
    {
        // ================================
        // Export
        // ================================

        [Export] private float _spinAttackDuration;
        [Export] private float _spinAttackDamage;

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
    }
}