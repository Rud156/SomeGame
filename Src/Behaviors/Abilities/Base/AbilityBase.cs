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

        [Export] private AbilityDisplay _abilityDisplay;

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
        protected bool abilityActive;
        protected bool markedForEnd;
        protected float currentCooldownDuration;
        protected int currentStackCount;
        protected float cooldownMultiplier;

        // Components
        protected AbilityProcessor abilityProcessor;


        // ================================
        // Properties
        // ================================

        public AbilityDisplay AbilityDisplay => _abilityDisplay;
        public bool AbilityActive => abilityActive;
        public bool MarkedForEnd => markedForEnd;
        public float CurrentCooldownDuration => currentCooldownDuration;
        public int CurrentStackCount => currentStackCount;

        public float CooldownMultiplier
        {
            get => cooldownMultiplier;
            set => cooldownMultiplier = value;
        }

        // ================================
        // Input Functions
        // ================================

        protected virtual bool IsAbilityTriggerPressed(AbilityType abilityType) => false;

        protected virtual Vector2 GetMovementInput() => Vector2.Zero;

        // ================================
        // Ability Functions
        // ================================

        public virtual void Initialize(AbilityProcessor abilityProcessor)
        {
            this.abilityProcessor = abilityProcessor;
            abilityActive = false;
            markedForEnd = true;
        }

        public virtual void Start()
        {
            abilityActive = true;
            markedForEnd = false;
            EmitSignal(SignalName.OnAbilityStarted, this);
        }

        public virtual void Update(float delta)
        {
        }

        public virtual void End()
        {
            abilityActive = false;
            markedForEnd = true;
            EmitSignal(SignalName.OnAbilityEnded, this);
        }

        public virtual bool CanStart(List<AbilityBase> activeAbilities)
        {
            // This means the ability has already started we can safely ignore it...
            if (abilityActive)
            {
                return false;
            }

            if (currentCooldownDuration > 0 && currentStackCount <= 0)
            {
                return false;
            }

            foreach (var ability in activeAbilities)
            {
                var display = ability._abilityDisplay;
                if (_abilityDisplay.disallowedAbilities.Contains(display.abilityEnum))
                {
                    return _abilityDisplay.abilityPriorityIndex > display.abilityPriorityIndex;
                }
            }

            return true;
        }

        public virtual bool NeedsToEnd()
        {
            return markedForEnd;
        }

        // ================================
        // Override Functions
        // ================================

        public override void _Process(double delta)
        {
            if (currentCooldownDuration > 0)
            {
                currentCooldownDuration -= (float)delta * cooldownMultiplier;

                if (currentCooldownDuration <= 0)
                {
                    if (currentStackCount < _abilityDisplay.stackCount)
                    {
                        currentStackCount += 1;
                        currentCooldownDuration = _abilityDisplay.cooldownDuration;
                        EmitSignal(SignalName.OnAbilityStackUpdated, this);
                    }
                    else
                    {
                        currentCooldownDuration = 0;
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
            currentCooldownDuration -= amount;
            if (currentCooldownDuration < 0)
            {
                currentCooldownDuration = 0;
            }
        }

        public void PercentCooldownReduction(float percent)
        {
            var amount = currentCooldownDuration * percent;
            FixedCooldownReduction(amount);
        }

        public void ResetCooldownMultiplier() => cooldownMultiplier = DefaultCooldownMultiplier;

        public void SetCooldownMultiplier(float cooldownMultiplier) => this.cooldownMultiplier = cooldownMultiplier;
    }
}