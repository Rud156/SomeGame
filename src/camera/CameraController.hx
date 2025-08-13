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
	var cameraFollowDistance:Vector3;
	@:export
	var cameraFollowSpeed:Float;
	@:export
	var cameraAcceleration:Float;

	// Data
	private var _target:Node3D;
	private var _cameraShaker:CameraShaker;

	// ================================
	// Override Functions
	// ================================

	public override function _ready():Void {
		var camera:Camera3D = get_viewport().get_camera_3d();
		_cameraShaker = new CameraShaker(camera);
	}

	public override function _process(delta:Float):Void {
		_cameraShaker.process(delta);
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
}
