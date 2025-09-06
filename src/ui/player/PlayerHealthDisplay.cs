using Godot;

namespace SomeGame.UI.Player
{
    public partial class PlayerHealthDisplay : Control
    {
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
    }
}