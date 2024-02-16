using UnityEngine;
namespace PixelMiner
{
    public class Pickaxe : MonoBehaviour, IItem, IUseable
    {
        public static System.Action OnItemBroken;

        private int _remainingUses;
        public ItemData Data { get ; set ;}
        public int RemainingUses { get => _remainingUses; }

  
        public void Initialize()
        {
            if(Data == null)
            {
                throw new System.NotImplementedException();
            }
            
            _remainingUses = Data.MaxUses;
        }


        public void Use(Player player)
        {
            if (_remainingUses > 0)
            {
                // Implemeent use logic here.



                // Decrease the remaining uses
                _remainingUses--;


                // Handle item is broken.
                if(_remainingUses == 0)
                {
                    Debug.Log($"{Data.ItemName} has been broken.");
                    Destroy(this.gameObject);
                    OnItemBroken?.Invoke();
                }
            }
            else
            {
                Debug.Log($"Out of uses for: {Data.ItemName}");
            }
        }

        
    }
}
