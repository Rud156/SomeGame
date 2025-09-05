using Godot;

namespace SomeGame.Camera
{
    public class CameraShaker
    {
        // ================================
        // Constants
        // ================================
        private const float CameraOriginThreshold = 0.05f;

        private readonly Camera3D _mainCamera;

        // Shake Data
        private float _strength;
        private float _decay;
        private float _magnitude;
        private float _force;
        private float _range;

        // Data
        private Vector3 _origin;
        private Vector2 _offset;
        private bool _isShaking;
        private bool _isAtOrigin;

        private RandomNumberGenerator _rng;

        // ================================
        // Public Functions
        // ================================

        public CameraShaker(Camera3D camera)
        {
            _strength = 1;
            _decay = 1;
            _magnitude = 1;
            _force = 0;
            _range = 1;

            _offset = new Vector2(1, 1);
            _isShaking = false;
            _isAtOrigin = true;

            _rng = new RandomNumberGenerator();
            _mainCamera = camera;
            _origin = _mainCamera.GlobalPosition;
        }

        public void Process(float delta)
        {
            if (_force > 0)
            {
                _force = Mathf.Max(_force - _decay * delta, 0);
                _range = Mathf.Max(_range - _decay * delta, 0);
                _Shake();
            }
            else
            {
                _isShaking = false;
            }

            if (!_isShaking && !_isAtOrigin)
            {
                _ReturnToOrigin(delta);
            }
        }

        public void StartShake(float decay, float magnitude)
        {
            _decay = decay;
            _magnitude = magnitude;

            _range = 1;
            _force = 1;
            _isAtOrigin = false;
            _isShaking = true;
        }

        // ================================
        // Private Functions
        // ================================

        private void _Shake()
        {
            if (!_isShaking)
            {
                return;
            }

            float amount = _force * _strength;
            float offsetX = _offset.X * amount * _rng.RandfRange(-_range, _range);
            float offsetY = _offset.Y * amount * _rng.RandfRange(-_range, _range);

            _mainCamera.Position = _origin + new Vector3(offsetX * _magnitude, offsetY * _magnitude, 0);
        }

        private void _ReturnToOrigin(float delta)
        {
            _mainCamera.Position = _mainCamera.Position.Lerp(_origin, delta * 2);
            if (_origin.DistanceTo(_mainCamera.Position) <= CameraOriginThreshold)
            {
                _isAtOrigin = true;
            }
        }
    }
}