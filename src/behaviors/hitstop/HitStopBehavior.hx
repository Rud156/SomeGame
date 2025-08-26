package src.behaviors.hitstop;

import gdscript.ObjectEx;
import godot.*;

class HitStopBehavior extends Node3D {
	// ================================
	// Export
	// ================================
	@:signal
	function hitStopStateChanged(active:Bool) {}

	public static inline var HIT_STOP_STATE_CHANGED:String = "hitStopStateChanged";

	// Data
	private var _duration:Float;

	// ================================
	// Export
	// ================================
	public var isActive(get, never):Bool;

	private function get_isActive():Bool {
		return _duration > 0;
	}

	// ================================
	// Override Functions
	// ================================

	public override function _process(delta:Float):Void {
		if (!isActive) {
			return;
		}

		_duration -= delta;
		if (_duration <= 0) {
			disableHitStop();
		}
	}

	// ================================
	// Public Functions
	// ================================

	public function enableHitStop(duration:Float):Void {
		_duration = duration;
		ObjectEx.emit_signal(HIT_STOP_STATE_CHANGED, true);
	}

	public function disableHitStop():Void {
		_duration = 0;
		ObjectEx.emit_signal(HIT_STOP_STATE_CHANGED, false);
	}
}
