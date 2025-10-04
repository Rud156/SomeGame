using System.Collections.Generic;
using Godot;
using NoDontDoIt.Godot.Raycasting;

namespace SomeGame.Behaviors.HealthSystem
{
    public partial class BurstDamageInRange : Node3D
    {
        // ================================
        // Export
        // ================================

        [ExportGroup("Damage")]
        [Export] private float _damageRadius;
        [Export] private float _damageAmount;

        [ExportGroup("HitStop")]
        [Export] private float _hitStopDuration;

        // ================================
        // Public Functions
        // ================================

        public void ApplyDamage(Vector3 position, IReadOnlyCollection<CollisionObject3D> excludeObjects)
        {
            var damageLocation = new Transform3D(Basis.Identity, position);
            var hitColliders = GetWorld3D().DirectSpaceState.OverlapSphere(
                _damageRadius,
                damageLocation,
                Vector3.Zero,
                exclude: excludeObjects
            );

            foreach (var hitCollider in hitColliders)
            {
                var damageHitStopPropagator = (DamageHitStopPropagator)hitCollider.Shape;
                damageHitStopPropagator.TakeDamage(_damageAmount);
                damageHitStopPropagator.EnableHitStop(_hitStopDuration);
            }
        }
    }
}