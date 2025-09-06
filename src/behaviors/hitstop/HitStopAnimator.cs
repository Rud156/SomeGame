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
        [Export] public HitStopBehavior hitStopBehavior;
        [Export] public AnimationTree animationTree;

        // Data
        private float _hsAnimSpeed;

        // ================================
        // Override Functions
        // ================================

        public override void _Ready()
        {
            hitStopBehavior.OnHitStopStateChanged += _HandleHitStopBehaviorChanged;
        }

        public override void _ExitTree()
        {
            hitStopBehavior.OnHitStopStateChanged -= _HandleHitStopBehaviorChanged;
        }

        // ================================
        // Private Functions
        // ================================

        private void _HandleHitStopBehaviorChanged(bool active)
        {
            if (active)
            {
                _hsAnimSpeed = (float)animationTree.Get(AnimationSpeed);
                animationTree.Set(AnimationSpeed, 0);
            }
            else
            {
                animationTree.Set(AnimationSpeed, _hsAnimSpeed);
            }
        }
    }
}