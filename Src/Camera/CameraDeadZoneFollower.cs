using Godot;

namespace SomeGame.Camera
{
    public partial class CameraDeadZoneFollower : Area3D
    {
        // ================================
        // Singleton
        // ================================

        private static CameraDeadZoneFollower _instance;
        public static CameraDeadZoneFollower Instance => _instance;

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

        [Export] private float _lerpRate;

        // Data
        private Node3D _target;
        private bool _isTargetOutside;

        // Lerp Data
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _lerpAmount;

        // ================================
        // Override Functions
        // ================================

        public override void _Ready()
        {
            PhantomCameraController.Instance.SetTarget(this);

            BodyEntered += _HandleBodyEntered;
            BodyExited += _HandleBodyExited;
        }

        public override void _ExitTree()
        {
            BodyEntered -= _HandleBodyEntered;
            BodyExited -= _HandleBodyExited;
        }

        public override void _Process(double delta)
        {
            if (_target == null || !_isTargetOutside || _lerpAmount > 1)
            {
                return;
            }

            var deltaTime = (float)delta;
            _lerpAmount += _lerpRate * deltaTime;

            var lerpedPosition = _startPosition.Lerp(_targetPosition, _lerpAmount);
            Position = lerpedPosition;

            var currentTargetPosition = _target.Position;
            if (!currentTargetPosition.IsEqualApprox(_targetPosition))
            {
                _startPosition = Position;
                _targetPosition = currentTargetPosition;
                _lerpAmount = 0;
            }
        }

        // ================================
        // Public Functions
        // ================================

        public void SetTarget(Node3D target)
        {
            _target = target;

            _startPosition = Position;
            _targetPosition = target.Position;
            _lerpAmount = 0;
        }

        // ================================
        // Private Functions
        // ================================

        private void _HandleBodyEntered(Node3D body)
        {
            if (body.GetInstanceId() == _target.GetInstanceId())
            {
                _isTargetOutside = false;
            }
        }

        private void _HandleBodyExited(Node3D body)
        {
            if (body.GetInstanceId() == _target.GetInstanceId())
            {
                _isTargetOutside = true;
            }
        }
    }
}