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
        [Export] private TextureRect abilityIcon;
        [Export] private TextureRect abilityFlasher;
        [Export] private Label abilityTimer;
        [Export] private TextureProgressBar abilityProgressBar;
        [Export] private Label abilityName;

        [ExportGroup("Ability Flasher")]
        [Export] private int flashCount;
        [Export] private float flashOnDuration;
        [Export] private float flashOffDuration;

        [ExportGroup("Ability Scaler")]
        [Export] private int scaleCount;
        [Export] private float defaultScale;
        [Export] private float biggerScale;
        [Export] private float scaleChangeDuration;

        // Data
        private bool _flasherActive;
        private bool _scalerActive;

        // ================================
        // Public Functions
        // ================================

        public void SetAbilityNameAndIcon(string name, Texture2D icon)
        {
            abilityName.Text = name;
            abilityIcon.Texture = icon;
        }

        public void SetAbilityProgress(float time, float progress, float minProgress, float maxProgress)
        {
            abilityTimer.Text = time.ToString("0.00");
            abilityProgressBar.Value = progress;
            abilityProgressBar.MinValue = minProgress;
            abilityProgressBar.MaxValue = maxProgress;
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
            for (var i = 0; i < flashCount; i++)
            {
                abilityFlasher.Visible = true;
                yield return flashOnDuration;
                abilityFlasher.Visible = false;
                yield return flashOffDuration;
            }

            _flasherActive = false;
        }

        private IEnumerable<float> ScaleCoroutine()
        {
            _scalerActive = true;
            for (var i = 0; i < scaleCount; i++)
            {
                Scale = Vector2.One * biggerScale;
                yield return scaleChangeDuration;
                Scale = Vector2.One * defaultScale;
                yield return scaleChangeDuration;
            }

            _scalerActive = false;
        }
    }
}