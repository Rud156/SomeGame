using Godot;
using SomeGame.Behaviors.Abilities.Base;

namespace SomeGame.Behaviors.Abilities.Base
{
    public abstract partial class MovementAbilityBase : AbilityBase
    {
        // ================================
        // Properties
        // ================================

        public Vector3 MovementData { get; set; }
    }
}