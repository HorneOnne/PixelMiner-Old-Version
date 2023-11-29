using UnityEngine;
using PixelMiner.WorldGen;
using PixelMiner.Enums;
using PixelMiner.Utilities;
using System.Collections.Generic;
using PixelMiner.WorldBuilding;

namespace PixelMiner
{
    public class PlayerMovement : MonoBehaviour
    {
        private InputHander _input;
        private Rigidbody _rb;
        private Animator _anim;
        private SpriteRenderer _sr;
        [SerializeField] private float moveSpeed;
        public Vector3 _moveDirection;

        private bool _hasAnimator;
        private bool _facingRight;

        // animation IDs
        private int _animIDVelocityX;
        private int _animIDVelocityY;


        [SerializeField] private Vector3 _predictMovePosition;
        private List<Vector3> _moveSuggestions = new List<Vector3>();
        private Tile _predictTile;
        private Vector3 _lastGroundTilePosition;   // Use to prevent move to fast through water tile.

        public bool FlyMode;


        // Continuous move
        public bool ContinuousMove;



        //private void Awake()
        //{
        //    _sr = GetComponentInChildren<SpriteRenderer>();
        //    _rb = GetComponent<Rigidbody>();
        //    _anim = GetComponentInChildren<Animator>();

        //    _hasAnimator = _anim != null;
        //}
        //void Start()
        //{
        //    _input = InputHander.Instance;
        //    AssignAnimationIDs();

        //    _lastGroundTilePosition = transform.position;
        //}




        //private void Update()
        //{
        //    Main.Instance.ToggleTileColor(_rb.position, Color.red);

        //    if(ContinuousMove)
        //    {
        //        _moveDirection = _input.Move == Vector2.zero ? _moveDirection : new Vector3(_input.Move.x, 0, _input.Move.y);
        //    }
        //    else
        //    {
        //        _moveDirection = new Vector3(_input.Move.x, 0, _input.Move.y);
        //    }
            
        //    if(_input.Cancel)
        //    {
        //        _moveDirection = Vector2.zero;
        //    }


        //    if (_moveDirection.x != 0 || _moveDirection.z != 0)
        //    {
        //        SmartMove();
        //        Tile curPositionTile = Main.Instance.GetTile(_rb.position, out Chunk2D chunk);
        //        if (!(curPositionTile != null && (
        //            curPositionTile.HeightType == HeightType.DeepWater ||
        //            curPositionTile.HeightType == HeightType.ShallowWater ||
        //            curPositionTile.HeightType == HeightType.River)))
        //        {
        //            _lastGroundTilePosition = _rb.position;
        //        }
        //    }
        //    else
        //    {
        //        _rb.MovePosition(_lastGroundTilePosition);
        //    }


      
         

        //    // Animation
        //    // =========
        //    Flip(_moveDirection);
        //    if (_hasAnimator)
        //    {
        //        _anim.SetFloat(_animIDVelocityX, _moveDirection.x);
        //        _anim.SetFloat(_animIDVelocityY, _moveDirection.z);
        //    }
        //}
        //private void FixedUpdate()
        //{
        //    if (FlyMode == true)
        //    {
        //        // Move diagonally
        //        Vector2 direction = _moveDirection;
        //        if (_moveDirection.x != 0 && _moveDirection.z != 0)
        //        {
        //            direction = ConvertDiagonalVectorToDimetricProjection(direction, Main.Instance.IsometricAngle);
        //        }
        //        Movement(direction);
        //    }
        //    else
        //    {
        //        if (_moveDirection == Vector3.zero)
        //        {
        //            _rb.velocity = Vector2.zero;
        //        }
        //        else if (_predictTile != null &&
        //                (_predictTile.HeightType == HeightType.DeepWater ||
        //                 _predictTile.HeightType == HeightType.ShallowWater ||
        //                 _predictTile.HeightType == HeightType.River))
        //        {
        //            Vector3 suggestionDirection = GetSuggestMoveDirection();
        //            if (suggestionDirection == Vector3.zero)
        //            {
        //                _rb.velocity = Vector3.zero;
        //            }
        //            else
        //            {
        //                if (_moveDirection.x != 0 && _moveDirection.z != 0)
        //                {
        //                    suggestionDirection = ConvertDiagonalVectorToDimetricProjection(suggestionDirection, Main.Instance.IsometricAngle);
        //                }
        //                Movement(suggestionDirection);
        //            }
        //        }
        //        else
        //        {
        //            Vector3 direction = _moveDirection;
        //            if (_moveDirection.x != 0 && _moveDirection.z != 0)
        //            {
        //                direction = ConvertDiagonalVectorToDimetricProjection(direction, Main.Instance.IsometricAngle);
        //            }
        //            Movement(direction);
        //        }
        //    }
        //}

        //private void SmartMove()
        //{

        //    _predictMovePosition = GetPredictMovePosition();
        //    _predictTile = Main.Instance.GetTile(_predictMovePosition, out Chunk2D chunk);
        //    Main.Instance.ToggleTileColor(_predictMovePosition, Color.blue);

           
        //    _moveSuggestions.Clear();
        //    Vector2 centerOffset = new Vector2(0.0f, 0.5f);
        //    Vector2 playerPosition = _rb.position;
        //    if (_moveDirection.x > 0 && _moveDirection.z == 0)
        //    {
        //        // Right
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.UpRightVector, centerOffset));
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.DownRightVector, centerOffset));

        //    }
        //    else if (_moveDirection.x < 0 && _moveDirection.z == 0)
        //    {
        //        // Left
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.UpLeftVector, centerOffset));
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.DownLeftVector, centerOffset));
        //    }
        //    else if (_moveDirection.x == 0 && _moveDirection.z > 0)
        //    {
        //        // Up
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.UpLeftVector, centerOffset));
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.UpRightVector, centerOffset));

        //    }
        //    else if (_moveDirection.x == 0 && _moveDirection.z < 0)
        //    {
        //        // Down
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.DownLeftVector, centerOffset));
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.DownRightVector, centerOffset));
        //    }
        //    else if (_moveDirection.x > 0 && _moveDirection.z > 0)
        //    {
        //        // Up Right
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.UpRightVector, centerOffset));
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.UpVector, centerOffset));
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.UpLeftVector, centerOffset));

        //    }
        //    else if (_moveDirection.x < 0 && _moveDirection.z > 0)
        //    {
        //        // Up Left
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.UpLeftVector, centerOffset));
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.UpVector, centerOffset));
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.UpRightVector, centerOffset));

        //    }
        //    else if (_moveDirection.x > 0 && _moveDirection.z < 0)
        //    {
        //        // Down Right
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.DownRightVector, centerOffset));
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.DownVector, centerOffset));
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.DownLeftVector, centerOffset));
        //    }
        //    else if (_moveDirection.x < 0 && _moveDirection.z < 0)
        //    {
        //        // Down Left;
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.DownLeftVector, centerOffset));
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.DownVector, centerOffset));
        //        _moveSuggestions.Add(Main.Instance.GetNeighborWorldPosition(playerPosition, MathHelper.DownRightVector, centerOffset));
        //    }

        //    foreach (var suggesttion in _moveSuggestions)
        //    {
        //        Tile tile = Main.Instance.GetTile(suggesttion, out Chunk2D c);

        //        if (tile != null && (
        //            tile.HeightType == HeightType.DeepWater ||
        //            tile.HeightType == HeightType.ShallowWater ||
        //            tile.HeightType == HeightType.River))
        //        {
        //            // Cant mmove
        //            Main.Instance.ToggleTileColor(suggesttion, Color.black);
        //        }
        //        else
        //        {
        //            // Can move
        //            Main.Instance.ToggleTileColor(suggesttion, Color.green);
        //        }
        //    }
        //}

        //private Vector3 GetPredictMovePosition()
        //{
        //    Vector3 predictPosition;
        //    Vector3 predictDirection = ConvertDiagonalVectorToDimetricProjection(_moveDirection, Main.Instance.IsometricAngle);
        //    Vector3 playerPosition = _rb.position;


        //    if (_moveDirection.x > 0 && _moveDirection.z == 0)
        //    {
        //        // Right
        //        predictPosition = playerPosition + _moveDirection * 0.45f;

        //    }
        //    else if (_moveDirection.x < 0 && _moveDirection.z == 0)
        //    {
        //        // Left
        //        predictPosition = playerPosition + _moveDirection * 0.45f;
        //    }
        //    else if (_moveDirection.x == 0 && _moveDirection.z > 0)
        //    {
        //        // Up
        //        predictPosition = playerPosition + _moveDirection * 0.45f;
        //    }
        //    else if (_moveDirection.x == 0 && _moveDirection.z < 0)
        //    {
        //        // Down
        //        predictPosition = playerPosition + _moveDirection * 0.45f;
        //    }
        //    else if (_moveDirection.x > 0 && _moveDirection.z > 0)
        //    {
        //        // Top Right
        //        predictPosition = playerPosition + predictDirection * 0.45f;
        //    }
        //    else if (_moveDirection.x < 0 && _moveDirection.z > 0)
        //    {
        //        // Top Left
        //        predictPosition = playerPosition + predictDirection * 1.2f;
        //    }

        //    else if (_moveDirection.x > 0 && _moveDirection.z < 0)
        //    {
        //        // Down Right
        //        predictPosition = playerPosition + predictDirection * 1.2f;
        //    }
        //    else if (_moveDirection.x < 0 && _moveDirection.z < 0)
        //    {
        //        // Down Left
        //        predictPosition = playerPosition + predictDirection * 1.2f;
        //    }
        //    else
        //    {
        //        predictPosition = playerPosition + Vector3.zero;
        //    }

        //    return predictPosition;
        //}

        //private Vector3 GetSuggestMoveDirection()
        //{
        //    foreach (var suggesttion in _moveSuggestions)
        //    {
        //        Tile tile = Main.Instance.GetTile(suggesttion, out Chunk2D c);

        //        if (tile != null && (
        //            tile.HeightType == HeightType.DeepWater ||
        //            tile.HeightType == HeightType.ShallowWater ||
        //            tile.HeightType == HeightType.River))
        //        {
        //            // Cant mmove
        //            Main.Instance.ToggleTileColor(suggesttion, Color.black);
        //        }
        //        else
        //        {
        //            return (suggesttion - _rb.position).normalized;
        //        }
        //    }
        //    return Vector3.zero;
        //}





        //private void AssignAnimationIDs()
        //{
        //    _animIDVelocityX = Animator.StringToHash("VelocityX");
        //    _animIDVelocityY = Animator.StringToHash("VelocityY");

        //}


        //private void Movement(Vector3 direction)
        //{
        //    Debug.Log($"Move");
        //    _rb.velocity = direction * moveSpeed;
        //}

        //private void Flip(Vector2 move)
        //{
        //    if (move.x > 0)
        //    {
        //        _sr.flipX = false;
        //    }
        //    else if (move.x < 0)
        //    {
        //        _sr.flipX = true;
        //    }
        //}

        ///// <summary>
        ///// Converts a 2D diagonal vector (45 degrees) in a manner consistent with dimetric projection based on the specified angle.
        ///// </summary>
        //private Vector2 ConvertDiagonalVectorToDimetricProjection(Vector2 originalVector, float desiredAngle)
        //{
        //    float radians = Mathf.Deg2Rad * desiredAngle;
        //    float magnitude = originalVector.magnitude;

        //    // Calculate the new vector components, considering the signs of the original components
        //    float newX = originalVector.x >= 0 ? magnitude * Mathf.Cos(radians) : -magnitude * Mathf.Cos(radians);
        //    float newY = originalVector.y >= 0 ? magnitude * Mathf.Sin(radians) : -magnitude * Mathf.Sin(radians);
        //    return new Vector2(newX, newY);
        //}


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
