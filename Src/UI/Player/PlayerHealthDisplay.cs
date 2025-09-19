using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using SomeGame.Behaviors.HealthSystem;

namespace SomeGame.UI.Player
{
    public partial class PlayerHealthDisplay : Control
    {
        private static PlayerHealthDisplay _instance;
        public static PlayerHealthDisplay Instance => _instance;

        // ================================
        // Export
        // ================================

        [ExportGroup("Lerp Data")]
        [Export] private float _displayLerpSpeed;
        [Export] private Curve _lerpCurve;

        [ExportGroup("Display Colors")]
        [Export] private Color _lowHealthColor;
        [Export] private Color _midHealthColor;
        [Export] private Color _fullHealthColor;

        [ExportGroup("Bar Flash")]
        [Export] private Color _flashColor;
        [Export] private int _flashCount;
        [Export] private float _flashOnDuration;
        [Export] private float _flashOffDuration;

        [ExportGroup("Bar Scaler")]
        [Export] private float _defaultScale;
        [Export] private float _biggerScale;
        [Export] private int _scaleCount;
        [Export] private float _scaleChangeDuration;

        [ExportGroup("Components")]
        [Export] private TextureProgressBar _progressBar;

        // Data
        private HealthAndDamage _healthAndDamage;

        private float _startHealth;
        private float _targetHealth;
        private float _lerpAmount;

        private bool _flashCoroutineActive;
        private bool _scaleCoroutineActive;

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

        public override void _Ready() => _lerpAmount = 1;

        public override void _ExitTree()
        {
            if (_healthAndDamage != null)
            {
                _healthAndDamage.OnHealthChanged -= _HandleHealthChanged;
            }
        }

        public override void _Process(double delta) => _UpdateHealthDisplay((float)delta);

        // ================================
        // Public Functions
        // ================================

        public void RegisterHealthAndDamage(HealthAndDamage healthAndDamage)
        {
            if (_healthAndDamage != null)
            {
                _healthAndDamage.OnHealthChanged -= _HandleHealthChanged;
            }

            _healthAndDamage = healthAndDamage;
            _healthAndDamage.OnHealthChanged += _HandleHealthChanged;

            _progressBar.MinValue = 0;
            _progressBar.MaxValue = _healthAndDamage.MaxHealth;
            _progressBar.Value = _healthAndDamage.CurrentHealth;
        }

        // ================================
        // Private Functions
        // ================================

        private void _UpdateHealthDisplay(float delta)
        {
            if (_lerpAmount >= 1)
            {
                return;
            }

            var percent = _lerpCurve.Sample(_lerpAmount);
            var mappedValue = Mathf.Lerp(_startHealth, _targetHealth, percent);
            _progressBar.Value = mappedValue;
            _lerpAmount += delta * _displayLerpSpeed;
        }

        private void _HandleHealthChanged(float oldHealth, float newHealth, float maxHealth)
        {
            _startHealth = oldHealth;
            _targetHealth = newHealth;
            _lerpAmount = 0;

            var healthRatio = newHealth / maxHealth;
            var healthColor = healthRatio <= 0.5
                ? _lowHealthColor.Lerp(_midHealthColor, healthRatio * 2)
                : _midHealthColor.Lerp(_fullHealthColor, (healthRatio - 0.5f) * 2);

            _StartBarFlasher(healthColor);
            _StartBarScaler();
        }

        private async void _StartBarFlasher(Color healthColor)
        {
            if (_flashCoroutineActive)
                return;

            foreach (var delayAmount in BarFlasher(healthColor))
            {
                var delay = Mathf.FloorToInt(delayAmount * 1000);
                await Task.Delay(delay);
            }
        }

        private async void _StartBarScaler()
        {
            if (_scaleCoroutineActive)
                return;

            foreach (var delayAmount in BarScaler())
            {
                var delay = Mathf.FloorToInt(delayAmount * 1000);
                await Task.Delay(delay);
            }
        }

        // ================================
        // Private Functions
        // ================================

        private IEnumerable<float> BarFlasher(Color finalColor)
        {
            _flashCoroutineActive = true;
            var startColor = _progressBar.TintProgress;
            for (var i = 0; i < _flashCount; i++)
            {
                _progressBar.TintProgress = _flashColor;
                yield return _flashOnDuration;
                _progressBar.TintProgress = startColor;
                yield return _flashOffDuration;
            }

            _progressBar.TintProgress = finalColor;
            _flashCoroutineActive = false;
        }

        private IEnumerable<float> BarScaler()
        {
            _scaleCoroutineActive = true;
            for (var i = 0; i < _scaleCount; i++)
            {
                Scale = Vector2.One * _biggerScale;
                yield return _scaleChangeDuration;
                Scale = Vector2.One * _defaultScale;
                yield return _scaleChangeDuration;
            }

            _scaleCoroutineActive = false;
        }
    }
}