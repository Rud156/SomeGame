package src.player;

import gdscript.ObjectEx;
import godot.*;
import src.helpers.MathUtils;

// This is an input based Event Processor
class PlayerInputController extends Node3D {
	private static var _instance:PlayerInputController;
	public static var instance(get, never):PlayerInputController;

	public static function get_instance():PlayerInputController {
		return _instance;
	}

	// ================================
	// Export
	// ================================
	// Signals

	@:signal
	function onJumpPressed() {}

	public static inline var ON_JUMP_PRESSED:String = "onJumpPressed";

	// Basic Movement
	private static inline var FORWARD_EVENT:String = "FORWARD";
	private static inline var BACKWARD_EVENT:String = "BACKWARD";
	private static inline var LEFT_EVENT:String = "LEFT";
	private static inline var RIGHT_EVENT:String = "RIGHT";
	private static inline var JUMP_EVENT:String = "JUMP";

	// Ability Inputs
	private static inline var ABILITY_1_EVENT:String = "ABILITY_1";
	private static inline var ABILITY_2_EVENT:String = "ABILITY_2";

	// Mouse
	@:const
	private static final MOUSE_RAYCAST_DISTANCE:Int = 2000;

	// Data
	private var _mouseInput:Vector2;
	private var _mousePosition:Vector3;
	private var _movementInput:Vector2;
	private var _lastNonZeroMovementInput:Vector2;
	private var _ability1Pressed:Bool;
	private var _ability2Pressed:Bool;

	// ================================
	// Properties
	// ================================
	public var mouseInput(get, never):Vector2;

	private function get_mouseInput():Vector2 {
		return _mouseInput;
	}

	public var mousePosition(get, never):Vector3;

	private function get_mousePosition():Vector3 {
		return _mousePosition;
	}

	public var movementInput(get, never):Vector2;

	private function get_movementInput():Vector2 {
		return _movementInput;
	}

	public var lastNonZeroMovementInput(get, never):Vector2;

	private function get_lastNonZeroMovementInput():Vector2 {
		return _lastNonZeroMovementInput;
	}

	public var ability1Pressed(get, never):Bool;

	private function get_ability1Pressed():Bool {
		return _ability1Pressed;
	}

	public var ability2Pressed(get, never):Bool;

	private function get_ability2Pressed():Bool {
		return _ability2Pressed;
	}

	// ================================
	// Override Functions
	// ================================

	public function new() {
		super();
		_instance = this;
	}

	public override function _input(event:InputEvent):Void {
		if (event.is_action_pressed(JUMP_EVENT)) {
			ObjectEx.emit_signal(ON_JUMP_PRESSED);
		}

		if (event is InputEventMouseMotion) {
			var inputMouseMotion:InputEventMouseMotion = cast event;
			_mouseInput = inputMouseMotion.relative;
		}
	}

	public override function _physics_process(_):Void {
		_mousePosition = _screenPointToRay();
		_movementInput = Input.get_vector(FORWARD_EVENT, BACKWARD_EVENT, RIGHT_EVENT, LEFT_EVENT);
		_ability1Pressed = Input.is_action_pressed(ABILITY_1_EVENT);
		_ability2Pressed = Input.is_action_pressed(ABILITY_2_EVENT);

		if (!MathUtils.IsNearlyZero(_movementInput.x) || !MathUtils.IsNearlyZero(_movementInput.y)) {
			_lastNonZeroMovementInput = _movementInput;
		}
	}

	// ================================
	// Public Functions
	// ================================

	public function setMouseMode(capture:Bool):Void {
		Input.set_mouse_mode(capture ? Input_MouseMode.MOUSE_MODE_CAPTURED : Input_MouseMode.MOUSE_MODE_VISIBLE);
	}

	public function hasNoDirectionalInput():Bool {
		return MathUtils.IsNearlyZero(_movementInput.x) && MathUtils.IsNearlyZero(_movementInput.y);
	}

	// ================================
	// Private Functions
	// ================================

	private function _screenPointToRay():Vector3 {
		var spaceState:PhysicsDirectSpaceState3D = get_world_3d().direct_space_state;
		var mousePosition = get_viewport().get_mouse_position();
		var camera:Camera3D = get_viewport().get_camera_3d();

		var rayOrigin:Vector3 = camera.project_ray_origin(mousePosition);
		var rayEnd:Vector3 = rayOrigin + camera.project_ray_normal(mousePosition) * MOUSE_RAYCAST_DISTANCE;
		var rayArray:Dictionary = spaceState.intersect_ray(PhysicsRayQueryParameters3D.create(rayOrigin, rayEnd));
		if (rayArray.has("position")) {
			return cast rayArray.get("position");
		}

		return Vector3.ZERO;
	}
}
