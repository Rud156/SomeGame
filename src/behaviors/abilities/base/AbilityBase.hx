package src.behaviors.abilities.base;

import godot.Node3D;
import src.behaviors.abilities.base.AbilityDisplay;

class AbilityBase extends Node3D {
	@:export
	var abilityDisplay:AbilityDisplay;

	// ================================
	// Override Functions
	// ================================

	public override function _process(delta:Float):Void {}
}
