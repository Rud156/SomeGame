using Godot;
using Godot.Collections;

namespace SomeGame.Player
{
    public partial class CustomInputController : Node3D
    {
        // ================================
        // Singleton
        // ================================

        private static CustomInputController _instance;
        public static CustomInputController Instance => _instance;

        // ================================
        // Constants
        // ================================

        private const string ForwardEvent = "FORWARD";
        private const string BackwardEvent = "BACKWARD";
        private const string LeftEvent = "LEFT";
        private const string RightEvent = "RIGHT";
        private const string LookForwardEvent = "LOOK_FORWARD";
        private const string LookBackwardEvent = "LOOK_BACKWARD";
        private const string LookLeftEvent = "LOOK_LEFT";
        private const string LookRightEvent = "LOOK_RIGHT";


        private const string JumpEvent = "JUMP";

        private const string Ability1Event = "ABILITY_1";
        private const string Ability2Event = "ABILITY_2";

        private const int MouseRaycastDistance = 2000;

        // ================================
        // Signals
        // ================================

        [Signal]
        public delegate void OnJumpPressedEventHandler();

        [Signal]
        public delegate void OnInputTypeUpdatedEventHandler(InputDeviceType inputDeviceType);

        // Data
        private Vector3 _mousePosition;
        private Vector2 _lookGamepadInput;
        private Vector2 _lastLookGamepadInput;

        private Vector2 _movementInput;
        private Vector2 _lastNonZeroMovementInput;

        private bool _ability1Pressed;
        private bool _ability2Pressed;

        // Input Type
        private InputDeviceType _lastUsedInputDeviceType;

        // ================================
        // Properties
        // ================================

        public Vector3 MousePosition => _mousePosition;
        public Vector2 LookGamepadInput => _lookGamepadInput;
        public Vector2 LastLookGamepadInput => _lastLookGamepadInput;
        public Vector2 MovementInput => _movementInput;
        public Vector2 LastNonZeroMovementInput => _lastNonZeroMovementInput;
        public bool Ability1Pressed => _ability1Pressed;
        public bool Ability2Pressed => _ability2Pressed;

        public InputDeviceType LastUsedInputDeviceType => _lastUsedInputDeviceType;

        // ================================
        // Override Functions
        // ================================

        // This method is called when the node enters the scene tree for the first time.
        public override void _EnterTree()
        {
            if (_instance != null)
            {
                QueueFree();
                return;
            }

            _instance = this;
        }

        public override void _Input(InputEvent @event)
        {
            switch (@event)
            {
                case InputEventMouseMotion:
                case InputEventKey:
                    _UpdateLastUsedInputDevice(InputDeviceType.Keyboard);
                    break;

                case InputEventJoypadMotion:
                case InputEventJoypadButton:
                    _UpdateLastUsedInputDevice(InputDeviceType.Gamepad);
                    break;
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            _mousePosition = _ScreenPointToRay();
            _lookGamepadInput = Input.GetVector(LookLeftEvent, LookRightEvent, LookBackwardEvent, LookForwardEvent);
            _movementInput = Input.GetVector(LeftEvent, RightEvent, BackwardEvent, ForwardEvent);
            _ability1Pressed = Input.IsActionPressed(Ability1Event);
            _ability2Pressed = Input.IsActionPressed(Ability2Event);

            if (!Mathf.IsZeroApprox(_lookGamepadInput.LengthSquared()))
            {
                _lastLookGamepadInput = _lookGamepadInput;
            }

            if (!Mathf.IsZeroApprox(_movementInput.LengthSquared()))
            {
                _lastNonZeroMovementInput = _movementInput;
            }

            if (Input.IsActionJustPressed(JumpEvent))
            {
                EmitSignal(SignalName.OnJumpPressed);
            }
        }

        // ================================
        // Public Functions
        // ================================

        public void SetMouseMode(bool capture)
        {
            var mouseMode = capture ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
            Input.MouseMode = mouseMode;
        }

        public bool HasNoDirectionalInput() => Mathf.IsZeroApprox(_movementInput.X) && Mathf.IsZeroApprox(_movementInput.Y);

        public bool HasNoLookGamepadInput() => Mathf.IsZeroApprox(_lookGamepadInput.X) && Mathf.IsZeroApprox(_lookGamepadInput.Y);

        public bool IsAbilityTriggerPressed(int abilityIndex)
        {
            return abilityIndex switch
            {
                0 => _ability1Pressed,
                1 => _ability2Pressed,
                _ => false
            };
        }

        // ================================
        // Private Functions
        // ================================

        private Vector3 _ScreenPointToRay()
        {
            var spaceState = GetWorld3D().DirectSpaceState;
            var mousePosition = GetViewport().GetMousePosition();
            var camera = GetViewport().GetCamera3D();

            var rayOrigin = camera.ProjectRayOrigin(mousePosition);
            var rayEnd = rayOrigin + camera.ProjectRayNormal(mousePosition) * MouseRaycastDistance;

            var queryParams = new PhysicsRayQueryParameters3D
            {
                From = rayOrigin,
                To = rayEnd
            };

            var rayResult = spaceState.IntersectRay(queryParams);
            if (rayResult.TryGetValue("position", out var position))
                return (Vector3)position;

            return _mousePosition;
        }

        private void _UpdateLastUsedInputDevice(InputDeviceType inputDeviceType)
        {
            _lastUsedInputDeviceType = inputDeviceType;

            SetMouseMode(inputDeviceType == InputDeviceType.Gamepad);
            EmitSignal(SignalName.OnInputTypeUpdated, (int)inputDeviceType);
        }
    }

    // ================================
    // Enums
    // ================================

    public enum InputDeviceType
    {
        Keyboard = 0,
        Gamepad = 1
    }
}