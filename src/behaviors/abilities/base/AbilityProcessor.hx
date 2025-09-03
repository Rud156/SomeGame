package src.behaviors.abilities.base;

import gdscript.ObjectEx;
import godot.*;
import src.behaviors.abilities.base.AbilityBase;
import src.behaviors.abilities.base.AbilityDisplay;

abstract class AbilityProcessor extends Node3D {
	// ================================
	// Export
	// ================================
	@:export
	var abilities:Array<PackedScene>;

	@:signal
	function onAbilityStarted(ability:AbilityBase) {}

	public static inline var ON_ABILITY_STARTED:String = "onAbilityStarted";

	@:signal
	function onAbilityEnded(ability:AbilityBase) {}

	public static inline var ON_ABILITY_ENDED:String = "onAbilityEnded";

	// Data
	private var _activeAbilities:Array<AbilityBase>;
	private var _abilities:Array<AbilityBase>;

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

		for (i in 0...abilities.length) {
			var ability:AbilityBase = cast(abilities[i].instantiate(), AbilityBase);
			ability.initialize();
			
			_abilities.push(ability);
			add_child(ability, Node_InternalMode.INTERNAL_MODE_BACK);
		}
	}

	public override function _process(delta:Float):Void {
		var index:Int = _activeAbilities.length - 1;
		while (index >= 0) {
			var ability:AbilityBase = _activeAbilities[index];
			if (ability.needsToEnd()) {
				ability.end();
				_activeAbilities.splice(index, 1);
				ObjectEx.emit_signal(ON_ABILITY_ENDED, ability);
			}

			index -= 1;
		}
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

	private function _checkAndActivateAbilities():Void {
		for (i in 0..._abilities.length) {
			var abilityBase:AbilityBase = _abilities[i];
			if (abilityBase.canStart(_activeAbilities)) {
				abilityBase.start();
				_activeAbilities.push(abilityBase);
				ObjectEx.emit_signal(ON_ABILITY_STARTED, abilityBase);
			}
		}
	}

	private function _processNextFrameAbilities():Void {
		for (newAbility in _abilitiesToAddNextFrame) {
			var newAbilityDisplay:AbilityDisplay = newAbility.getAbilityDisplay();

			var canStartNewAbility:Bool = true;
			var index = _activeAbilities.length - 1;
			while (index >= 0) {
				var activeAbility:AbilityBase = _activeAbilities[index];
				var activeAbilityDisplay:AbilityDisplay = activeAbility.getAbilityDisplay();

				if (newAbilityDisplay.disallowedAbilities.contains(activeAbilityDisplay.abilityEnum)) {
					// This means that the new ability is more important
					// So we need to kill the existing invalid one and then start the new one
					if (newAbilityDisplay.abilityPriorityIndex > activeAbilityDisplay.abilityPriorityIndex) {
						activeAbility.end();
						_activeAbilities.splice(index, 1);
						ObjectEx.emit_signal(ON_ABILITY_ENDED, activeAbility);
					}
					// Otherwise we cannot start the new ability
					else {
						canStartNewAbility = false;
					}
				}
			}

			if (canStartNewAbility) {
				newAbility.start();
				_activeAbilities.push(newAbility);
				ObjectEx.emit_signal(ON_ABILITY_STARTED, newAbility);
			}

			index -= 1;
		}

		_abilitiesToAddNextFrame = [];
	}
}
