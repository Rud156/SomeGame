using Godot;

namespace SomeGame.Behaviors.HealthSystem
{
    public partial class AoeBurstDamage : Node3D
    {
        // ================================
        // Export
        // ================================

        [Export] private CollisionShape3D _collisionShape;

        // ================================
        // Public Functions
        // ================================

        public void ApplyDamage()
        {
        }
    }
}