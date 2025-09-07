using Godot;

namespace SomeGame.Behaviors.HealthSystem
{
    public partial class HealthAndDamage : Node3D
    {
        // ================================
        // Exports
        // ================================

        [Export] public float maxHealth;

        // Signals
        [Signal]
        public delegate void OnHealthChangedEventHandler(float oldHealth, float newHealth, float maxHealth);

        [Signal]
        public delegate void OnDiedEventHandler(float maxHealth);

        // Data
        private float _currentHealth;

        // ================================
        // Properties
        // ================================

        public float MaxHealth => maxHealth;

        public float CurrentHealth => _currentHealth;

        // ================================
        // Override Functions
        // ================================

        public override void _Ready()
        {
            _currentHealth = maxHealth;
            EmitSignal(SignalName.OnHealthChanged, 0, _currentHealth, maxHealth);
        }

        // ================================
        // Public Functions
        // ================================

        public void TakeDamage(float damage)
        {
            var oldHealth = _currentHealth;

            _currentHealth -= damage;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);

            EmitSignal(SignalName.OnHealthChanged, oldHealth, _currentHealth, maxHealth);
            if (_currentHealth <= 0)
            {
                EmitSignal(SignalName.OnDied, maxHealth);
            }
        }

        public void Heal(float heal)
        {
            var oldHealth = _currentHealth;

            _currentHealth += heal;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);

            EmitSignal(SignalName.OnHealthChanged, oldHealth, _currentHealth, maxHealth);
            if (_currentHealth <= 0)
            {
                EmitSignal(SignalName.OnDied, maxHealth);
            }
        }
    }
}