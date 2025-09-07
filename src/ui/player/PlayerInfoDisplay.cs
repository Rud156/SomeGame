using Godot;

namespace SomeGame.UI.Player
{
    public partial class PlayerInfoDisplay : Control
    {
        private static PlayerInfoDisplay _instance;
        public static PlayerInfoDisplay Instance => _instance;

        // ================================
        // Exports
        // ================================

        [Export] public Label playerNameLabel;
        [Export] public TextureRect playerIcon;

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

        // ================================
        // Public Functions
        // ================================

        public void SetPlayerInfo(string playerName, Texture2D playerTexture)
        {
            playerNameLabel.Text = playerName;
            playerIcon.Texture = playerTexture;
        }
    }
}