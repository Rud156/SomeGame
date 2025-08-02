package src.player;

import godot.*;

// This is an input based Event Processor
class PlayerInputController extends Node {
	private static var _instance:PlayerInputController;
	public static var instance(get, never):PlayerInputController;

	public static function get_instance():PlayerInputController {
		return _instance;
	}

	// Basic Movement
	private static final FORWARD_EVENT:String = "FORWARD";
	private static final BACKWARD_EVENT:String = "BACKWARD";
	private static final LEFT_EVENT:String = "LEFT";
	private static final RIGHT_EVENT:String = "RIGHT";
	private static final JUMP_EVENT:String = "JUMP";

	// Ability Inputs
	private static final ABILITY_1_EVENT:String = "ABILITY_1";
	private static final ABILITY_2_EVENT:String = "ABILITY_2";
	private static final ABILITY_3_EVENT:String = "ABILITY_3";
	private static final ABILITY_4_EVENT:String = "ABILITY_4";

	// Data
	private var _mouseInput:Vector2;
	private var _movementInput:Vector2;
	private var _jumpPressed:Bool;
	private var _ability1Pressed:Bool;
	private var _ability2Pressed:Bool;
	private var _ability3Pressed:Bool;
	private var _ability4Pressed:Bool;

	// ================================
	// Properties
	// ================================
	public var mouseInput(get, never):Vector2;

	private function get_mouseInput():Vector2 {
		return _mouseInput;
	}

	public var movementInput(get, never):Vector2;

	private function get_movementInput():Vector2 {
		return _movementInput;
	}

	public var jumpPressed(get, never):Bool;

	private function get_jumpPressed():Bool {
		return _jumpPressed;
	}

	public var ability1Pressed(get, never):Bool;

	private function get_ability1Pressed():Bool {
		return _ability1Pressed;
	}

	public var ability2Pressed(get, never):Bool;

	private function get_ability2Pressed():Bool {
		return _ability2Pressed;
	}

	public var ability3Pressed(get, never):Bool;

	private function get_ability3Pressed():Bool {
		return _ability3Pressed;
	}

	public var ability4Pressed(get, never):Bool;

	private function get_ability4Pressed():Bool {
		return _ability4Pressed;
	}

	// ================================
	// Override Functions
	// ================================
	public override function _ready():Void {
		_instance = this;
	}

	public override function _input(event:InputEvent):Void {
		if (event.is_action_pressed(JUMP_EVENT)) {
			_jumpPressed = true;
		}

		if (event is InputEventMouseMotion) {
			var inputMouseMotion:InputEventMouseMotion = cast event;
			_mouseInput = inputMouseMotion.relative;
		}
	}

	public override function _process(delta:Float):Void {
		_resetInputs();
	}

	public override function _physics_process(delta:Float):Void {
		_movementInput = Input.get_vector(LEFT_EVENT, RIGHT_EVENT, BACKWARD_EVENT, FORWARD_EVENT);
		_ability1Pressed = Input.is_action_pressed(ABILITY_1_EVENT);
		_ability2Pressed = Input.is_action_pressed(ABILITY_2_EVENT);
		_ability3Pressed = Input.is_action_pressed(ABILITY_3_EVENT);
		_ability4Pressed = Input.is_action_pressed(ABILITY_4_EVENT);
	}

	// ================================
	// Public Functions
	// ================================
	public function SetMouseMode(capture:Bool) {
		Input.set_mouse_mode(capture ? Input_MouseMode.MOUSE_MODE_CAPTURED : Input_MouseMode.MOUSE_MODE_VISIBLE);
	}

	// ================================
	// Private Functions
	// ================================
	private function _resetInputs() {
		_jumpPressed = false;
	}
}
