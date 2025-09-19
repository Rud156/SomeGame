using Godot;

namespace SomeGame.Player
{
    [GlobalClass]
    public partial class PlayerInfoDataDisplay : Resource
    {
        // ================================
        // Export
        // ================================

        [Export] private string playerName;
        [Export] private Texture2D playerIcon;
    }
}