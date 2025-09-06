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
        [Export] public float displayLerpSpeed;
        [Export] public Curve lerpCurve;

        [ExportGroup("Display Colors")]
        [Export] public Color lowHealthColor;
        [Export] public Color midHealthColor;
        [Export] public Color fullHealthColor;

        [ExportGroup("Bar Flash")]
        [Export] public Color flashColor;
        [Export] public int flashCount;
        [Export] public float flashOnDuration;
        [Export] public float flashOffDuration;

        [ExportGroup("Bar Scaler")]
        [Export] public float defaultScale;
        [Export] public float biggerScale;
        [Export] public int scaleCount;
        [Export] public float scaleChangeDuration;

        [ExportGroup("Components")]
        [Export] public TextureProgressBar progressBar;

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
            if (_lerpAmount > 1)
            {
                return;
            }

            var percent = lerpCurve.Sample(_lerpAmount);
            var mappedValue = Mathf.Lerp(_startHealth, _targetHealth, percent);
            progressBar.Value = mappedValue;
            _lerpAmount += delta * displayLerpSpeed;
        }

        private async void _HandleHealthChanged(float oldHealth, float newHealth, float maxHealth)
        {
            _startHealth = oldHealth;
            _targetHealth = newHealth;
            _lerpAmount = 0;

            var healthRatio = newHealth / maxHealth;
            var healthColor = healthRatio <= 0.5
                ? lowHealthColor.Lerp(midHealthColor, healthRatio * 2)
                : midHealthColor.Lerp(fullHealthColor, (healthRatio - 0.5f) * 2);

            if (!_flashCoroutineActive)
            {
                foreach (var delayAmount in BarFlasher(healthColor))
                {
                    var delay = Mathf.FloorToInt(delayAmount * 1000);
                    await Task.Delay(delay);
                }
            }

            if (!_scaleCoroutineActive)
            {
                foreach (var delayAmount in BarScaler())
                {
                    var delay = Mathf.FloorToInt(delayAmount * 1000);
                    await Task.Delay(delay);
                }
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