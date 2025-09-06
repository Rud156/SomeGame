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
        [Export] public TextureRect abilityIcon;
        [Export] public TextureRect abilityFlasher;
        [Export] public Label abilityTimer;
        [Export] public TextureProgressBar abilityProgressBar;
        [Export] public Label abilityName;

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

        public async void TriggerAbilityFx()
        {
            try
            {
                if (!_flasherActive)
                {
                    foreach (var delayAmount in FlashCoroutine())
                    {
                        var delay = Mathf.FloorToInt(delayAmount * 1000);
                        await Task.Delay(delay);
                    }
                }

                if (!_scalerActive)
                {
                    foreach (var delayAmount in ScaleCoroutine())
                    {
                        var delay = Mathf.FloorToInt(delayAmount * 1000);
                        await Task.Delay(delay);
                    }
                }
            }
            catch (Exception e)
            {
                GD.Print($"Unable to run async TriggerAbilityFx: {e.Message}");
            }
        }

        // ================================
        // Private Functions
        // ================================

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