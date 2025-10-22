using System.Collections.Generic;
using Godot;
using Godot.Collections;
using SomeGame.Behaviors.Common;

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
        [Export(PropertyHint.Layers3DPhysics)] private uint _collisionMask;

        [ExportGroup("HitStop")]
        [Export] private float _hitStopDuration;

        // Data
        private bool _isActive;
        private Rid _sphereShape;
        private Array<Rid> _excludeObjects;
        private PhysicsShapeQueryParameters3D _queryParams;
        private float _currentTickDuration;

        // ================================
        // Override Functions
        // ================================

        public override void _Ready()
        {
            _currentTickDuration = 0;
            _sphereShape = PhysicsServer3D.SphereShapeCreate();
            PhysicsServer3D.ShapeSetData(_sphereShape, _damageRadius);
        }

        public override void _ExitTree()
        {
            PhysicsServer3D.FreeRid(_sphereShape);
        }

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
                var spaceState = GetWorld3D().DirectSpaceState;
                var result = spaceState.IntersectShape(_queryParams);

                foreach (var intersectResult in result)
                {
                    var collider = (CollisionObject3D)intersectResult.GetValueOrDefault("collider").AsGodotObject();
                    if (collider is DamageHitStopPropagator damageHitStopPropagator)
                    {
                        damageHitStopPropagator.TakeDamage(_damageAmount);
                        damageHitStopPropagator.EnableHitStop(_hitStopDuration);
                    }
                }

                _currentTickDuration = _damageTickRate;
            }
        }

        // ================================
        // Public Functions
        // ================================

        public void Enable(Vector3 position, Array<Rid> excludeObjects)
        {
            _isActive = true;
            _excludeObjects = excludeObjects;

            UpdatePosition(position);
        }

        public void Disable() => _isActive = false;

        public void UpdatePosition(Vector3 position)
        {
            var damageLocation = new Transform3D(Basis.Identity, position);
            _queryParams = new PhysicsShapeQueryParameters3D
            {
                ShapeRid = _sphereShape,
                Transform = damageLocation,
                Exclude = _excludeObjects,
                CollisionMask = _collisionMask,
                CollideWithAreas = true,
                CollideWithBodies = true
            };
        }
    }
}