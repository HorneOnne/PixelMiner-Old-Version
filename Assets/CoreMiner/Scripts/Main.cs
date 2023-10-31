using CoreMiner.WorldGen;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoreMiner
{
    public class Main : MonoBehaviour
    {
        public static Main Instance { get; private set; }

        public List<CustomTileBase> TileBaseList = new List<CustomTileBase>();
        public List<CustomAnimatedTileBase> AnimatedTileBaseList = new List<CustomAnimatedTileBase>();
        private Dictionary<TileType, CustomTileBase> _tileBaseDict = new Dictionary<TileType, CustomTileBase>();
        private Dictionary<TileType, CustomAnimatedTileBase> _animatedTileBaseDict = new Dictionary<TileType, CustomAnimatedTileBase>();

        private void Awake()
        {
            Instance = this;
            LoadTileBaseDictionary();
        }

        private void Start()
        {
            
        }

        private void LoadTileBaseDictionary()
        {  
            foreach(var tilebase in TileBaseList)
            {
                _tileBaseDict.Add(tilebase.Type, tilebase);
            }

            foreach (var animatedTile in AnimatedTileBaseList)
            {
                _animatedTileBaseDict.Add(animatedTile.Type, animatedTile);
            }
        }

        public TileBase GetTileBase(TileType tileType)
        {
            if(_tileBaseDict.ContainsKey(tileType))
            {
                return _tileBaseDict[tileType];
            }
            if (_animatedTileBaseDict.ContainsKey(tileType))
            {
                return _animatedTileBaseDict[tileType];
            }
            return null;
        }

    }
}

