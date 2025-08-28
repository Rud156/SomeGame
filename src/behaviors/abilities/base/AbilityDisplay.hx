package src.behaviors.abilities.base;

import godot.*;
import src.behaviors.abilities.base.AbilityEnum;
import src.behaviors.abilities.base.AbilityType;

class AbilityDisplay extends Resource {
	// ================================
	// Export
	// ================================
	@:export
	public var abilityName:String;
	@:export
	public var abilityDescription:String;
	@:export
	public var abilityIcon:Texture2D;
	@:export
	public var isMovementAbility:Bool;
	@:export
	public var abilityType:AbilityType;
	@:export
	public var abilityEnum:AbilityEnum;
	@:export
	public var abilityPriorityIndex:Int;
	@:export
	public var cooldownDuration:Float;
	@:export
	public var stackCount:Int;
	@:export
	public var abilityEffect:Array<PackedScene>;
	@:export
	public var abilityEffectOffset:Array<Vector3>;
	@:export
	public var disallowedAbilities:Array<AbilityEnum>;
}
