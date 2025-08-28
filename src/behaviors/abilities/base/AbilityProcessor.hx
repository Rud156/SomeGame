package src.behaviors.abilities.base;

import godot.*;
import src.behaviors.abilities.base.AbilityBase;

abstract class AbilityProcessor extends CharacterBody3D {
	private var _activeAbilities:Array<AbilityBase>;
	
    // These are abilities that will be added next frame and also are always added externally
    // So like effects like Knockback, Stun etc.
    // So these always take precedence over user inputted abilities...
	private var _abilitiesToAddNextFrame:Array<AbilityBase>;

	// ================================
	// Properties
	// ================================
	public var activeAbilities(get, never):Array<AbilityBase>;

	private function get_activeAbilities():Array<AbilityBase> {
		return _activeAbilities;
	}

	// ================================
	// Override Functions
	// ================================

	public override function _ready():Void {
		_activeAbilities = [];
		_abilitiesToAddNextFrame = [];
	}

	// ================================
	// Public Functions
	// ================================

	public function addExternalAbility(ability:AbilityBase):Void {
		_abilitiesToAddNextFrame.push(ability);
	}

	// ================================
	// Private Functions
	// ================================

	private function _checkAndActivateAbilities():Void {}

	private function _processNextFrameAbilities():Void {}
}
