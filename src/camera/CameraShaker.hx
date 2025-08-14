package src.camera;

import godot.*;

class CameraShaker {
	// ================================
	// Constants
	// ================================
	@:const
	private static final CAMERA_ORIGIN_THRESHOLD:Float = 0.05;

	var _mainCamera:Camera3D;

	// Shake Data
	private var _strength:Float;
	private var _decay:Float;
	private var _magnitude:Float;
	private var _force:Float;
	private var _range:Float;

	// Data
	private var _origin:Vector3;
	private var _offset:Vector2;
	private var _isShaking:Bool;
	private var _isAtOrigin:Bool;

	private var _rng:RandomNumberGenerator;

	// ================================
	// Public Functions
	// ================================

	public function new(camera:Camera3D) {
		_strength = 1;
		_decay = 1;
		_magnitude = 1;
		_force = 0;
		_range = 1;

		_offset = new Vector2(1, 1);
		_isShaking = false;
		_isAtOrigin = true;

		_rng = new RandomNumberGenerator();
		_mainCamera = camera;
		_origin = _mainCamera.get_global_position();
	}

	public function process(delta:Float):Void {
		if (_force > 0) {
			_force = Godot.max(_force - (_decay) * delta, 0);
			_range = Godot.max(_range - (_decay) * delta, 0);
			_shake();
		} else {
			_isShaking = false;
		}

		if (!_isShaking && !_isAtOrigin) {
			_returnToOrigin(delta);
		}
	}

	public function startShake(decay:Float, magnitude:Float):Void {
		_decay = decay;
		_magnitude = magnitude;

		_range = 1;
		_force = 1;
		_isAtOrigin = false;
		_isShaking = true;
	}

	// ================================
	// Private Functions
	// ================================

	private function _shake():Void {
		if (!_isShaking) {
			return;
		}

		var amount = _force * _strength;
		var offsetX = _offset.x * amount * _rng.randf_range(-_range, _range);
		var offsetY = _offset.y * amount * _rng.randf_range(-_range, _range);

		_mainCamera.position = Vector3.add(_origin, new Vector3(offsetX * _magnitude, offsetY * _magnitude, 0));
	}

	private function _returnToOrigin(delta:Float):Void {
		_mainCamera.position = Godot.lerp(_mainCamera.position, _origin, delta * 2);
		if (_origin.distance_to(_mainCamera.position) <= CAMERA_ORIGIN_THRESHOLD) {
			_isAtOrigin = true;
		}
	}
}
