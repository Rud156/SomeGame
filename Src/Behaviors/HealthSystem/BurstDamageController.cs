using Godot;
using Godot.Collections;

namespace SomeGame.Behaviors.HealthSystem
{
    public partial class BurstDamageController : Node3D
    {
        [Export] private Array<Node3D> _burstDamageNodes;

        // ================================
        // Public Functions
        // ================================

        public void ApplyDamage(Array<Rid> excludeObjects) 
        {
            foreach (var burstDamageNode in _burstDamageNodes)
            {
                var burstDamage = (IBurstDamage)burstDamageNode;
                burstDamage.ApplyDamage(excludeObjects);
            }
        }

        public void ApplyDamage(Vector3 position, Array<Rid> excludeObjects)
        {
            foreach (var burstDamageNode in _burstDamageNodes)
            {
                var burstDamage = (IBurstDamage)burstDamageNode;
                burstDamage.ApplyDamage(position, excludeObjects);
            }
        }
    }
}