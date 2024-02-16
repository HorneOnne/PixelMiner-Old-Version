namespace PixelMiner
{
    public interface IItem 
    {
        public ItemData Data { get; set; }
        public void Initialize();
    }

}
