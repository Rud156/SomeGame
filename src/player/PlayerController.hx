package src.player;

import src.player.PlayerInputController;
import godot.*;

class PlayerController extends CharacterBody3D {
	// ================================
	// Export
	// ================================
	@:export
	var movementSpeed:Float;
	@:export
	var airMovementSpeed:Float;
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

	// https://github.com/KenneyNL/Starter-Kit-3D-Platformer/blob/main/scripts/player.gd
	// ================================
	// Override Functions
	// ================================

	public override function _ready():Void {
		_movementVelocity = Vector3.ZERO;
		_pushMovementState(PlayerMovementState.NORMAL);
	}

	public override function _process(delta:Float):Void {
		_handleMovement(delta);
		_processGravity(delta);

		_handleJumpPressed();
		_applyMovement(delta);
		move_and_slide();
	}

	// ================================
	// Private Functions
	// ================================

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
		// TODO: Look up Forward and Right Vector...

		var playerInput:Vector2 = PlayerInputController.instance.movementInput;
		var mappedMovement:Vector2 = playerInput * movementSpeed;
		_movementVelocity.x = mappedMovement.x;
		_movementVelocity.z = mappedMovement.y;
	}

	private function _updateFallingState(delta:Float):Void {}

	private function _updateCustomMovement(delta:Float):Void {}

	private function _processGravity(delta:Float):Void {
		if (!is_on_floor()) {
			var yVelocity:Float = _movementVelocity.y;
			yVelocity += GRAVITY * gravityMultiplier;
			_movementVelocity.y = yVelocity;
		} else {
			_movementVelocity.y = GRAVITY * gravityMultiplier;
		}
	}

	private function _handleJumpPressed() {
		if (PlayerInputController.instance.jumpPressed) {
			_movementVelocity.y = jumpVelocity;
		}
	}

	private function _applyMovement(delta:Float):Void {
		velocity = _movementVelocity * delta;
	}

	private function _pushMovementState(movementState:PlayerMovementState) {
		_movementStack.push(movementState);
	}

	private function _popMovementState(movementState:PlayerMovementState) {
		_movementStack.pop();
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
