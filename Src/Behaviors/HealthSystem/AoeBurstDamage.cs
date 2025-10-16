using System.Collections.Generic;
using Godot;
using Godot.Collections;
using SomeGame.Behaviors.Common;

namespace SomeGame.Behaviors.HealthSystem
{
    public partial class AoeBurstDamage : Node3D, IBurstDamage
    {
        // ================================
        // Export
        // ================================

        [ExportGroup("Damage")]
        [Export] private float _damageAmount;
        [Export] private Shape3D _collisionShape;

        [ExportGroup("HitStop")]
        [Export] private float _hitStopDuration;

        // ================================
        // Public Functions
        // ================================

        public void ApplyDamage(Array<Rid> excludeObjects)
        {
            var position = GlobalPosition;
            ApplyDamage(position, excludeObjects);
        }

        public void ApplyDamage(Vector3 position, Array<Rid> excludeObjects)
        {
            var damageLocation = new Transform3D(Basis.Identity, position);
            var queryParams = new PhysicsShapeQueryParameters3D
            {
                Shape = _collisionShape,
                Transform = damageLocation,
                Exclude = excludeObjects,
                CollideWithAreas = true,
                CollideWithBodies = true
            };

            var world = GetWorld3D();
            var spaceState = GetWorld3D().DirectSpaceState;
            var result = spaceState.IntersectShape(queryParams);

            foreach (var intersectResult in result)
            {
                var collider = (CollisionObject3D)intersectResult.GetValueOrDefault("collider").AsGodotObject();
                if (collider is DamageHitStopPropagator damageHitStopPropagator)
                {
                    damageHitStopPropagator.TakeDamage(_damageAmount);
                    damageHitStopPropagator.EnableHitStop(_hitStopDuration);
                }
            }
        }
    }
}