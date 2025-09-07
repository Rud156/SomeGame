using Godot;
using Godot.Collections;

namespace SomeGame.Behaviors.Abilities.Base
{
    [GlobalClass]
    public partial class AbilityDisplay : Resource
    {
        // ================================
        // Export
        // ================================

        [Export] public string abilityName;

        [Export] public string abilityDescription;

        [Export] public Texture2D abilityIcon;

        [Export] public bool isMovementAbility;

        [Export] public AbilityType abilityType;

        [Export] public AbilityEnum abilityEnum;

        [Export] public int abilityPriorityIndex;

        [Export] public float cooldownDuration;

        [Export] public int stackCount;

        [Export] public Array<PackedScene> abilityEffect;

        [Export] public Array<Vector3> abilityEffectOffset;

        [Export] public Array<AbilityEnum> disallowedAbilities;
    }
}