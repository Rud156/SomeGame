using Godot;

namespace SomeGame.Behaviors.HitStop
{
    public partial class HitStopAnimator : Node3D
    {
        // ================================
        // Constants
        // ================================
        private const string AnimationSpeed = "parameters/AnimatorSpeed/scale";

        // ================================
        // Export
        // ================================
        [Export] private HitStopBehavior _hitStopBehavior;
        [Export] private AnimationTree _animationTree;

        // Data
        private float _hsAnimSpeed;

        // ================================
        // Override Functions
        // ================================

        public override void _Ready()
        {
            _hitStopBehavior.OnHitStopStateChanged += _HandleHitStopBehaviorChanged;
        }

        public override void _ExitTree()
        {
            _hitStopBehavior.OnHitStopStateChanged -= _HandleHitStopBehaviorChanged;
        }

        // ================================
        // Private Functions
        // ================================

        private void _HandleHitStopBehaviorChanged(bool active)
        {
            if (active)
            {
                _hsAnimSpeed = (float)_animationTree.Get(AnimationSpeed);
                _animationTree.Set(AnimationSpeed, 0);
            }
            else
            {
                _animationTree.Set(AnimationSpeed, _hsAnimSpeed);
            }
        }
    }
}