using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace SomeGame.Behaviors.Abilities.Base
{
    public partial class AbilityProcessor : Node3D
    {
        // ================================
        // Export
        // ================================
        [Export] private Array<PackedScene> _abilities;
        [Export] private AnimationTree _animationTree;
        [Export] private Node3D _character;
        [Export] private Node3D _characterMesh;

        [Signal]
        public delegate void OnAbilityStartedEventHandler(AbilityBase ability);

        [Signal]
        public delegate void OnAbilityEndedEventHandler(AbilityBase ability);

        // Data
        private List<AbilityBase> _allAbilities;
        private List<AbilityBase> _activeAbilities;

        // These are abilities that will be added next frame and also are always added externally
        // So like effects like Knockback, Stun etc.
        // So these always take precedence over user inputted abilities...
        private List<AbilityBase> _abilitiesToAddNextFrame;

        // ================================
        // Properties
        // ================================
        public List<AbilityBase> ActiveAbilities => _activeAbilities;
        public AnimationTree AnimationTree => _animationTree;
        public Node3D Character => _character;
        public Node3D CharacterMesh => _characterMesh;

        // ================================
        // Override Functions
        // ================================

        public override void _Ready()
        {
            _allAbilities = [];
            _activeAbilities = [];
            _abilitiesToAddNextFrame = [];

            foreach (var abilityPackedScene in _abilities)
            {
                var ability = (AbilityBase)abilityPackedScene.Instantiate();
                ability.Initialize(this);

                _allAbilities.Add(ability);
                AddChild(ability);
            }
        }

        public override void _Process(double delta)
        {
            for (var i = _activeAbilities.Count - 1; i >= 0; i--)
            {
                var ability = _activeAbilities[i];
                if (ability.NeedsToEnd())
                {
                    ability.End();
                    _activeAbilities.RemoveAt(i);
                    EmitSignal(SignalName.OnAbilityEnded, ability);
                }
            }

            _CheckAndActivateAbilities();
            _ProcessNextFrameAbilities();
        }

        // ================================
        // Public Functions
        // ================================

        public void AddExternalAbility(AbilityBase ability)
        {
            _abilitiesToAddNextFrame.Add(ability);
        }

        // ================================
        // Private Functions
        // ================================

        private void _CheckAndActivateAbilities()
        {
            foreach (var abilityBase in _allAbilities.Where(abilityBase => abilityBase.CanStart(_activeAbilities)))
            {
                abilityBase.Start();
                _activeAbilities.Add(abilityBase);
                EmitSignal(SignalName.OnAbilityStarted, abilityBase);
            }
        }

        private void _ProcessNextFrameAbilities()
        {
            foreach (var newAbility in _abilitiesToAddNextFrame)
            {
                var newAbilityDisplay = newAbility.AbilityDisplay;
                var canStartNewAbility = true;

                for (var i = _activeAbilities.Count - 1; i >= 0; i--)
                {
                    var activeAbility = _activeAbilities[i];
                    var activeAbilityDisplay = activeAbility.AbilityDisplay;

                    if (newAbilityDisplay.disallowedAbilities.Contains(activeAbilityDisplay.abilityEnum))
                    {
                        // This means that the new ability is more important
                        // So we need to kill the existing invalid one and then start the new one
                        if (newAbilityDisplay.abilityPriorityIndex > activeAbilityDisplay.abilityPriorityIndex)
                        {
                            activeAbility.End();
                            _activeAbilities.RemoveAt(i);
                            EmitSignal(SignalName.OnAbilityEnded, activeAbility);
                        }
                        // Otherwise, we cannot start the new ability
                        else
                        {
                            canStartNewAbility = false;
                        }
                    }
                }

                if (canStartNewAbility)
                {
                    newAbility.Start();
                    _activeAbilities.Add(newAbility);
                    EmitSignal(SignalName.OnAbilityStarted, newAbility);
                }
            }

            _abilitiesToAddNextFrame.Clear();
        }
    }
}