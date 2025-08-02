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
	var maxJumpCount:Int;

	private var _gravity:Float;
    private var _movementVelocity:Vector3;

	// https://github.com/KenneyNL/Starter-Kit-3D-Platformer/blob/main/scripts/player.gd
	// ================================
	// Override Functions
	// ================================
	public override function _process(delta:Float):Void {}
}
