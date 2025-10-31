using Godot;
using SomeGame.Camera;

namespace SomeGame.Global
{
    public partial class GameManager : Node3D
    {
        // ================================
        // Singleton 
        // ================================

        private static GameManager _instance;
        public static GameManager Instance => _instance;

        public override void _EnterTree()
        {
            if (_instance != null)
            {
                QueueFree();
                return;
            }

            _instance = this;
        }

        // ================================
        // Public Functions
        // ================================

        public void RegisterMainTarget(Node3D mainTarget) => CameraDeadZoneFollower.Instance.SetTarget(mainTarget);
    }
}