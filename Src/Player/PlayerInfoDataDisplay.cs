using Godot;

namespace SomeGame.Player
{
    [GlobalClass]
    public partial class PlayerInfoDataDisplay : Resource
    {
        // ================================
        // Export
        // ================================

        [Export] public string playerName;
        [Export] public Texture2D playerIcon;
    }
}