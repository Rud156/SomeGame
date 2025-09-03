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
	var abilities:Array<Node3D>;

	@:signal
	function onAbilityStarted(ability:AbilityBase) {}

	public static inline var ON_ABILITY_STARTED:String = "onAbilityStarted";

	@:signal
	function onAbilityEnded(ability:AbilityBase) {}

	public static inline var ON_ABILITY_ENDED:String = "onAbilityEnded";

	// Data
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

	public override function _process(delta:Float):Void {
		var index:Int = _activeAbilities.length - 1;
		while (index >= 0) {
			var ability:AbilityBase = _activeAbilities[index];
			if (ability.abilityNeedsToEnd()) {
				ability.abilityEnd();
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
		for (i in 0...abilities.length) {
			var abilityBase:AbilityBase = cast(abilities[i], AbilityBase);
			if (abilityBase.abilityCanStart(_activeAbilities)) {
				abilityBase.abilityStart();
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
						activeAbility.abilityEnd();
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
				newAbility.abilityStart();
				_activeAbilities.push(newAbility);
				ObjectEx.emit_signal(ON_ABILITY_STARTED, newAbility);
			}

			index -= 1;
		}

		_abilitiesToAddNextFrame = [];
	}
}
