using GameCore.Utils;

namespace GameCore.Map
{
    /// <summary>
    ///     This represents a tile object, the map is made up of tiles.
    /// </summary>
    public class Tile
    {
        public enum TileIds
        {
            Desert,
            Grass,
            Road
        }

        public static Vector Size = new Vector(1,1);

        private TileIds theTileId = TileIds.Desert;

        public Tile(TileIds aTileId)
        {
            theTileId = aTileId;
        }

        public TileIds TheTileId
        {
            get { return theTileId; }
        }
    }
}