namespace PixelMiner
{
    public interface IUseable
    {
        public void Use(Player player);
        public int RemainingUses { get; }
    }

}
