using Godot;
using SomeGame.Behaviors.HitStop;

namespace SomeGame.Behaviors.HealthSystem
{
    public partial class DamageHitStopPropagator : CollisionShape3D
    {
        // ================================
        // Export
        // ================================

        [Export] public HealthAndDamage _healthAndDamage;
        [Export] public HitStopBehavior _hitStopBehavior;

        // ================================
        // Public Functions
        // ================================

        public void TakeDamage(float damage) => _healthAndDamage.TakeDamage(damage);

        public void EnableHitStop(float duration) => _hitStopBehavior.EnableHitStop(duration);
    }
}