package src.camera;

import godot.*;
import src.camera.CameraShaker;

class CameraController extends Node3D {
	private static var _instance:CameraController;
	public static var instance(get, never):CameraController;

	public static function get_instance():CameraController {
		return _instance;
	}

	// ================================
	// Export
	// ================================
	@:export
	var followDistance:Vector3;
	@:export
	var followSpeed:Float;
	@:export
	var followLerpCurve:Curve;
	@:export
	var lookRotationSpeed:Float;

	// Data
	private var _target:Node3D;
	private var _startPosition:Vector3;
	private var _targetPosition:Vector3;
	private var _lerpAmount:Float;
	private var _cameraShaker:CameraShaker;

	// ================================
	// Override Functions
	// ================================

	public function new() {
		super();
		_instance = this;
	}

	public override function _ready():Void {
		var camera:Camera3D = get_viewport().get_camera_3d();
		_cameraShaker = new CameraShaker(camera);

		_startPosition = get_global_position();
		_targetPosition = Vector3.ZERO;
		_lerpAmount = 0;
	}

	public override function _process(delta:Float):Void {
		_cameraShaker.process(delta);

		_lookAtTargetPosition(delta);
		_updateLastTargetPosition();
		_updateCameraFollow(delta);
	}

	// ================================
	// Public Functions
	// ================================

	public function setTargetObject(target:Node3D):Void {
		_target = target;
	}

	public function clearTarget():Void {
		_target = null;
	}

	public function startShake(decay:Float, magnitude:Float):Void {
		_cameraShaker.startShake(decay, magnitude);
	}

	// ================================
	// Private Functions
	// ================================

	private function _lookAtTargetPosition(delta:Float):Void {
		var targetPosition:Vector3 = _target != null ? _target.get_global_position() : _targetPosition;
		var cameraTransform:Transform3D = get_global_transform();
		var lookAtTransform:Transform3D = cameraTransform.looking_at(targetPosition, Vector3.UP);

		var x:Vector3 = Godot.lerp(cameraTransform.basis.x, lookAtTransform.basis.x, lookRotationSpeed * delta);
		var y:Vector3 = Godot.lerp(cameraTransform.basis.y, lookAtTransform.basis.y, lookRotationSpeed * delta);
		var z:Vector3 = Godot.lerp(cameraTransform.basis.z, lookAtTransform.basis.z, lookRotationSpeed * delta);
		set_global_basis(new Basis(x, y, z));
	}

	private function _updateCameraFollow(delta:Float):Void {
		if (_lerpAmount >= 1) {
			return;
		}

		_lerpAmount += followSpeed * delta;

		var lerpValue:Float = followLerpCurve.sample(_lerpAmount);
		var mappedPosition:Vector3 = Godot.lerp(_startPosition, _targetPosition, lerpValue);
		set_position(mappedPosition);
	}

	private function _updateLastTargetPosition():Void {
		if (_target != null) {
			var targetPosition:Vector3 = _target.get_global_position();
			targetPosition = Vector3.add(targetPosition, followDistance);

			if (!targetPosition.is_equal_approx(_targetPosition)) {
				_startPosition = get_global_position();
				_targetPosition = targetPosition;
				_lerpAmount = 0;
			}
		}
	}
}
