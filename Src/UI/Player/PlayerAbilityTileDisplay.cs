using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace SomeGame.UI.Player
{
    public partial class PlayerAbilityTileDisplay : Control
    {
        // ================================
        // Export
        // ================================

        [ExportGroup("Components")]
        [Export] private TextureRect _abilityIcon;
        [Export] private TextureRect _abilityFlasher;
        [Export] private Label _abilityTimer;
        [Export] private TextureProgressBar _abilityProgressBar;
        [Export] private TextureRect _abilityKey;

        [ExportGroup("Ability Flasher")]
        [Export] private int _flashCount;
        [Export] private float _flashOnDuration;
        [Export] private float _flashOffDuration;

        [ExportGroup("Ability Scaler")]
        [Export] private int _scaleCount;
        [Export] private float _defaultScale;
        [Export] private float _biggerScale;
        [Export] private float _scaleChangeDuration;

        // Data
        private bool _flasherActive;
        private bool _scalerActive;

        // ================================
        // Public Functions
        // ================================

        public void SetAbilityIcon(Texture2D icon) => _abilityIcon.Texture = icon;
        
        public void SetAbilityKeyIcon(Texture2D keyIcon) => _abilityKey.Texture = keyIcon;

        public void SetAbilityProgress(float time, float progress, float minProgress, float maxProgress)
        {
            _abilityTimer.Text = time.ToString("0.00");
            _abilityProgressBar.Value = progress;
            _abilityProgressBar.MinValue = minProgress;
            _abilityProgressBar.MaxValue = maxProgress;
        }

        public void TriggerAbilityFx()
        {
            _StartAbilityFlasher();
            _StartAbilityScaler();
        }

        // ================================
        // Private Functions
        // ================================

        private async void _StartAbilityFlasher()
        {
            if (_flasherActive)
                return;

            foreach (var delayAmount in FlashCoroutine())
            {
                var delay = Mathf.FloorToInt(delayAmount * 1000);
                await Task.Delay(delay);
            }
        }

        private async void _StartAbilityScaler()
        {
            if (_scalerActive)
                return;

            foreach (var delayAmount in ScaleCoroutine())
            {
                var delay = Mathf.FloorToInt(delayAmount * 1000);
                await Task.Delay(delay);
            }
        }

        private IEnumerable<float> FlashCoroutine()
        {
            _flasherActive = true;
            for (var i = 0; i < _flashCount; i++)
            {
                _abilityFlasher.Visible = true;
                yield return _flashOnDuration;
                _abilityFlasher.Visible = false;
                yield return _flashOffDuration;
            }

            _flasherActive = false;
        }

        private IEnumerable<float> ScaleCoroutine()
        {
            _scalerActive = true;
            for (var i = 0; i < _scaleCount; i++)
            {
                Scale = Vector2.One * _biggerScale;
                yield return _scaleChangeDuration;
                Scale = Vector2.One * _defaultScale;
                yield return _scaleChangeDuration;
            }

            _scalerActive = false;
        }
    }
}