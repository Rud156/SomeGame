package src.behaviors.abilities.base;

import godot.*;
import src.behaviors.abilities.base.AbilityBase;

abstract class MovementAbilityBase extends AbilityBase {
	private var _movementData:Vector3;

	// ================================
	// Properties
	// ================================
	public var movementData(get, set):Vector3;

	private function get_movementData():Vector3 {
		return _movementData;
	}

	private function set_movementData(value:Vector3):Vector3 {
		_movementData = value;
		return _movementData;
	}
}
