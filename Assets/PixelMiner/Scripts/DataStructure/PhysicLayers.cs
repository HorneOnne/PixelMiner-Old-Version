namespace PixelMiner.DataStructure
{
    [System.Flags]
    public enum PhysicLayers : int
    {
        None = 0x0,
        Default = 0x1,

        Player = 0x2,
        Items = 0x4,
    }

    public static class PhysicLayersExtension
    {
        static PhysicLayersExtension()
        {

        }

        //private static void InitializeSolidTransparentBlocksSet()
        //{
        //    _solidTransparentVoxelSet = new HashSet<BlockType>()
        //    {
        //            {BlockType.Glass},
        //            {BlockType.Leaves},
        //    };
        //}


        //public static PhysicLayers GetCollideLayers(this PhysicLayers layer)
        //{
            
        //}



    }

}
