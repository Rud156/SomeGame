package src.behaviors.hitstop;

import godot.*;
import src.behaviors.hitstop.HitStopBehavior;

class HitStopAnimator extends Node3D {
	// ================================
	// Constants
	// ================================
	private static inline var ANIMATION_SPEED:String = "parameters/AnimatorSpeed/scale";

	// ================================
	// Export
	// ================================
	@:export
	var hitStopBehavior:HitStopBehavior;
	@:export
	var hitStopAnimator:AnimationTree;

	// Data
	private var _handleHitStopBehaviorCallable:Callable;
	private var _hsAnimSpeed:Float;
	// ================================
	// Override Functions
	// ================================

	public override function _ready():Void {
		_handleHitStopBehaviorCallable = Callable.create(this, "_handleHitStopBehaviorChanged");
		hitStopBehavior.connect(HitStopBehavior.HIT_STOP_STATE_CHANGED, _handleHitStopBehaviorCallable);
	}

	public override function _exit_tree():Void {
		hitStopBehavior.disconnect(HitStopBehavior.HIT_STOP_STATE_CHANGED, _handleHitStopBehaviorCallable);
	}

	// ================================
	// Private Functions
	// ================================

	private function _handleHitStopBehaviorChanged(active:Bool):Void {
		if (active) {
			_hsAnimSpeed = hitStopAnimator.get(ANIMATION_SPEED);
			hitStopAnimator.set(ANIMATION_SPEED, 0);
		} else {
			hitStopAnimator.set(ANIMATION_SPEED, _hsAnimSpeed);
		}
	}
}
