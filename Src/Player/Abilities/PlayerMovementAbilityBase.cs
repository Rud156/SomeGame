using Godot;
using SomeGame.Behaviors.Abilities.Base;

namespace SomeGame.Player.Abilities
{
    public abstract partial class PlayerMovementAbilityBase : MovementAbilityBase
    {
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