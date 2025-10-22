using Godot;
using SomeGame.Behaviors.Common;


namespace SomeGame.Enemies.Core
{
    public partial class EnemyController : DamageHitStopPropagator
    {
        // ================================
        // Export
        // ================================

        [ExportGroup("Detection")]
        [Export] private float _detectionRadius;
        [Export(PropertyHint.Layers3DPhysics)] private uint _detectionMask;

        [ExportGroup("Components")]
        [Export] private NavigationAgent3D _navAgent;
        [Export] private EnemyAnimationController _animationController;

        // ================================
        // Override Functions
        // ================================

        public override void _Ready()
        {
        }
    }

    // ================================
    // Enums
    // ================================

    public enum CoreEnemyState
    {
        Idle,
        Patrolling,
        Alerted,
        Chasing,
        Attacking,
        Fallback,
        Dead,
        CustomMovement
    }
}