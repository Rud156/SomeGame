using Godot;
using SomeGame.Behaviors.HitStop;

namespace SomeGame.Behaviors.HealthSystem
{
    public partial class DamageHitStopPropagator : CharacterBody3D
    {
        // ================================
        // Export
        // ================================

        [Export] private HealthAndDamage _healthAndDamage;
        [Export] private HitStopBehavior _hitStopBehavior;

        // ================================
        // Properties
        // ================================

        public HealthAndDamage HealthAndDamage => _healthAndDamage;
        public HitStopBehavior HitStopBehavior => _hitStopBehavior;

        // ================================
        // Public Functions
        // ================================

        public void TakeDamage(float damage) => _healthAndDamage.TakeDamage(damage);

        public void EnableHitStop(float duration) => _hitStopBehavior.EnableHitStop(duration);
    }
}