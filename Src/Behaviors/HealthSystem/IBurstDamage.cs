using Godot;
using Godot.Collections;

namespace SomeGame.Behaviors.HealthSystem
{
    public interface IBurstDamage
    {
        void ApplyDamage(Vector3 position, Array<Rid> excludeObjects);
    }
}