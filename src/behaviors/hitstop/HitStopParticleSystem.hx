package src.behaviors.hitstop;

import godot.*;
import src.behaviors.hitstop.HitStopBehavior;

class HitStopParticleSystem extends Node3D {
	// ================================
	// Export
	// ================================
	@:export
	var hitStopBehavior:HitStopBehavior;

	// Data
	private var _handleHitStopBehaviorCallable:Callable;
	private var _activeParticleSystems:Array<GPUParticles3D>;

	// ================================
	// Override Functions
	// ================================

	public override function _ready():Void {
		_activeParticleSystems = [];

		_handleHitStopBehaviorCallable = Callable.create(this, "_handleHitStopBehaviorChanged");
		hitStopBehavior.connect(HitStopBehavior.HIT_STOP_STATE_CHANGED, _handleHitStopBehaviorCallable);
	}

	public override function _exit_tree():Void {
		hitStopBehavior.disconnect(HitStopBehavior.HIT_STOP_STATE_CHANGED, _handleHitStopBehaviorCallable);
	}

	public override function _process(_):Void {
		_checkAndRemoveInactiveParticles();
	}

	// ================================
	// Public Functions
	// ================================

	public function addParticleSystem(particleSystem:GPUParticles3D):Void {
		_activeParticleSystems.push(particleSystem);
	}

	// ================================
	// Private Functions
	// ================================

	private function _checkAndRemoveInactiveParticles():Void {
		// TODO: Check if this works...
		var index:Int = _activeParticleSystems.length - 1;

		while (index >= 0) {
			if (_activeParticleSystems[index] == null) {
				_activeParticleSystems.splice(index, 1);
				index -= 1;
			}
		}
	}

	private function _handleHitStopBehaviorChanged(active:Bool):Void {
		// TODO: Check if this actually works
		// Also might need to save particles prior speed before setting it back/setting it to 0

		if (active) {
			for (i in 0..._activeParticleSystems.length) {
				_activeParticleSystems[i].set_speed_scale(0);
			}
		} else {
			for (i in 0..._activeParticleSystems.length) {
				_activeParticleSystems[i].set_speed_scale(1);
			}
		}
	}
}
