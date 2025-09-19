using System.Collections.Generic;
using Godot;

namespace SomeGame.Behaviors.HitStop
{
    public partial class HitStopParticleSystem : Node3D
    {
        // ================================
        // Export
        // ================================
        [Export] private HitStopBehavior _hitStopBehavior;

        // Data
        private List<GpuParticles3D> _activeParticleSystems;

        // ================================
        // Override Functions
        // ================================

        public override void _Ready()
        {
            _activeParticleSystems = [];
            _hitStopBehavior.OnHitStopStateChanged += _HandleHitStopBehaviorChanged;
        }

        public override void _ExitTree()
        {
            _hitStopBehavior.OnHitStopStateChanged -= _HandleHitStopBehaviorChanged;
        }

        public override void _Process(double delta)
        {
            _CheckAndRemoveInactiveParticles();
        }

        // ================================
        // Public Functions
        // ================================

        public void AddParticleSystem(GpuParticles3D particleSystem)
        {
            _activeParticleSystems.Add(particleSystem);
        }

        // ================================
        // Private Functions
        // ================================

        private void _CheckAndRemoveInactiveParticles()
        {
            // TODO: Check if this works...
            for (int index = _activeParticleSystems.Count - 1; index >= 0; index--)
            {
                if (_activeParticleSystems[index] == null)
                {
                    _activeParticleSystems.RemoveAt(index);
                }
            }
        }

        private void _HandleHitStopBehaviorChanged(bool active)
        {
            // TODO: Check if this actually works
            // Also might need to save particles prior speed before setting it back/setting it to 0

            foreach (var particleSystem in _activeParticleSystems)
            {
                if (active)
                    particleSystem.SetSpeedScale(0);
                else
                    particleSystem.SetSpeedScale(1);
            }
        }
    }
}