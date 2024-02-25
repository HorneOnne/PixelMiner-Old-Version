namespace PixelMiner.Enums
{
    [System.Flags]
    public enum ControlScheme
    {
        Touch = 1 << 1,
        Controller = 1 << 2,
        KeyboardAndMouse = 1 << 3,
        ALl = Touch | Controller | KeyboardAndMouse
    }

}

