using Godot;

namespace SomeGame.Camera
{
    public partial class CameraController : Node3D
    {
        private static CameraController _instance;
        public static CameraController Instance => _instance;

        // ================================
        // Export
        // ================================
        [Export] public Vector3 followDistance;
        [Export] public float followSpeed;
        [Export] public Curve followLerpCurve;
        [Export] public float lookRotationSpeed;

        // Data
        private Node3D _target;
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _lerpAmount;
        private CameraShaker _cameraShaker;

        // ================================
        // Override Functions
        // ================================

        public override void _EnterTree()
        {
            if (_instance != null)
            {
                QueueFree();
                return;
            }

            _instance = this;
        }

        public override void _Ready()
        {
            base._Ready();
            _instance = this;
            var camera = GetViewport().GetCamera3D();
            _cameraShaker = new CameraShaker(camera);

            _startPosition = GlobalPosition;
            _targetPosition = Vector3.Zero;
            _lerpAmount = 0;
        }

        public override void _Process(double delta)
        {
            var deltaTime = (float)delta;

            _cameraShaker.Process(deltaTime);
            _LookAtTargetPosition(deltaTime);
            _UpdateLastTargetPosition();
            _UpdateCameraFollow(deltaTime);
        }

        // ================================
        // Public Functions
        // ================================

        public void SetTargetObject(Node3D target)
        {
            _target = target;
        }

        public void ClearTarget()
        {
            _target = null;
        }

        public void StartShake(float decay, float magnitude)
        {
            _cameraShaker.StartShake(decay, magnitude);
        }

        // ================================
        // Private Functions
        // ================================

        private void _LookAtTargetPosition(float delta)
        {
            var targetPosition = _target != null ? _target.GlobalPosition : _targetPosition;
            var cameraTransform = GlobalTransform;
            var lookAtTransform = cameraTransform.LookingAt(targetPosition, Vector3.Up);

            var x = cameraTransform.Basis.X.Lerp(lookAtTransform.Basis.X, lookRotationSpeed * delta);
            var y = cameraTransform.Basis.Y.Lerp(lookAtTransform.Basis.Y, lookRotationSpeed * delta);
            var z = cameraTransform.Basis.Z.Lerp(lookAtTransform.Basis.Z, lookRotationSpeed * delta);
            GlobalBasis = new Basis(x, y, z);
        }

        private void _UpdateCameraFollow(float delta)
        {
            if (_lerpAmount >= 1)
            {
                return;
            }

            _lerpAmount += followSpeed * delta;

            var lerpValue = followLerpCurve.Sample(_lerpAmount);

            var mappedPosition = _startPosition.Lerp(_targetPosition, lerpValue);
            Position = mappedPosition;
        }

        private void _UpdateLastTargetPosition()
        {
            if (_target != null)
            {
                var targetPosition = _target.GlobalPosition;
                targetPosition += followDistance;

                if (!targetPosition.IsEqualApprox(_targetPosition))
                {
                    _startPosition = GlobalPosition;
                    _targetPosition = targetPosition;
                    _lerpAmount = 0;
                }
            }
        }
    }
}