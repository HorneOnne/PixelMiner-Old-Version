namespace PixelMiner
{
    public interface IUseable
    {
        public bool Use(Player player);
        public int RemainingUses { get; }
    }
}
