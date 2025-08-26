package src.behaviors.abilities.base;

import godot.*;
import src.behaviors.abilities.base.AbilityEnum;
import src.behaviors.abilities.base.AbilityType;

class AbilityDisplay extends Resource {
	@:export
	var abilityName:String;
	@:export
	var abilityDescription:String;
	@:export
	var abilityIcon:Texture2D;
	@:export
	var isMovementAbility:Bool;
	@:export
	var abilityType:AbilityType;
	@:export
	var abilityEnum:AbilityEnum;
	@:export
	var cooldownDuration:Float;
	@:export
	var stackCount:Int;
	@:export
	var abilityEffect:Array<PackedScene>;
	@:export
	var abilitySpawnOffset:Array<Vector3>;
	@:export
	var disallowedAbilities:Array<AbilityEnum>;
}
