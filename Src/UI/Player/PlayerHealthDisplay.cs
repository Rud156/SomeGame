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
        [Export] private float displayLerpSpeed;
        [Export] private Curve lerpCurve;

        [ExportGroup("Display Colors")]
        [Export] private Color lowHealthColor;
        [Export] private Color midHealthColor;
        [Export] private Color fullHealthColor;

        [ExportGroup("Bar Flash")]
        [Export] private Color flashColor;
        [Export] private int flashCount;
        [Export] private float flashOnDuration;
        [Export] private float flashOffDuration;

        [ExportGroup("Bar Scaler")]
        [Export] private float defaultScale;
        [Export] private float biggerScale;
        [Export] private int scaleCount;
        [Export] private float scaleChangeDuration;

        [ExportGroup("Components")]
        [Export] private TextureProgressBar progressBar;

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

            progressBar.MinValue = 0;
            progressBar.MaxValue = _healthAndDamage.MaxHealth;
            progressBar.Value = _healthAndDamage.CurrentHealth;
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

            var percent = lerpCurve.Sample(_lerpAmount);
            var mappedValue = Mathf.Lerp(_startHealth, _targetHealth, percent);
            progressBar.Value = mappedValue;
            _lerpAmount += delta * displayLerpSpeed;
        }

        private void _HandleHealthChanged(float oldHealth, float newHealth, float maxHealth)
        {
            _startHealth = oldHealth;
            _targetHealth = newHealth;
            _lerpAmount = 0;

            var healthRatio = newHealth / maxHealth;
            var healthColor = healthRatio <= 0.5
                ? lowHealthColor.Lerp(midHealthColor, healthRatio * 2)
                : midHealthColor.Lerp(fullHealthColor, (healthRatio - 0.5f) * 2);

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
            var startColor = progressBar.TintProgress;
            for (var i = 0; i < flashCount; i++)
            {
                progressBar.TintProgress = flashColor;
                yield return flashOnDuration;
                progressBar.TintProgress = startColor;
                yield return flashOffDuration;
            }

            progressBar.TintProgress = finalColor;
            _flashCoroutineActive = false;
        }

        private IEnumerable<float> BarScaler()
        {
            _scaleCoroutineActive = true;
            for (var i = 0; i < scaleCount; i++)
            {
                Scale = Vector2.One * biggerScale;
                yield return scaleChangeDuration;
                Scale = Vector2.One * defaultScale;
                yield return scaleChangeDuration;
            }

            _scaleCoroutineActive = false;
        }
    }
}