using SharpDX.XInput;

namespace MuteWindowHotkey
{
    public class XInputController
    {
        Controller controller;
        Gamepad gamepad;
        public bool connected = false;
        public int deadband = 2500;
        public Point leftThumb, rightThumb = new Point(0, 0);
        public float leftTrigger, rightTrigger;
        public Dictionary<string, bool> Buttons { get; set; } = new Dictionary<string, bool>()
        {
            { "A", false },
            { "B", false },
            { "X", false },
            { "Y", false },
            { "Start", false },
            { "Select", false },
            { "RB", false },
            { "LB", false },
            { "DPadUp", false },
            { "DPadDown", false },
            { "DPadLeft", false },
            { "DPadRight", false },
            { "LeftStick", false },
            { "RightStick", false },
            { "LT", false },
            { "RT", false }
        };
        public XInputController()
        {
            controller = new Controller(UserIndex.One);
            connected = controller.IsConnected;
        }

        // Call this method to update all class values
        public void Update()
        {
            if (!connected)
                return;

            gamepad = controller.GetState().Gamepad;

            Buttons["A"] = gamepad.Buttons.HasFlag(GamepadButtonFlags.A);
            Buttons["B"] = gamepad.Buttons.HasFlag(GamepadButtonFlags.B);
            Buttons["X"] = gamepad.Buttons.HasFlag(GamepadButtonFlags.X);
            Buttons["Y"] = gamepad.Buttons.HasFlag(GamepadButtonFlags.Y);
            Buttons["Start"] = gamepad.Buttons.HasFlag(GamepadButtonFlags.Start);
            Buttons["Select"] = gamepad.Buttons.HasFlag(GamepadButtonFlags.Back);
            Buttons["RB"] = gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder);
            Buttons["LB"] = gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder);
            Buttons["DPadUp"] = gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp);
            Buttons["DPadDown"] = gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown);
            Buttons["DPadLeft"] = gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft);
            Buttons["DPadRight"] = gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight);
            Buttons["LeftStick"] = gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb);
            Buttons["RightStick"] = gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb);

            if (gamepad.LeftTrigger > 0.5)
                Buttons["LT"] = true;
            else
                Buttons["LT"] = false;
            if(gamepad.RightTrigger > 0.5)
                Buttons["RT"] = true;
            else
                Buttons["RT"] = false;
            Thread.Sleep(10);
        }
    }
}
