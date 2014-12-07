using System.Drawing;

namespace GameCore.Map
{
    public class Map
    {
        /// <summary>
        /// The size of the map.
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
            theSize = new Size(20, 30);
            theTiles = CreatTestTiles(theSize);
        }

        public Size TheSize
        {
            get { return theSize; }
        }

        public Tile this[int x, int y]
        {
            get
            {
                return theTiles[x,y];
            }
        }

        private static Tile[,] CreatTestTiles(Size aSize)
        {
            Tile[,] tempTiles = new Tile[aSize.Width, aSize.Height];
            for (int x = 0; x < aSize.Width; x++)
            {
                for (int y = 0; y < aSize.Height; y++)
                {
                    tempTiles[x,y] = new Tile(Tile.TileIds.Desert);
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