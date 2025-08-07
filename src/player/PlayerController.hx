package src.player;

import godot.*;
import src.helpers.MathUtils;
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

	// ================================
	// Constants
	// ================================
	private static final GRAVITY:Float = -9.8;

	// Movement Modifiers
	private var _movementVelocity:Vector3;

	// State
	private var _movementStack:Array<PlayerMovementState>;
	private var _currentJumpCount:Int;
	private var _currentMovementSpeed:Float;

	// https://github.com/KenneyNL/Starter-Kit-3D-Platformer/blob/main/scripts/player.gd
	// ================================
	// Override Functions
	// ================================

	public override function _ready():Void {
		_movementVelocity = Vector3.ZERO;
		_pushMovementState(PlayerMovementState.NORMAL);
		_resetFallingStateData();
	}

	public override function _process(delta:Float):Void {
		_updateGroundedState();
		_handleMovement(delta);
		_processGravity();

		_handleJumpPressed();
		_applyMovement(delta);
	}

	// ================================
	// Private Functions
	// ================================

	private function _updateGroundedState() {
		var movementState:PlayerMovementState = _peekMovementState();
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
		if (PlayerInputController.instance.HasNoDirectionalInput()) {
			_currentMovementSpeed -= groundedDeceleration * delta;
		} else {
			_currentMovementSpeed += groundedAcceleration * delta;
		}
		_currentMovementSpeed = MathUtils.clampf(_currentMovementSpeed, 0, maxGroundedSpeed);

		var playerInput:Vector2 = PlayerInputController.instance.movementInput;
		var basis:Basis = get_global_basis();
		var forward:Vector3 = basis.z;
		var right:Vector3 = basis.x;

		var mappedMovement:Vector3 = Vector3.add(Vector3.mult2(forward, playerInput.x), Vector3.mult2(right, playerInput.y));
		mappedMovement.y = 0;
		mappedMovement = mappedMovement.normalized() * _currentMovementSpeed;

		_movementVelocity.x = mappedMovement.x;
		_movementVelocity.z = mappedMovement.z;
	}

	private function _updateFallingState(delta:Float):Void {
		if (!PlayerInputController.instance.HasNoDirectionalInput()) {
			_currentMovementSpeed += airAcceleration * delta;
		}
		_currentMovementSpeed = MathUtils.clampf(_currentMovementSpeed, 0, maxAirSpeed);

		var playerInput:Vector2 = PlayerInputController.instance.movementInput;
		var basis:Basis = get_global_basis();
		var forward:Vector3 = basis.z;
		var right:Vector3 = basis.x;

		var mappedMovement:Vector3 = Vector3.add(Vector3.mult2(forward, playerInput.x), Vector3.mult2(right, playerInput.y));
		mappedMovement.y = 0;
		mappedMovement = mappedMovement.normalized() * _currentMovementSpeed;

		_movementVelocity.x = MathUtils.clampf(_movementVelocity.x + mappedMovement.x, -maxAirSpeed, maxAirSpeed);
		_movementVelocity.z = MathUtils.clampf(_movementVelocity.z + mappedMovement.z, -maxAirSpeed, maxAirSpeed);

		if (is_on_floor()) {
			_resetFallingStateData();
			_popMovementState();
		}
	}

	private function _resetFallingStateData() {
		_currentJumpCount = 0;
	}

	private function _updateCustomMovement(delta:Float):Void {}

	private function _processGravity():Void {
		if (!is_on_floor()) {
			var yVelocity:Float = _movementVelocity.y;
			yVelocity += GRAVITY * gravityMultiplier;
			_movementVelocity.y = yVelocity;
		} else {
			_movementVelocity.y = GRAVITY * gravityMultiplier;
		}
	}

	private function _handleJumpPressed() {
		if (PlayerInputController.instance.jumpPressed && _currentJumpCount < maxJumpCount) {
			_movementVelocity.y = jumpVelocity;
			_currentJumpCount += 1;
		}
	}

	private function _applyMovement(delta:Float):Void {
		trace("Velocity: " + _movementVelocity);
		velocity = _movementVelocity * delta;
		move_and_slide();
	}

	private function _pushMovementState(movementState:PlayerMovementState) {
		_movementStack.push(movementState);
	}

	private function _popMovementState() {
		_movementStack.pop();
	}

	private function _peekMovementState():PlayerMovementState {
		return _movementStack[_movementStack.length - 1];
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
