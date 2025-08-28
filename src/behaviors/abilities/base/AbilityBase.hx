package src.behaviors.abilities.base;

import gdscript.ObjectEx;
import godot.*;
import src.behaviors.abilities.base.AbilityDisplay;

abstract class AbilityBase extends Node3D {
	// ================================
	// Constants
	// ================================
	@:const
	public static final DEFAULT_COOLDOWN_MULTIPLIER:Float = 1;

	// ================================
	// Export
	// ================================
	@:export
	var abilityDisplay:AbilityDisplay;

	// Signals

	@:signal
	function onAbilityStarted(ability:AbilityBase) {}

	public static inline var ON_ABILITY_STARTED:String = "onAbilityStarted";

	@:signal
	function onAbilityEnded(ability:AbilityBase) {}

	public static inline var ON_ABILITY_ENDED:String = "onAbilityEnded";

	@:signal
	function onAbilityCooldownComplete(ability:AbilityBase) {}

	public static inline var ON_ABILITY_COOLDOWN_COMPLETE:String = "onAbilityCooldownComplete";

	@:signal
	function onAbilityStackUpdated(ability:AbilityBase) {}

	public static inline var ON_ABILITY_STACK_UPDATED:String = "onAbilityStackUpdated";

	// Data
	private var _abilityActive:Bool;
	private var _currentCooldownDuration:Float;
	private var _currentStackCount:Int;
	private var _cooldownMultiplier:Float;

	// ================================
	// Properties
	// ================================
	public var abilityActive(get, never):Bool;

	private function get_abilityActive():Bool {
		return _abilityActive;
	}

	public var currentCooldownDuration(get, never):Float;

	private function get_currentCooldownDuration():Float {
		return _currentCooldownDuration;
	}

	public var currentStackCount(get, never):Int;

	private function get_currentStackCount():Int {
		return _currentStackCount;
	}

	public var cooldownMultiplier(get, set):Float;

	private function get_cooldownMultiplier():Float {
		return _cooldownMultiplier;
	}

	private function set_cooldownMultiplier(value:Float):Float {
		_cooldownMultiplier = value;
		return _cooldownMultiplier;
	}

	// ================================
	// Public Functions
	// ================================

	public function getAbilityDisplay():AbilityDisplay {
		return abilityDisplay;
	}

	// ================================
	// Ability Functions
	// ================================

	public function abilityStart():Void {
		_abilityActive = true;
		ObjectEx.emit_signal(ON_ABILITY_STARTED, this);
	}

	public function abilityEnd():Void {
		_abilityActive = false;
		ObjectEx.emit_signal(ON_ABILITY_ENDED, this);
	}

	public function abilityCanStart(activeAbilities:Array<AbilityBase>):Bool {
		if (_currentCooldownDuration > 0 && _currentStackCount <= 0) {
			return false;
		}

		for (ability in activeAbilities) {
			var display:AbilityDisplay = ability.getAbilityDisplay();

			if (abilityDisplay.isMovementAbility && display.isMovementAbility) {
				return false;
			}

			if (abilityDisplay.disallowedAbilities.contains(display.abilityEnum)) {
				return false;
			}
		}

		return true;
	}

	public function abilityNeedsToEnd():Bool {
		return true;
	}

	// ================================
	// Override Functions
	// ================================

	public override function _process(delta:Float):Void {
		if (_currentCooldownDuration > 0) {
			_currentCooldownDuration -= delta * _cooldownMultiplier;

			if (_currentCooldownDuration <= 0) {
				if (_currentStackCount < abilityDisplay.stackCount) {
					_currentStackCount += 1;
					_currentCooldownDuration = abilityDisplay.cooldownDuration;
					ObjectEx.emit_signal(ON_ABILITY_STACK_UPDATED, this);
				} else {
					_currentCooldownDuration = 0;
					ObjectEx.emit_signal(ON_ABILITY_COOLDOWN_COMPLETE, this);
				}
			}
		}
	}

	// ================================
	// Cooldown Functions
	// ================================

	public function fixedCooldownReduction(amount:Float):Void {
		_currentCooldownDuration -= amount;
		if (_currentCooldownDuration < 0) {
			_currentCooldownDuration = 0;
		}
	}

	public function percentCooldownReduction(percent:Float):Void {
		var amount:Float = _currentCooldownDuration * percent;
		fixedCooldownReduction(amount);
	}
}
