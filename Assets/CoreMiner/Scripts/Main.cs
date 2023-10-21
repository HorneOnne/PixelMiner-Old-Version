using CoreMiner.WorldGen;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoreMiner
{
    public class Main : MonoBehaviour
    {
        public static Main Instance { get; private set; }

        public List<CustomTileBase> tileBaseList = new List<CustomTileBase>();
        private Dictionary<TileType, CustomTileBase> tileBaseDict = new Dictionary<TileType, CustomTileBase>();

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
            foreach(var tilebase in tileBaseList)
            {
                tileBaseDict.Add(tilebase.Type, tilebase);
            }
        }

        public TileBase GetTileBase(TileType tileType)
        {
            return tileBaseDict[tileType];
        }
    }
}

