using Godot;
using SomeGame.Behaviors.Abilities.Base;
using SomeGame.Helpers;
using SomeGame.UI.Player;

namespace SomeGame.Player.Abilities
{
    public abstract partial class PlayerMovementAbilityBase : MovementAbilityBase
    {
        // ================================
        // Override Functions
        // ================================

        public override void _Ready()
        {
            PlayerAbilityDisplay.Instance.SetAbilityIcon(AbilityDisplay.abilityIcon, AbilityDisplay.abilityType);
            PlayerAbilityDisplay.Instance.SetAbilityBorderColor(ExtensionFunctions.AverageColorFromTexture(AbilityDisplay.abilityIcon), AbilityDisplay.abilityType);
        }

        // ================================
        // Input Functions
        // ================================

        protected override bool IsAbilityTriggerPressed(AbilityType abilityType)
        {
            return CustomInputController.Instance.IsAbilityTriggerPressed((int)abilityType);
        }

        protected override Vector2 GetMovementInput()
        {
            return CustomInputController.Instance.MovementInput;
        }
    }
}