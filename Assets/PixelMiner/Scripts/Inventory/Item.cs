using UnityEngine;
namespace PixelMiner
{
    public abstract class Item : MonoBehaviour
    {
        public ItemData Data { get; set; }
        public Vector3 Offset;
        public Vector3 RotAngles;
        protected MeshRenderer[] meshRenderers;

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
    }

}
