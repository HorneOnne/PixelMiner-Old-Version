using UnityEngine;
using PixelMiner.DataStructure;

namespace PixelMiner
{
    public abstract class Item : MonoBehaviour
    {
        public ItemData Data { get; set; }
        public Vector3 Offset;
        public Vector3 RotAngles;
        protected Renderer[] renderer;

        [SerializeField] protected DynamicEntity dEntity;
        public DynamicEntity DynamicEntity { get=> dEntity;}
        [field: SerializeField] public Vector3 BoxOffset { get; set; }    
        [field: SerializeField] public Vector3 BoxSize { get; set; }
        public LayerMask PhysicLayer;

        protected virtual void Awake()
        {
            renderer = GetComponentsInChildren<Renderer>();
            for(int i = 0; i < renderer.Length; i++)
            {
                renderer[i].enabled = false;
            }
            
        }

        public virtual void Initialize(ItemData data)
        {
            this.Data = data;
            for (int i = 0; i < renderer.Length; i++)
            {
                renderer[i].enabled = true;
            }
        }

        public virtual void EnablePhysics()
        {
           
        }
    }

}
