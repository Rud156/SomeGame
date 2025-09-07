using System.Collections.Generic;
using Godot;

namespace SomeGame.Behaviors.Abilities.Base
{
    public abstract partial class AbilityBase : Node3D
    {
        // ================================
        // Constants
        // ================================

        private const float DefaultCooldownMultiplier = 1f;

        // ================================
        // Export
        // ================================

        [Export] public AbilityDisplay abilityDisplay;

        // Signals
        [Signal]
        public delegate void OnAbilityStartedEventHandler(AbilityBase ability);

        [Signal]
        public delegate void OnAbilityEndedEventHandler(AbilityBase ability);

        [Signal]
        public delegate void OnAbilityCooldownCompleteEventHandler(AbilityBase ability);

        [Signal]
        public delegate void OnAbilityStackUpdatedEventHandler(AbilityBase ability);

        // Data
        private bool _abilityActive;
        private float _currentCooldownDuration;
        private int _currentStackCount;
        private float _cooldownMultiplier;

        // ================================
        // Properties
        // ================================

        public bool AbilityActive => _abilityActive;
        public float CurrentCooldownDuration => _currentCooldownDuration;
        public int CurrentStackCount => _currentStackCount;

        public float CooldownMultiplier
        {
            get => _cooldownMultiplier;
            set => _cooldownMultiplier = value;
        }

        // ================================
        // Ability Functions
        // ================================

        public virtual void Initialize()
        {
        }

        public virtual void Start()
        {
            _abilityActive = true;
            EmitSignal(SignalName.OnAbilityStarted, this);
        }

        public virtual void End()
        {
            _abilityActive = false;
            EmitSignal(SignalName.OnAbilityEnded, this);
        }

        public virtual bool CanStart(List<AbilityBase> activeAbilities)
        {
            if (_currentCooldownDuration > 0 && _currentStackCount <= 0)
            {
                return false;
            }

            foreach (var ability in activeAbilities)
            {
                var display = ability.abilityDisplay;

                if (abilityDisplay.disallowedAbilities.Contains(display.abilityEnum))
                {
                    return false;
                }
            }

            return true;
        }

        public virtual bool NeedsToEnd()
        {
            return true;
        }

        // ================================
        // Override Functions
        // ================================

        public override void _Process(double delta)
        {
            if (_currentCooldownDuration > 0)
            {
                _currentCooldownDuration -= (float)delta * _cooldownMultiplier;

                if (_currentCooldownDuration <= 0)
                {
                    if (_currentStackCount < abilityDisplay.stackCount)
                    {
                        _currentStackCount += 1;
                        _currentCooldownDuration = abilityDisplay.cooldownDuration;
                        EmitSignal(SignalName.OnAbilityStackUpdated, this);
                    }
                    else
                    {
                        _currentCooldownDuration = 0;
                        EmitSignal(SignalName.OnAbilityCooldownComplete, this);
                    }
                }
            }
        }

        // ================================
        // Cooldown Functions
        // ================================

        public void FixedCooldownReduction(float amount)
        {
            _currentCooldownDuration -= amount;
            if (_currentCooldownDuration < 0)
            {
                _currentCooldownDuration = 0;
            }
        }

        public void PercentCooldownReduction(float percent)
        {
            var amount = _currentCooldownDuration * percent;
            FixedCooldownReduction(amount);
        }

        public void ResetCooldownMultiplier() => _cooldownMultiplier = DefaultCooldownMultiplier;

        public void SetCooldownMultiplier(float cooldownMultiplier) => _cooldownMultiplier = cooldownMultiplier;
    }
}