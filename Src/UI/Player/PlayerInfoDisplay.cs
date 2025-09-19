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

        [Export] private Label _playerNameLabel;
        [Export] private TextureRect _playerIcon;

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
            _playerNameLabel.Text = playerName;
            _playerIcon.Texture = playerTexture;
        }
    }
}