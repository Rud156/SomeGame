using System;
using Godot;
using SomeGame.Behaviors.Abilities.Base;

namespace SomeGame.UI.Player
{
    public partial class PlayerAbilityDisplay : Control
    {
        private static PlayerAbilityDisplay _instance;
        public static PlayerAbilityDisplay Instance => _instance;

        // ================================
        // Export
        // ================================

        [ExportGroup("Components")] [Export] public PlayerAbilityTileDisplay primaryTile;
        [Export] public PlayerAbilityTileDisplay secondaryTile;

        // ================================
        // Override Functions
        // ================================

        public override void _EnterTree()
        {
            if (_instance != null)
            {
                QueueFree();
                return;
            }

            _instance = this;
        }

        // ================================
        // Public Functions
        // ================================

        public void SetAbilityNameAndIcon(string abilityName, Texture2D icon, AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.Ability1:
                    primaryTile.SetAbilityNameAndIcon(abilityName, icon);
                    break;

                case AbilityType.Ability2:
                    secondaryTile.SetAbilityNameAndIcon(abilityName, icon);
                    break;

                case AbilityType.ExternalAbility:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(abilityType), abilityType, null);
            }
        }

        public void SetAbilityProgress(float time, float progress, float minProgress, float maxProgress, AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.Ability1:
                    primaryTile.SetAbilityProgress(time, progress, minProgress, maxProgress);
                    break;

                case AbilityType.Ability2:
                    secondaryTile.SetAbilityProgress(time, progress, minProgress, maxProgress);
                    break;

                case AbilityType.ExternalAbility:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(abilityType), abilityType, null);
            }
        }

        public void TriggerAbilityFx(AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.Ability1:
                    primaryTile.TriggerAbilityFx();
                    break;

                case AbilityType.Ability2:
                    secondaryTile.TriggerAbilityFx();
                    break;

                case AbilityType.ExternalAbility:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(abilityType), abilityType, null);
            }
        }
    }
}