using System;
namespace PixelMiner.Enums
{
    [Flags]
    public enum InventoryID : byte
    {
        None = 0,       // No flags set
        Hotbars = 1,
        Bag = 2,
        Chest = 4,
    }

}
