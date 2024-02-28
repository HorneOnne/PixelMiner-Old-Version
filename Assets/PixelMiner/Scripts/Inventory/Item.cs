using UnityEngine;
using PixelMiner.DataStructure;

namespace PixelMiner
{
    public abstract class Item : MonoBehaviour
    {
        public ItemData Data { get; set; }
        public Vector3 Offset;
        public Vector3 RotAngles;
        protected MeshRenderer[] meshRenderers;

        protected DynamicEntity dEntity;
        public DynamicEntity DynamicEntity { get=> dEntity;}
        [field: SerializeField] public Vector3 BoxOffset { get; set; }    
        [field: SerializeField] public Vector3 BoxSize { get; set; }
        protected LayerMask PhysicLayer { get => this.gameObject.layer; }

        private void Awake()
        {
            meshRenderers = GetComponentsInChildren<MeshRenderer>();
            for(int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].enabled = false;
            }
            
        }

        public virtual void Initialize(ItemData data)
        {
            this.Data = data;
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].enabled = true;
            }
        }

        public virtual void EnablePhysics() { }
    }

}
