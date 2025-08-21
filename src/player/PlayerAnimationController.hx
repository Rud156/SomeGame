package src.player;

import src.helpers.MathUtils;
import godot.*;
import src.player.PlayerController.PlayerController;
import src.player.PlayerController.PlayerMovementState;
import src.player.PlayerInputController;

class PlayerAnimationController extends Node3D {
	// ================================
	// Constants
	// ================================
	@:const
	private static final GROUNDED_ANIM_PARAM:String = "parameters/GroundedBSP/blend_position";
	@:const
	private static final JUMP_TRIGGER_MAIN:String = "parameters/Lowerbody/blend_amount";
	@:const
	private static final JUMP_TRIGGER_SECONDARY:String = "parameters/Lowerbody/blend_amount";
	@:const
	private static final JUMP_IS_FALLING:String = "parameters/JumpSM/conditions/is_falling";
	@:const
	private static final JUMP_IS_GROUNDED:String = "parameters/JumpSM/conditions/is_grounded";

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

	public override function _process(_):Void {
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
		var moveZ:Float = -PlayerInputController.instance.movementInput.x;
		var moveX:Float = PlayerInputController.instance.movementInput.y;

		var yRotation:Float = playerController.global_rotation.y;

		var vAxisVMovement:Float = moveZ * Godot.cos(yRotation);
		var vAxisHMovement:Float = moveZ * Godot.sin(yRotation);

		var hAxisVMovement:Float = moveX * Godot.sin(yRotation);
		var hAxisHMovement:Float = moveX * Godot.cos(yRotation);

		var vMovement:Float = vAxisVMovement + hAxisVMovement;
		var hMovement:Float = vAxisHMovement + hAxisHMovement;

		animationController.set(GROUNDED_ANIM_PARAM, new Vector2(hMovement, vMovement));
	}

	private function _handleOnJumpTriggered():Void {
		animationController.set(JUMP_TRIGGER_MAIN, 1);
		animationController.set(JUMP_TRIGGER_SECONDARY, true);
		animationController.set(JUMP_IS_GROUNDED, false);
	}

	private function _handleOnPlayerStateChanged(currentState:PlayerMovementState):Void {
		switch (currentState) {
			case PlayerMovementState.NORMAL:
				_resetJumpAnimation();

			case PlayerMovementState.CUSTOM_MOVEMENT:
				_resetJumpAnimation();

			case PlayerMovementState.FALLING:
				animationController.set(JUMP_IS_FALLING, true);
		}
	}

	private function _resetJumpAnimation():Void {
		animationController.set(JUMP_TRIGGER_MAIN, 0);
		animationController.set(JUMP_TRIGGER_SECONDARY, false);
		animationController.set(JUMP_IS_FALLING, false);
		animationController.set(JUMP_IS_GROUNDED, true);
	}
}
