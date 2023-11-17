using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoreMiner.UI
{
    public class UIMapPreview : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        private RectTransform _rectTransform;
        private Image _image;

        public float TargetZoom;
        public float MinZoom = 0.001f;
        public float MaxZoom = 10.0f;
        public float ZoomSpeed = 0.05f;

        public Vector2 Offset = Vector2.zero;

        // Drag
      
        private Vector2 _dragStartPosition;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
        }

        private void OnEnable()
        {
            InputHander.Instance.OnMouseScrollFinish += () =>
            {
                Debug.Log("Mouse scroll finish");
            };

            PreviewHeightmap((int)Offset.x, (int)Offset.y, 0.5f);
        }

        private void OnDisable()
        {
            InputHander.Instance.OnMouseScrollFinish -= () =>
            {
                Debug.Log("Mouse scroll finish");
            };
        }

        private void SetImage(Texture2D texture)
        {
            // Convert the Texture2D to a Sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            // Set the Sprite on the Image component
            _image.sprite = sprite;
        }

        public async void PreviewHeightmap(float offsetX, float offsetY, float zoom)
        {
            if (UIMapPreviewManager.Instance.HasHeightMap())
            {
                InputHander.Instance.ActiveUIMap();

                UIMapPreviewManager.Instance.SetActiveHeightMap();

                int textureWidth = 288;
                int textureHeight = 162;
                float[,] heightValues = await WorldGeneration.Instance.GetHeightMapDataAsync(0, 0, textureWidth, textureHeight, zoom, offsetX, offsetY);

                Texture2D texture2D = new Texture2D(textureWidth, textureHeight);
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
                                pixels[x + y * textureWidth] = WorldGenUtilities.DeepColor;
                            }
                            else if (heightValue < WorldGeneration.Instance.Water)
                            {
                                pixels[x + y * textureWidth] = WorldGenUtilities.ShallowColor;
                            }
                            else if (heightValue < WorldGeneration.Instance.Sand)
                            {
                                pixels[x + y * textureWidth] = WorldGenUtilities.SandColor;
                            }
                            else if (heightValue < WorldGeneration.Instance.Grass)
                            {
                                pixels[x + y * textureWidth] = WorldGenUtilities.GrassColor;
                            }
                            else if (heightValue < WorldGeneration.Instance.Forest)
                            {
                                pixels[x + y * textureWidth] = WorldGenUtilities.ForestColor;
                            }
                            else if (heightValue < WorldGeneration.Instance.Rock)
                            {
                                pixels[x + y * textureWidth] = WorldGenUtilities.RockColor;
                            }
                            else
                            {
                                pixels[x + y * textureWidth] = WorldGenUtilities.SnowColor;
                            }
                        }
                    });
                });
               

                texture2D.SetPixels(pixels);
                texture2D.wrapMode = TextureWrapMode.Clamp;
                texture2D.filterMode = FilterMode.Point;
                texture2D.Apply();
                UIMapPreviewManager.Instance.HeightMapPreview.SetImage(texture2D);
            }
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
                    PreviewHeightmap(Offset.x, Offset.y, TargetZoom);
                    _needUpdateTexture = false;
                    timeSinceLastUpdate = 0f; // Reset the time since the last update
                }
            }

            if(Clicked)
            {
                if(Time.time - _offsetNoiseTimer > _offsetNoiseTime)
                {
                    _offsetNoiseTimer = Time.time;
                    // Check the offset continuously in the Update method
                    if (Input.GetMouseButton(0)) // 0 corresponds to the left mouse button
                    {
                        // Calculate the normalized offset from the center
                        float normalizedOffsetX = (Input.mousePosition.x - Screen.width / 2.0f) / Screen.width;
                        float normalizedOffsetY = (Input.mousePosition.y - Screen.height / 2.0f) / Screen.height;

                        // Clamp the values to be in the range [0, 1]
                        //normalizedOffsetX = Mathf.Clamp01(normalizedOffsetX);
                        //normalizedOffsetY = Mathf.Clamp01(normalizedOffsetY);

                        // Apply any necessary calculations or use the normalized offsets as needed
                        Debug.Log($"{normalizedOffsetX}\t{normalizedOffsetY}");

                        Offset += new Vector2(normalizedOffsetX, normalizedOffsetY) * 20f;
                        // Use dragDirection as needed
                        PreviewHeightmap(Offset.x, Offset.y, TargetZoom);

                        // Call your method with the offsets if needed
                        // Example:
                        // await GetHeightMapDataAsync(isoFrameX, isoFrameY, width, height, zoom, offsetXFromCenter, offsetYFromCenter);
                    }
                }
                
            }

            
        }

        #region Unity Event Interfaces
        public bool Clicked;
        public void OnPointerUp(PointerEventData eventData)
        {
            Clicked = false;
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            Clicked = true;
        }
        #endregion
    }
}
