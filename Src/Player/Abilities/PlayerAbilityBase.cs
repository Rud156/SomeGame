using System.Collections.Generic;
using SomeGame.Behaviors.Abilities.Base;
using SomeGame.Helpers;
using SomeGame.UI.Player;

namespace SomeGame.Player.Abilities
{
    public abstract partial class PlayerAbilityBase : AbilityBase
    {
        // ================================
        // Ability Functions
        // ================================

        public override void Start()
        {
            base.Start();
            PlayerAbilityDisplay.Instance.TriggerAbilityFx(AbilityDisplay.abilityType);
        }

        public override bool CanStart(IReadOnlyCollection<AbilityBase> activeAbilities)
        {
            var canStart = base.CanStart(activeAbilities);
            if (!IsAbilityTriggerPressed(AbilityDisplay.abilityType))
            {
                canStart = false;
            }

            return canStart;
        }

        // ================================
        // Override Functions
        // ================================

        public override void _Ready()
        {
            base._Ready();

            PlayerAbilityDisplay.Instance.SetAbilityIcon(AbilityDisplay.abilityIcon, AbilityDisplay.abilityType);
            PlayerAbilityDisplay.Instance.SetAbilityBorderColor(ExtensionFunctions.AverageColorFromTexture(AbilityDisplay.abilityIcon), AbilityDisplay.abilityType);
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            PlayerAbilityDisplay.Instance.SetAbilityProgress(
                currentCooldownDuration,
                currentCooldownDuration / AbilityDisplay.cooldownDuration,
                AbilityDisplay.abilityType
            );

            PlayerAbilityDisplay.Instance.SetAbilityStackCount(currentStackCount, AbilityDisplay.stackCount, AbilityDisplay.abilityType);
        }
    }
}