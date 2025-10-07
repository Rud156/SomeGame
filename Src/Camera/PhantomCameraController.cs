using Godot;
using PhantomCamera.Noise;

namespace SomeGame.Camera
{
    public partial class PhantomCameraController : Node3D
    {
        // ================================
        // Singleton
        // ================================

        private static PhantomCameraController _instance;
        public static PhantomCameraController Instance => _instance;

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
        // Export
        // ================================

        [Export] private Node3D _phantomCamera;
        [Export] private Node3D _phantomCameraShaker;

        // ================================
        // Public Functions
        // ================================

        public void SetTargetObject(Node3D target)
        {
            _phantomCamera.Call("set_follow_target", target);
            _phantomCamera.Call("set_look_at_target", target);
        }

        public void StartCameraShake(PhantomCameraNoise3D noiseAsset, float duration)
        {
            _phantomCameraShaker.Call("set_noise", noiseAsset.Resource);
            _phantomCameraShaker.Call("set_duration", duration);
            _phantomCameraShaker.Call("emit");
        }
    }
}