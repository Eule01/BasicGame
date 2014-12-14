#region

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GameCore.Utils;

#endregion

namespace GameCore.Map
{
    public class Map
    {
        /// <summary>
        ///     The size of the map.
        /// </summary>
        private Size theSize = Size.Empty;


        private Tile[,] theTiles;

        public Map()
        {
            Init();
        }

        /// <summary>
        ///     Initialize the map object.
        /// </summary>
        private void Init()
        {
            theSize = new Size(20, 20);
            theTiles = CreatTestTiles(theSize);
        }

        public Size TheSize
        {
            get { return theSize; }
        }

        public Tile this[int x, int y]
        {
            get { return theTiles[x, y]; }
        }

        public List<Tile> Tiles
        {
            get { return theTiles.Cast<Tile>().ToList(); }
//            get { return new List<Tile>(theTiles.GetEnumerator());}
        }

        internal static Map GetTestMap()
        {
            return new Map();
        }


        private static Tile[,] CreatTestTiles(Size aSize)
        {
            Tile[,] tempTiles = new Tile[aSize.Width,aSize.Height];
            for (int x = 0; x < aSize.Width; x++)
            {
                for (int y = 0; y < aSize.Height; y++)
                {
                    Tile tempTile = new Tile(Tile.TileIds.Desert) {Location = new Vector(x, y)};
                    tempTiles[x, y] = tempTile;
                }
            }
            return tempTiles;
        }

        /// <summary>
        ///     Closes this object and disposes everything.
        /// </summary>
        public void Close()
        {
        }
    }
}