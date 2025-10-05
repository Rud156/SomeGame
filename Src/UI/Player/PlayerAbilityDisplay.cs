using System;
using System.Threading.Tasks;
using Godot;
using SomeGame.Behaviors.Abilities.Base;
using SomeGame.Player;

namespace SomeGame.UI.Player
{
    public partial class PlayerAbilityDisplay : Control
    {
        private static PlayerAbilityDisplay _instance;
        public static PlayerAbilityDisplay Instance => _instance;

        // ================================
        // Export
        // ================================

        [Export] private PlayerAbilityTileDisplay _primaryTile;
        [Export] private Texture2D _primaryKeyboardIcon;
        [Export] private Texture2D _primaryGamepadIcon;
        [Export] private PlayerAbilityTileDisplay _secondaryTile;
        [Export] private Texture2D _secondaryKeyboardIcon;
        [Export] private Texture2D _secondaryGamepadIcon;

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

        public override void _Ready() => CustomInputController.Instance.OnInputTypeUpdated += _HandleInputTypeUpdated;

        public override void _ExitTree() => CustomInputController.Instance.OnInputTypeUpdated -= _HandleInputTypeUpdated;

        // ================================
        // Public Functions
        // ================================

        public void SetAbilityBorderColor(Color color, AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.Ability1:
                    _primaryTile.SetAbilityBorderColor(color);
                    break;

                case AbilityType.Ability2:
                    _secondaryTile.SetAbilityBorderColor(color);
                    break;

                case AbilityType.ExternalAbility:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(abilityType), abilityType, null);
            }
        }

        public void SetAbilityIcon(Texture2D icon, AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.Ability1:
                    _primaryTile.SetAbilityIcon(icon);
                    break;

                case AbilityType.Ability2:
                    _secondaryTile.SetAbilityIcon(icon);
                    break;

                case AbilityType.ExternalAbility:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(abilityType), abilityType, null);
            }
        }

        public void SetAbilityStackCount(int count, int maxCount, AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.Ability1:
                    _primaryTile.SetAbilityStack(count, maxCount);
                    break;

                case AbilityType.Ability2:
                    _secondaryTile.SetAbilityStack(count, maxCount);
                    break;

                case AbilityType.ExternalAbility:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(abilityType), abilityType, null);
            }
        }

        public void SetAbilityProgress(float remainingTime, float progress, AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.Ability1:
                    _primaryTile.SetAbilityProgress(remainingTime, progress);
                    break;

                case AbilityType.Ability2:
                    _secondaryTile.SetAbilityProgress(remainingTime, progress);
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
                    _primaryTile.TriggerAbilityFx();
                    break;

                case AbilityType.Ability2:
                    _secondaryTile.TriggerAbilityFx();
                    break;

                case AbilityType.ExternalAbility:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(abilityType), abilityType, null);
            }
        }

        // ================================
        // Private Functions
        // ================================

        private void _HandleInputTypeUpdated(InputDeviceType inputDeviceType)
        {
            switch (inputDeviceType)
            {
                case InputDeviceType.Keyboard:
                    _primaryTile.SetAbilityKeyIcon(_primaryKeyboardIcon);
                    _secondaryTile.SetAbilityKeyIcon(_secondaryKeyboardIcon);
                    break;

                case InputDeviceType.Gamepad:
                    _primaryTile.SetAbilityKeyIcon(_primaryGamepadIcon);
                    _secondaryTile.SetAbilityKeyIcon(_secondaryGamepadIcon);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(inputDeviceType), inputDeviceType, null);
            }
        }
    }
}