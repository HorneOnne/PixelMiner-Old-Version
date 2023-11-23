using UnityEngine;
using PixelMiner.WorldGen;
using PixelMiner.Enums;
using PixelMiner.Utilities;
using System.Collections.Generic;

namespace PixelMiner
{
    public class PlayerMovement : MonoBehaviour
    {
        private InputHander _input;
        private Rigidbody2D _rb;
        private Animator _anim;
        private Transform _model;
        [SerializeField] private float moveSpeed;
        public Vector2 _moveDirection;

        private bool _hasAnimator;
        private bool _facingRight;

        // animation IDs
        private int _animIDVelocityX;
        private int _animIDVelocityY;


        [SerializeField] private Vector2 _predictMovePosition;
        private List<Vector2> _moveSuggestions = new List<Vector2>();
        private Tile _predictTile;


        private void Awake()
        {
            _model = transform.Find("Model");
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponentInChildren<Animator>();

            _hasAnimator = _anim != null;
        }
        void Start()
        {
            _input = InputHander.Instance;
            AssignAnimationIDs();
        }




        private void Update()
        {
            Main.Instance.SetTileColor(transform.position, Color.red);
            

            if (_input.Move.x != 0 || _input.Move.y != 0)
            {
                SmartMove();
            }


            // Animation
            // =========
            Flip(_input.Move);
            if (_hasAnimator)
            {
                _anim.SetFloat(_animIDVelocityX, _input.Move.x);
                _anim.SetFloat(_animIDVelocityY, _input.Move.y);
            }
        }
        private void FixedUpdate()
        {
            if (_input.Move == Vector2.zero)
            {
                _rb.velocity = Vector2.zero;
            }
            else if (_predictTile != null &&
                    (_predictTile.HeightType == HeightType.DeepWater ||
                     _predictTile.HeightType == HeightType.ShallowWater ||
                     _predictTile.HeightType == HeightType.River))
            {
                Vector2 suggestionDirection = GetSuggestMoveDirection();
                if (suggestionDirection == Vector2.zero)
                {
                    _rb.velocity = Vector2.zero;
                }
                else
                {
                    Movement(suggestionDirection);
                }
            }
            else
            {
                Movement(_input.Move);
            }

        }

        private void SmartMove()
        {

            _predictMovePosition = GetPredictMovePosition();
            _predictTile = Main.Instance.GetTile(_predictMovePosition, out Chunk chunk);
            Main.Instance.SetTileColor(_predictMovePosition, Color.blue);
            
            _moveSuggestions.Clear();
            Vector2 centerOffset = new Vector2(0.0f, 0.5f);
            if (_input.Move.x > 0 && _input.Move.y == 0)
            {
                // Right
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.UpRightVector, centerOffset));
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.DownRightVector, centerOffset));

            }
            else if (_input.Move.x < 0 && _input.Move.y == 0)
            {
                // Left
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.UpLeftVector, centerOffset));
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.DownLeftVector, centerOffset));
            }
            else if (_input.Move.x == 0 && _input.Move.y > 0)
            {
                // Up
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.UpLeftVector, centerOffset));
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.UpRightVector, centerOffset));

            }
            else if (_input.Move.x == 0 && _input.Move.y < 0)
            {
                // Down
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.DownLeftVector, centerOffset));
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.DownRightVector, centerOffset));
            }
            else if (_input.Move.x > 0 && _input.Move.y > 0)
            {
                // Up Right
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.UpRightVector, centerOffset));
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.UpVector, centerOffset));
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.UpLeftVector, centerOffset));
                
            }
            else if (_input.Move.x < 0 && _input.Move.y > 0)
            {
                // Up Left
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.UpLeftVector, centerOffset));
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.UpVector, centerOffset));           
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.UpRightVector, centerOffset));

            }
            else if (_input.Move.x > 0 && _input.Move.y < 0)
            {
                // Down Right
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.DownRightVector, centerOffset));
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.DownVector, centerOffset));
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.DownLeftVector, centerOffset));
            }
            else if (_input.Move.x < 0 && _input.Move.y < 0)
            {
                // Down Left;
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.DownLeftVector, centerOffset));
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.DownVector, centerOffset));
                _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(transform.position, MathHelper.DownRightVector, centerOffset));
            }

            foreach (var suggesttion in _moveSuggestions)
            {
                Tile tile = Main.Instance.GetTile(suggesttion, out Chunk c);
   
                if (tile != null && (
                    tile.HeightType == HeightType.DeepWater ||
                    tile.HeightType == HeightType.ShallowWater ||
                    tile.HeightType == HeightType.River))
                {               
                    // Cant mmove
                    Main.Instance.SetTileColor(suggesttion, Color.black);
                }
                else
                {
                    // Can move
                    Main.Instance.SetTileColor(suggesttion, Color.green);
                }
            }
        }

        private Vector2 GetPredictMovePosition()
        {
            Vector2 predictPosition;
            Vector2 predictDirection = ConvertDiagonalVectorToDimetricProjection(_input.Move, WorldGeneration.Instance.IsometricAngle);
     

            if (_input.Move.x > 0 && _input.Move.y == 0)
            {
                // Right
                predictPosition = (Vector2)transform.position + _input.Move * 0.45f;

            }
            else if (_input.Move.x < 0 && _input.Move.y == 0)
            {
                // Left
                predictPosition = (Vector2)transform.position + _input.Move * 0.45f;
            }
            else if (_input.Move.x == 0 && _input.Move.y > 0)
            {
                // Up
                predictPosition = (Vector2)transform.position + _input.Move * 0.45f;
            }
            else if (_input.Move.x == 0 && _input.Move.y < 0)
            {
                // Down
                predictPosition = (Vector2)transform.position + _input.Move * 0.45f;
            }
            else if (_input.Move.x > 0 && _input.Move.y > 0)
            {
                // Top Right
                predictPosition = (Vector2)transform.position + predictDirection * 0.45f;
            }
            else if (_input.Move.x < 0 && _input.Move.y > 0)
            {
                // Top Left
                predictPosition = (Vector2)transform.position + predictDirection * 1.2f;
            }

            else if (_input.Move.x > 0 && _input.Move.y < 0)
            {
                // Down Right
                predictPosition = (Vector2)transform.position + predictDirection * 1.2f;
            }
            else if (_input.Move.x < 0 && _input.Move.y < 0)
            {
                // Down Left
                predictPosition = (Vector2)transform.position + predictDirection * 1.2f;
            }
            else
            {
                predictPosition = (Vector2)transform.position + Vector2.zero;
            }

            return predictPosition;
        }

        private Vector2 GetSuggestMoveDirection()
        {
            foreach (var suggesttion in _moveSuggestions)
            {
                Tile tile = Main.Instance.GetTile(suggesttion, out Chunk c);

                if (tile != null && (
                    tile.HeightType == HeightType.DeepWater ||
                    tile.HeightType == HeightType.ShallowWater ||
                    tile.HeightType == HeightType.River))
                {
                    // Cant mmove
                    Main.Instance.SetTileColor(suggesttion, Color.black);
                }
                else
                {
                    return (suggesttion - (Vector2)transform.position).normalized;
                }
            }
            return Vector2.zero;
        }

 



        private void AssignAnimationIDs()
        {
            _animIDVelocityX = Animator.StringToHash("VelocityX");
            _animIDVelocityY = Animator.StringToHash("VelocityY");

        }


        private void Movement(Vector2 direction)
        {
            // Move diagonally
            if (_input.Move.x != 0 && _input.Move.y != 0)
            {
                direction = ConvertDiagonalVectorToDimetricProjection(direction, WorldGeneration.Instance.IsometricAngle);
            }

            _rb.velocity = direction * moveSpeed;
        }

        private void Flip(Vector2 move)
        {
            if (move.x > 0)
            {
                _model.localScale = new Vector3(-1, 1, 1);
            }
            else if (move.x < 0)
            {
                _model.localScale = new Vector3(1, 1, 1);
            }
        }

        /// <summary>
        /// Converts a 2D diagonal vector (45 degrees) in a manner consistent with dimetric projection based on the specified angle.
        /// </summary>
        private Vector2 ConvertDiagonalVectorToDimetricProjection(Vector2 originalVector, float desiredAngle)
        {
            float radians = Mathf.Deg2Rad * desiredAngle;
            float magnitude = originalVector.magnitude;

            // Calculate the new vector components, considering the signs of the original components
            float newX = originalVector.x >= 0 ? magnitude * Mathf.Cos(radians) : -magnitude * Mathf.Cos(radians);
            float newY = originalVector.y >= 0 ? magnitude * Mathf.Sin(radians) : -magnitude * Mathf.Sin(radians);
            return new Vector2(newX, newY);
        }


        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(_predictMovePosition, 0.15f);
        }

#if DEV_MODE
        public void SetPlayerSpeed(float value)
        {
            this.moveSpeed = value;
        }
#endif
    }

}
