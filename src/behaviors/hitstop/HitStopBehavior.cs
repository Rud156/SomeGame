using Godot;

namespace SomeGame.Behaviors.HitStop
{
    public partial class HitStopBehavior : Node3D
    {
        // ================================
        // Export
        // ================================
        [Signal]
        public delegate void OnHitStopStateChangedEventHandler(bool active);

        // Data
        private float _duration;

        // ================================
        // Properties
        // ================================
        public bool IsActive => _duration > 0;

        // ================================
        // Override Functions
        // ================================

        public override void _Process(double delta)
        {
            var deltaTime = (float)delta;

            if (!IsActive)
            {
                return;
            }

            _duration -= deltaTime;
            if (_duration <= 0)
            {
                DisableHitStop();
            }
        }

        // ================================
        // Public Functions
        // ================================

        public void EnableHitStop(float duration)
        {
            _duration = duration;
            EmitSignal(SignalName.OnHitStopStateChanged, true);
        }

        public void DisableHitStop()
        {
            _duration = 0;
            EmitSignal(SignalName.OnHitStopStateChanged, false);
        }
    }
}