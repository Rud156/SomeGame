using SomeGame.Behaviors.Abilities.Base;
using SomeGame.Helpers;
using SomeGame.UI.Player;

namespace SomeGame.Player.Abilities
{
    public abstract partial class PlayerAbilityBase : AbilityBase
    {
        // ================================
        // Override Functions
        // ================================

        public override void _Ready()
        {
            PlayerAbilityDisplay.Instance.SetAbilityIcon(AbilityDisplay.abilityIcon, AbilityDisplay.abilityType);
            PlayerAbilityDisplay.Instance.SetAbilityBorderColor(ExtensionFunctions.AverageColorFromTexture(AbilityDisplay.abilityIcon), AbilityDisplay.abilityType);
        }
    }
}