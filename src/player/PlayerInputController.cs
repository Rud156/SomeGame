using Godot;

namespace SomeGame.Player
{
    public partial class PlayerInputController : Node3D
    {
        // ================================
        // Singleton
        // ================================
        private static PlayerInputController _instance;

        public static PlayerInputController Instance => _instance;

        // ================================
        // Signals
        // ================================
        [Signal]
        public delegate void OnJumpPressedEventHandler();

        // ================================
        // Constants
        // ================================
        private const string ForwardEvent = "FORWARD";
        private const string BackwardEvent = "BACKWARD";
        private const string LeftEvent = "LEFT";
        private const string RightEvent = "RIGHT";
        private const string JumpEvent = "JUMP";

        private const string Ability1Event = "ABILITY_1";
        private const string Ability2Event = "ABILITY_2";

        // Distance for the mouse raycast.
        private const int MouseRaycastDistance = 2000;

        // ================================
        // Data
        // ================================
        // Private fields to store input data.
        private Vector2 _mouseInput;
        private Vector3 _mousePosition;
        private Vector2 _movementInput;
        private Vector2 _lastNonZeroMovementInput;
        private bool _ability1Pressed;
        private bool _ability2Pressed;

        // ================================
        // Properties
        // ================================
        public Vector2 MouseInput => _mouseInput;
        public Vector3 MousePosition => _mousePosition;
        public Vector2 MovementInput => _movementInput;
        public Vector2 LastNonZeroMovementInput => _lastNonZeroMovementInput;
        public bool Ability1Pressed => _ability1Pressed;
        public bool Ability2Pressed => _ability2Pressed;

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
            if (@event is InputEventMouseMotion mouseMotion)
            {
                _mouseInput = mouseMotion.Relative;
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            _mousePosition = ScreenPointToRay();
            _movementInput = Input.GetVector(LeftEvent, RightEvent, ForwardEvent, BackwardEvent);
            _ability1Pressed = Input.IsActionPressed(Ability1Event);
            _ability2Pressed = Input.IsActionPressed(Ability2Event);

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
            Input.MouseModeEnum mouseMode = capture ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
            Input.MouseMode = mouseMode;
        }

        public bool HasNoDirectionalInput()
        {
            return Mathf.IsZeroApprox(_movementInput.X) && Mathf.IsZeroApprox(_movementInput.Y);
        }

        // ================================
        // Private Functions
        // ================================

        private Vector3 ScreenPointToRay()
        {
            var spaceState = GetWorld3D().DirectSpaceState;
            var mousePosition = GetViewport().GetMousePosition();
            var camera = GetViewport().GetCamera3D();

            var rayOrigin = camera.ProjectRayOrigin(mousePosition);
            var rayEnd = rayOrigin + camera.ProjectRayNormal(mousePosition) * MouseRaycastDistance;

            var query = new PhysicsRayQueryParameters3D
            {
                From = rayOrigin,
                To = rayEnd
            };

            var rayResult = spaceState.IntersectRay(query);

            if (rayResult.TryGetValue("position", out var position))
                return (Vector3)position;

            return Vector3.Zero;
        }
    }
}