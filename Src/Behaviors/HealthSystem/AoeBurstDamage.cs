using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace SomeGame.Behaviors.HealthSystem
{
    public partial class AoeBurstDamage : Area3D, IBurstDamage
    {
        // ================================
        // Export
        // ================================

        [ExportGroup("Damage")]
        [Export] private float _damageAmount;

        [ExportGroup("HitStop")]
        [Export] private float _hitStopDuration;

        // ================================
        // Public Functions
        // ================================

        public void ApplyDamage(Vector3 position, Array<Rid> excludeObjects)
        {
            var damageLocation = new Transform3D(Basis.Identity, position);
            var areaShape = GetRid();

            // TODO: Fix this...
            var queryParams = new PhysicsShapeQueryParameters3D
            {
                ShapeRid = areaShape,
                Transform = damageLocation,
                Exclude = excludeObjects,
                CollideWithAreas = true,
                CollideWithBodies = true
            };

            var world = GetWorld3D();
            var spaceState = GetWorld3D().DirectSpaceState;
            var result = spaceState.IntersectShape(queryParams);
            PhysicsServer3D.FreeRid(areaShape);

            foreach (var intersectResult in result)
            {
                var collider = (CollisionObject3D)intersectResult.GetValueOrDefault("collider").AsGodotObject();
                if (collider is DamageHitStopPropagator damageHitStopPropagator)
                {
                    damageHitStopPropagator.TakeDamage(_damageAmount);
                    // damageHitStopPropagator.EnableHitStop(_hitStopDuration);
                }
            }
        }
    }
}