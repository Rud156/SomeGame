using System;
using System.Collections.Generic;
using Godot;
using SomeGame.Behaviors.Abilities.Base;
using SomeGame.Behaviors.Common;


namespace SomeGame.Enemies.Core
{
    public abstract partial class EnemyController : DamageHitStopPropagator
    {
        // ================================
        // Constants
        // ================================

        protected const float Gravity = -9.8f;

        // ================================
        // Export
        // ================================

        [ExportGroup("Detection")]
        [Export] protected float detectionRadius;
        [Export(PropertyHint.Layers3DPhysics)] protected uint detectionMask;

        [ExportGroup("Components")]
        [Export] protected AbilityProcessor abilityProcessor;

        // ================================
        // Signals
        // ================================
        [Signal]
        public delegate void OnEnemyStateChangedEventHandler(CoreEnemyState enemyState);

        // Enemy State
        private List<CoreEnemyState> _stateStack;

        // ================================
        // Override Functions
        // ================================

        public override void _Ready()
        {
            _stateStack = [];

            PushEnemyState(CoreEnemyState.Idle);

            abilityProcessor.OnAbilityStarted += HandleAbilityStarted;
            abilityProcessor.OnAbilityEnded += HandleAbilityEnded;
        }

        public override void _ExitTree()
        {
            abilityProcessor.OnAbilityStarted -= HandleAbilityStarted;
            abilityProcessor.OnAbilityEnded -= HandleAbilityEnded;
        }

        public override void _Process(double delta)
        {
            float deltaTime = (float)delta;

            if (HitStopBehavior is { IsActive: false })
            {
                _HandleMovement(deltaTime);
            }
        }

        // ================================
        // Public Functions
        // ================================

        public CoreEnemyState TopEnemyState => _stateStack[^1];

        // ================================
        // Private Functions
        // ================================

        private void _HandleMovement(float delta)
        {
            switch (TopEnemyState)
            {
                case CoreEnemyState.Idle:
                    UpdateIdleState(delta);
                    break;

                case CoreEnemyState.Patrolling:
                    UpdatePatrollingState(delta);
                    break;

                case CoreEnemyState.Alerted:
                    UpdateAlertedState(delta);
                    break;

                case CoreEnemyState.Chasing:
                    UpdateChasingState(delta);
                    break;

                case CoreEnemyState.Attacking:
                    UpdateAttackingState(delta);
                    break;

                case CoreEnemyState.Fallback:
                    UpdateFallbackState(delta);
                    break;

                case CoreEnemyState.Dead:
                    UpdateDeadState(delta);
                    break;

                case CoreEnemyState.CustomMovement:
                    UpdateCustomMovementState(delta);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // ================================
        // Protected Functions
        // ================================

        protected virtual void HandleAbilityEnded(AbilityBase ability)
        {
        }

        protected virtual void HandleAbilityStarted(AbilityBase ability)
        {
        }

        protected virtual void UpdateIdleState(float delta)
        {
        }

        protected virtual void UpdatePatrollingState(float delta)
        {
        }

        protected virtual void UpdateAlertedState(float delta)
        {
        }

        protected virtual void UpdateChasingState(float delta)
        {
        }

        protected virtual void UpdateAttackingState(float delta)
        {
        }

        protected virtual void UpdateFallbackState(float delta)
        {
        }

        protected virtual void UpdateDeadState(float delta)
        {
        }

        protected virtual void UpdateCustomMovementState(float delta)
        {
        }

        protected void PushEnemyState(CoreEnemyState newState)
        {
            _stateStack.Add(newState);
            EmitSignal(SignalName.OnEnemyStateChanged, (int)newState);
        }

        protected void PopEnemyState()
        {
            _stateStack.RemoveAt(_stateStack.Count - 1);
            EmitSignal(SignalName.OnEnemyStateChanged, (int)TopEnemyState);
        }
    }

    // ================================
    // Enums
    // ================================

    public enum CoreEnemyState
    {
        Idle,
        Patrolling,
        Alerted,
        Chasing,
        Attacking,
        Fallback,
        Dead,
        CustomMovement
    }
}