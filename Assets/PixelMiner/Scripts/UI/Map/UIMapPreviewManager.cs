using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using PixelMiner.WorldGen;

namespace PixelMiner.UI
{
    public class UIMapPreviewManager : MonoBehaviour,IPointerUpHandler, IPointerDownHandler
    {
        public static UIMapPreviewManager Instance { get; private set; }
        public UIMapPreview HeightMapPreview { get; private set; }
        public UIMapPreview HeatMapPreview { get; private set; }
        public UIMapPreview MoistureMapPreview { get; private set; }

        private RectTransform _rectTransform;
        [ReadOnly] public float TargetZoom = 1.0f;
        [ReadOnly] public float MinZoom = 0.001f;
        [ReadOnly] public float MaxZoom = 10.0f;
        [ReadOnly] public float ZoomSpeed = 0.05f;

        public Vector2 Offset = Vector2.zero;

        // Drag

        private Vector2 _dragStartPosition;


        private void Awake()
        {
            Instance = this;
            _rectTransform = GetComponent<RectTransform>();
            HeightMapPreview = transform.Find("HeightMapPreview")?.GetComponent<UIMapPreview>();
            HeatMapPreview = transform.Find("HeatMapPreview")?.GetComponent<UIMapPreview>();
            MoistureMapPreview = transform.Find("MoistureMapPreview")?.GetComponent<UIMapPreview>();
        }


        public bool HasHeightMap() => HeightMapPreview != null;
        public bool HasHeatMap() => HeatMapPreview != null;
        public bool HasMoistureMap() => MoistureMapPreview != null;


        public void SetActiveHeightMap()
        {
            if (HasHeightMap()) HeightMapPreview.gameObject.SetActive(true);
            if (HasHeatMap()) HeatMapPreview.gameObject.SetActive(false);
            if (HasMoistureMap()) MoistureMapPreview.gameObject.SetActive(false);

            UpdateHeightMapPreview();
        }

        public void SetActiveHeatMap()
        {
            if (HasHeightMap()) HeightMapPreview.gameObject.SetActive(false);
            if (HasHeatMap()) HeatMapPreview.gameObject.SetActive(true);
            if (HasMoistureMap()) MoistureMapPreview.gameObject.SetActive(false);
        }

        public void SetActiveMoistureMap()
        {
            if (HasHeightMap()) HeightMapPreview.gameObject.SetActive(false);
            if (HasHeatMap()) HeatMapPreview.gameObject.SetActive(false);
            if (HasMoistureMap()) MoistureMapPreview.gameObject.SetActive(true);
        }


        public void CloseAllMap()
        {
            if (HasHeightMap()) HeightMapPreview.gameObject.SetActive(false);
            if (HasHeatMap()) HeatMapPreview.gameObject.SetActive(false);
            if (HasMoistureMap()) MoistureMapPreview.gameObject.SetActive(false);
        }



        private bool _needUpdateTexture = false;
        private float timeSinceLastUpdate = 0f;
        private float updateInterval = 0.05f; // Set the update interval as needed

        private float _offsetNoiseTime = 0.05f;
        private float _offsetNoiseTimer = 0.0f;
        private void Update()
        {
            TargetZoom = Mathf.Clamp(TargetZoom + InputHander.Instance.MouseScrollY * Time.deltaTime * ZoomSpeed, MinZoom, MaxZoom);

            if (InputHander.Instance.MouseScrollY > 0 || InputHander.Instance.MouseScrollY < 0)
            {
                Debug.Log("Scrolling");
                _needUpdateTexture = true;
                timeSinceLastUpdate = 0f; // Reset the time since the last update
            }
            else
            {
                timeSinceLastUpdate += Time.deltaTime;

                // Update the texture at a lower rate when there is no scrolling
                if (timeSinceLastUpdate >= updateInterval && _needUpdateTexture)
                {
                    Debug.Log("No Scroll");
                    UpdateHeightMapPreview();
                    _needUpdateTexture = false;
                    timeSinceLastUpdate = 0f; // Reset the time since the last update
                }
            }

            if (Clicked)
            {
                if (Time.time - _offsetNoiseTimer > _offsetNoiseTime)
                {
                    _offsetNoiseTimer = Time.time;
                    // Check the offset continuously in the Update method
                    if (Input.GetMouseButton(0)) // 0 corresponds to the left mouse button
                    {
                        // Calculate the normalized offset from the center
                        float normalizedOffsetX = (Input.mousePosition.x - Screen.width / 2.0f) / Screen.width;
                        float normalizedOffsetY = (Input.mousePosition.y - Screen.height / 2.0f) / Screen.height;

                        // Apply any necessary calculations or use the normalized offsets as needed
                        Debug.Log($"{normalizedOffsetX}\t{normalizedOffsetY}");

                        Offset += new Vector2(normalizedOffsetX, normalizedOffsetY) * 20f;
                        // Use dragDirection as needed
                        UpdateHeightMapPreview();

                    }
                }

            }


        }

        public bool Clicked;
        public void OnPointerUp(PointerEventData eventData)
        {
            Clicked = false;
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            Clicked = true;
        }



        private async void UpdateHeightMapPreview()
        {
            Texture2D texture = await GetHeightmapTextureAsync(Offset.x, Offset.y, TargetZoom);
            HeightMapPreview.SetImage(texture);
        }


        private async Task<Texture2D> GetHeightmapTextureAsync(float offsetX, float offsetY, float zoom)
        {
            int textureWidth = 288;
            int textureHeight = 162;
            float[,] heightValues = await WorldGeneration.Instance.GetHeightMapDataAsync(0, 0, textureWidth, textureHeight, zoom, offsetX, offsetY);

            Texture2D texture = new Texture2D(textureWidth, textureHeight);
            Color[] pixels = new Color[textureWidth * textureHeight];

            await Task.Run(() =>
            {
                Parallel.For(0, textureWidth, x =>
                {
                    for (int y = 0; y < textureHeight; y++)
                    {
                        float heightValue = heightValues[x, y];
                        if (heightValue < WorldGeneration.Instance.DeepWater)
                        {
                            pixels[x + y * textureWidth] = WorldGeneration.DeepColor;
                        }
                        else if (heightValue < WorldGeneration.Instance.Water)
                        {
                            pixels[x + y * textureWidth] = WorldGeneration.ShallowColor;
                        }
                        else if (heightValue < WorldGeneration.Instance.Sand)
                        {
                            pixels[x + y * textureWidth] = WorldGeneration.SandColor;
                        }
                        else if (heightValue < WorldGeneration.Instance.Grass)
                        {
                            pixels[x + y * textureWidth] = WorldGeneration.GrassColor;
                        }
                        else if (heightValue < WorldGeneration.Instance.Forest)
                        {
                            pixels[x + y * textureWidth] = WorldGeneration.ForestColor;
                        }
                        else if (heightValue < WorldGeneration.Instance.Rock)
                        {
                            pixels[x + y * textureWidth] = WorldGeneration.RockColor;
                        }
                        else
                        {
                            pixels[x + y * textureWidth] = WorldGeneration.SnowColor;
                        }
                    }
                });
            });

            texture.SetPixels(pixels);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            texture.Apply();
            return texture;
        }
    }
}
