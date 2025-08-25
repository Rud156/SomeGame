package src.player;

import gdscript.ObjectEx;
import godot.*;
import src.behaviors.hitstop.HitStopBehavior;
import src.camera.CameraController;
import src.player.PlayerInputController;

class PlayerController extends CharacterBody3D {
	// ================================
	// Export
	// ================================
	@:export
	var groundedAcceleration:Float;
	@:export
	var groundedDeceleration:Float;
	@:export
	var maxGroundedSpeed:Float;
	@:export
	var airAcceleration:Float;
	@:export
	var maxAirSpeed:Float;
	@:export
	var jumpVelocity:Float;
	@:export
	var maxJumpCount:Int;
	@:export
	var gravityMultiplier:Float;
	@:export
	var hitStopBehavior:HitStopBehavior;

	// Signals

	@:signal
	function onJumpTriggered() {}

	public static inline var ON_JUMP_TRIGGERED:String = "onJumpTriggered";

	@:signal
	function onPlayerStateChanged(movementState:PlayerMovementState) {}

	public static inline var ON_PLAYER_STATE_CHANGED:String = "onPlayerStateChanged";

	// ================================
	// Constants
	// ================================
	@:const
	private static final GRAVITY:Float = -9.8;

	// Movement Modifiers
	private var _movementVelocity:Vector3;

	// State
	private var _movementStack:Array<PlayerMovementState>;
	private var _jumpPressed:Bool;
	private var _currentJumpCount:Int;
	private var _currentMovementSpeed:Float;

	// Events
	private var _handleJumpPressedCallable:Callable;

	// ================================
	// Override Functions
	// ================================

	public override function _ready():Void {
		_movementVelocity = Vector3.ZERO;
		_pushMovementState(PlayerMovementState.NORMAL);
		_resetFallingStateData();

		_handleJumpPressedCallable = Callable.create(this, "_handleJumpPressed");
		PlayerInputController.instance.connect(PlayerInputController.ON_JUMP_PRESSED, _handleJumpPressedCallable);

		CameraController.instance.setTargetObject(this);
	}

	public override function _exit_tree():Void {
		PlayerInputController.instance.disconnect(PlayerInputController.ON_JUMP_PRESSED, _handleJumpPressedCallable);
	}

	public override function _process(delta:Float):Void {
		if (!hitStopBehavior.isActive) {
			_updateGroundedState();
			_handleMovement(delta);

			_processGravity();
			_applyJump();

			_applyMovement(delta);
			_updateMeshRotation();
		}
	}

	// ================================
	// Public Functions
	// ================================

	public function peekMovementState():PlayerMovementState {
		return _movementStack[_movementStack.length - 1];
	}

	// ================================
	// Private Functions
	// ================================

	private function _updateGroundedState():Void {
		var movementState:PlayerMovementState = peekMovementState();
		if (!is_on_floor() && movementState != PlayerMovementState.FALLING && movementState != PlayerMovementState.CUSTOM_MOVEMENT) {
			_pushMovementState(PlayerMovementState.FALLING);
		}
	}

	private function _handleMovement(delta:Float):Void {
		switch (_movementStack[_movementStack.length - 1]) {
			case PlayerMovementState.NORMAL:
				_updateNormalState(delta);

			case PlayerMovementState.FALLING:
				_updateFallingState(delta);

			case PlayerMovementState.CUSTOM_MOVEMENT:
				_updateCustomMovement(delta);
		}
	}

	private function _updateNormalState(delta:Float):Void {
		if (PlayerInputController.instance.hasNoDirectionalInput()) {
			_currentMovementSpeed -= groundedDeceleration * delta;
		} else {
			_currentMovementSpeed += groundedAcceleration * delta;
		}
		_currentMovementSpeed = Godot.clampf(_currentMovementSpeed, 0, maxGroundedSpeed);

		var lastPlayerInput:Vector2 = PlayerInputController.instance.lastNonZeroMovementInput;
		var forward:Vector3 = -Vector3.FORWARD;
		var right:Vector3 = -Vector3.RIGHT;

		var mappedMovement:Vector3 = Vector3.add(Vector3.mult2(forward, lastPlayerInput.y), Vector3.mult2(right, lastPlayerInput.x));
		mappedMovement.y = 0;
		mappedMovement = mappedMovement.normalized() * _currentMovementSpeed;

		_movementVelocity.x = mappedMovement.x;
		_movementVelocity.z = mappedMovement.z;
	}

	private function _updateFallingState(delta:Float):Void {
		if (!PlayerInputController.instance.hasNoDirectionalInput()) {
			_currentMovementSpeed += airAcceleration * delta;
		}
		_currentMovementSpeed = Godot.clampf(_currentMovementSpeed, 0, maxAirSpeed);

		var lastPlayerInput:Vector2 = PlayerInputController.instance.lastNonZeroMovementInput;
		var forward:Vector3 = -Vector3.FORWARD;
		var right:Vector3 = -Vector3.RIGHT;

		var mappedMovement:Vector3 = Vector3.add(Vector3.mult2(forward, lastPlayerInput.y), Vector3.mult2(right, lastPlayerInput.x));
		mappedMovement.y = 0;
		mappedMovement = mappedMovement.normalized() * _currentMovementSpeed;

		_movementVelocity.x = Godot.clampf(_movementVelocity.x + mappedMovement.x, -maxAirSpeed, maxAirSpeed);
		_movementVelocity.z = Godot.clampf(_movementVelocity.z + mappedMovement.z, -maxAirSpeed, maxAirSpeed);

		if (is_on_floor()) {
			_resetFallingStateData();
			_popMovementState();
		}
	}

	private function _resetFallingStateData():Void {
		_currentJumpCount = 0;
	}

	private function _updateCustomMovement(_):Void {}

	private function _processGravity():Void {
		if (!is_on_floor()) {
			var yVelocity:Float = _movementVelocity.y;
			yVelocity += GRAVITY * gravityMultiplier;
			_movementVelocity.y = yVelocity;
		} else {
			_movementVelocity.y = GRAVITY * gravityMultiplier;
		}
	}

	private function _applyJump():Void {
		if (_jumpPressed) {
			_movementVelocity.y = jumpVelocity;
			_jumpPressed = false;
			ObjectEx.emit_signal(ON_JUMP_TRIGGERED);
		}
	}

	private function _handleJumpPressed():Void {
		if (_currentJumpCount < maxJumpCount) {
			_jumpPressed = true;
			_currentJumpCount += 1;
		}
	}

	private function _applyMovement(delta:Float):Void {
		velocity = _movementVelocity * delta;
		move_and_slide();
	}

	private function _updateMeshRotation():Void {
		var mousePosition:Vector3 = PlayerInputController.instance.mousePosition;
		var playerPosition:Vector3 = get_global_position();
		var targetPosition:Vector3 = new Vector3(mousePosition.x, playerPosition.y, mousePosition.z);
		if (!playerPosition.is_equal_approx(targetPosition)) {
			look_at(targetPosition, Vector3.UP);
		}
	}

	private function _pushMovementState(movementState:PlayerMovementState):Void {
		_movementStack.push(movementState);
		ObjectEx.emit_signal(ON_PLAYER_STATE_CHANGED, movementState);
	}

	private function _popMovementState():Void {
		_movementStack.pop();
		ObjectEx.emit_signal(ON_PLAYER_STATE_CHANGED, peekMovementState());
	}
}

// ================================
// Enums
// ================================
enum PlayerMovementState {
	NORMAL;
	FALLING;
	CUSTOM_MOVEMENT;
}
