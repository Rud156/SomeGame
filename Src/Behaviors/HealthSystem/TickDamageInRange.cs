using System.Collections.Generic;
using Godot;
using NoDontDoIt.Godot.Raycasting;

namespace SomeGame.Behaviors.HealthSystem
{
    public partial class TickDamageInRange : Node3D
    {
        // ================================
        // Export
        // ================================

        [ExportGroup("Damage")]
        [Export] private float _damageRadius;
        [Export] private float _damageAmount;
        [Export] private float _damageTickRate;

        [ExportGroup("HitStop")]
        [Export] private float _hitStopDuration;

        // Data
        private bool _isActive;
        private Transform3D _damageLocation;
        private IReadOnlyCollection<CollisionObject3D> _excludeObjects;
        private float _currentTickDuration;

        // ================================
        // Override Functions
        // ================================

        public override void _Ready() => _currentTickDuration = 0;

        public override void _Process(double delta)
        {
            if (!_isActive)
            {
                return;
            }

            var deltaTime = (float)delta;
            _currentTickDuration -= deltaTime;

            if (_currentTickDuration <= 0)
            {
                var hitColliders = GetWorld3D().DirectSpaceState.OverlapSphere(
                    _damageRadius,
                    _damageLocation,
                    Vector3.Zero,
                    exclude: _excludeObjects
                );

                foreach (var hitCollider in hitColliders)
                {
                    var damageHitStopPropagator = (DamageHitStopPropagator)hitCollider.Shape;
                    damageHitStopPropagator.TakeDamage(_damageAmount);
                    damageHitStopPropagator.EnableHitStop(_hitStopDuration);
                }

                _currentTickDuration = _damageTickRate;
            }
        }

        // ================================
        // Public Functions
        // ================================

        public void Enable(Vector3 position, IReadOnlyCollection<CollisionObject3D> excludeObjects)
        {
            _isActive = true;
            _excludeObjects = excludeObjects;

            UpdatePosition(position);
        }

        public void Disable() => _isActive = false;

        public void UpdatePosition(Vector3 position) => _damageLocation = new Transform3D(Basis.Identity, position);
    }
}