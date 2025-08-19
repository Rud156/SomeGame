package src.player;

import src.helpers.MathUtils;
import godot.*;
import src.player.PlayerController.PlayerController;
import src.player.PlayerController.PlayerMovementState;
import src.player.PlayerInputController;

class PlayerAnimationController extends Node3D {
	// ================================
	// Export
	// ================================
	@:export
	var playerController:PlayerController;
	@:export
	var animationController:AnimationTree;

	// Events
	private var _handleOnJumpTriggeredCallable:Callable;
	private var _handleOnPlayerStateChangedCallable:Callable;

	// ================================
	// Override Functions
	// ================================

	public override function _ready():Void {
		_handleOnJumpTriggeredCallable = Callable.create(this, "_handleOnJumpTriggered");
		playerController.connect(PlayerController.ON_JUMP_TRIGGERED, _handleOnJumpTriggeredCallable);

		_handleOnPlayerStateChangedCallable = Callable.create(this, "_handleOnPlayerStateChanged");
		playerController.connect(PlayerController.ON_PLAYER_STATE_CHANGED, _handleOnPlayerStateChangedCallable);
	}

	public override function _exit_tree():Void {
		playerController.disconnect(PlayerController.ON_JUMP_TRIGGERED, _handleOnJumpTriggeredCallable);
		playerController.disconnect(PlayerController.ON_PLAYER_STATE_CHANGED, _handleOnPlayerStateChangedCallable);
	}

	public override function _process(delta:Float):Void {
		// Handle Grounded Motion...
		switch (playerController.peekMovementState()) {
			case PlayerMovementState.NORMAL:
				_setGroundedAnimation();

			case PlayerMovementState.CUSTOM_MOVEMENT:
			case PlayerMovementState.FALLING:
		}
	}

	// ================================
	// Private Functions
	// ================================

	private function _setGroundedAnimation():Void {
		var moveZ:Float = PlayerInputController.instance.movementInput.y;
		var moveX:Float = PlayerInputController.instance.movementInput.x;

		var yRotation:Float = MathUtils.To360Angle(playerController.rotation.y);

		var vAxisVMovement:Float = moveZ * Godot.cos(Godot.deg_to_rad(yRotation));
		var vAxisHMovement:Float = moveZ * Godot.sin(Godot.deg_to_rad(yRotation));

		var hAxisVMovement:Float = moveX * Godot.sin(Godot.deg_to_rad(yRotation));
		var hAxisHMovement:Float = moveX * Godot.cos(Godot.deg_to_rad(yRotation));

		var vMovement:Float = vAxisVMovement + hAxisVMovement;
		var hMovement:Float = vAxisHMovement + hAxisHMovement;

		animationController.set("parameters/GroundedBSP/blend_position", new Vector2(hMovement, vMovement));
	}

	private function _handleOnJumpTriggered():Void {}

	private function _handleOnPlayerStateChanged(currentState:PlayerMovementState):Void {
		switch (currentState) {
			case PlayerMovementState.NORMAL:
			case PlayerMovementState.FALLING:

			case PlayerMovementState.CUSTOM_MOVEMENT:
		}
	}
}
